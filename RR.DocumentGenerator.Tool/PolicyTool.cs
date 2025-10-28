using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using RR.DocumentGenerator.Dto;
using RR.DocumentGenerator.Dto.Actions;
using RR.DocumentGenerator.Service;
using System.ComponentModel;
using System.Text.Json;
using IContainer = QuestPDF.Infrastructure.IContainer;

namespace RR.DocumentGenerator.Tool
{
    [McpServerToolType]
    public sealed class PolicyTool(ILogger<PolicyTool> logger, 
        ITokenService tokenService,
        IBlobStorageService blobStorageService,
        IConfiguration configuration)
    {
        private readonly ILogger<PolicyTool> _logger = logger;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IBlobStorageService _blobStorageService = blobStorageService;
        private readonly IConfiguration _configuration = configuration;

        [McpServerTool, Description("Creates a professional Certificate of Insurance PDF document containing " +
            "policy details, carrier information, producer details, and insured company information. Returns a formatted PDF file " +
            "with proper headers, sections, and footer disclaimers suitable for official insurance documentation.")]
        public async Task<Uri> GenerateAsync([Description("Complete policy information including policy numbers," +
            " effective dates, carrier details (name, address, email), producer information (name, address, email), " +
            "and insured company details (name, address, phone). All fields are required to generate " +
            "a valid certificate of insurance.")] CreatePolicyActionDto request)
        {
            ArgumentNullException.ThrowIfNull(request);

            _logger.LogInformation("Generating policy document for Policy Number: {PolicyNumber}", request.PolicyNumber);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(50);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(content => ComposeContent(content, request));
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });
                });
            });

            var bytes = document.GeneratePdf();

            _logger.LogInformation("Successfully generated PDF for Policy Number: {PolicyNumber}, Size: {Size} bytes", 
                request.PolicyNumber, bytes.Length);

            var oid = _tokenService.GetOid();
            var fileName = $"Certificate_of_Insurance_{request.PolicyNumber}_{oid}_{DateTime.UtcNow.ToString("yyyyMMdd_HHmmss")}.pdf";
            var contentType = "application/pdf";
            var length = bytes.Length;

            var metadata = new Dictionary<string, string>
            {
                { "userId", oid.Value!.ToString() }
            };

            var container = _configuration["AzureStorage:TemporaryContainer"] ?? "temporary";

            await _blobStorageService.UploadAsync(container, fileName, bytes, metadata, CancellationToken.None);

            var sasUri = _blobStorageService.GenerateSasUri(container, fileName, TimeSpan.FromHours(1), 
                            BlobSasPermissions.Read);

            return sasUri;
        }

        private void ComposeHeader(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().BorderBottom(2).PaddingBottom(10).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("CERTIFICATE OF INSURANCE")
                            .FontSize(18)
                            .Bold()
                            .FontColor(Colors.Blue.Darken2);
                        col.Item().PaddingTop(5).Text($"Issue Date: {DateTime.UtcNow:MM/dd/yyyy}")
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken2);
                    });
                });

                column.Item().PaddingTop(10);
            });
        }

        private void ComposeContent(IContainer container, CreatePolicyActionDto request)
        {
            container.Column(column =>
            {
                // Policy Information Section
                column.Item().Element(c => ComposeSectionTitle(c, "POLICY INFORMATION"));
                column.Item().PaddingBottom(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    ComposeKeyValue(table, "Policy Number:", request.PolicyNumber);
                    ComposeKeyValue(table, "Certificate Number:", request.CertificateNumber);
                    ComposeKeyValue(table, "Issue Date:", request.IssueDate.ToString("MM/dd/yyyy"));
                    ComposeKeyValue(table, "Effective Date:", request.PolicyEffectiveDate.ToString("MM/dd/yyyy"));
                    ComposeKeyValue(table, "Expiration Date:", request.PolicyExpirationDate.ToString("MM/dd/yyyy"));
                });

                column.Item().PaddingTop(15);

                // Carrier Information Section
                column.Item().Element(c => ComposeSectionTitle(c, "CARRIER INFORMATION"));
                column.Item().PaddingBottom(10).Column(col =>
                {
                    col.Item().Text(request.CarrierName).Bold().FontSize(11);
                    col.Item().Text(request.CarrierAddress).FontSize(9);
                    col.Item().Text($"Email: {request.CarrierEmail}").FontSize(9);
                });

                column.Item().PaddingTop(15);

                // Producer Information Section
                column.Item().Element(c => ComposeSectionTitle(c, "PRODUCER INFORMATION"));
                column.Item().PaddingBottom(10).Column(col =>
                {
                    col.Item().Text(request.ProducerName).Bold().FontSize(11);
                    col.Item().Text(request.ProducerAddress).FontSize(9);
                    col.Item().Text($"Email: {request.ProducerEmail}").FontSize(9);
                });

                column.Item().PaddingTop(15);

                // Insured Information Section
                column.Item().Element(c => ComposeSectionTitle(c, "INSURED INFORMATION"));
                column.Item().PaddingBottom(10).Column(col =>
                {
                    col.Item().Text(request.InsuredCompanyName).Bold().FontSize(11);
                    col.Item().Text(request.InsuredCompanyAddress).FontSize(9);
                    col.Item().Text($"Phone: {request.InsuredCompanyPhone}").FontSize(9);
                });

                // Footer Disclaimer
                column.Item().PaddingTop(30).BorderTop(1).PaddingTop(10).Text(
                    "This certificate is issued as a matter of information only and confers no rights upon the certificate holder. " +
                    "This certificate does not affirmatively or negatively amend, extend or alter the coverage afforded by the policies below.")
                    .FontSize(8)
                    .Italic()
                    .FontColor(Colors.Grey.Darken1);
            });
        }

        private void ComposeSectionTitle(IContainer container, string title)
        {
            container.BorderBottom(1)
                .BorderColor(Colors.Blue.Darken2)
                .PaddingBottom(5)
                .Text(title)
                .FontSize(12)
                .Bold()
                .FontColor(Colors.Blue.Darken2);
        }

        private void ComposeKeyValue(TableDescriptor table, string key, string value)
        {
            table.Cell().Padding(3).Text(key).Bold().FontSize(9);
            table.Cell().Padding(3).Text(value).FontSize(9);
        }
    }
}
