using System.Text.Json.Serialization;

namespace RR.DocumentGenerator.Dto.Actions
{
    public class CreatePolicyActionDto
    {
        [JsonPropertyName("policyNumber")]
        public string PolicyNumber { get; set; } = null!;

        [JsonPropertyName("certificateNumber")]
        public string CertificateNumber { get; set; } = null!;

        [JsonPropertyName("policyEffectiveDate")]
        public DateOnly PolicyEffectiveDate { get; set; }

        [JsonPropertyName("policyExpirationDate")]
        public DateOnly PolicyExpirationDate { get; set; }

        [JsonPropertyName("issueDate")]
        public DateOnly IssueDate { get; set; }

        [JsonPropertyName("carrierName")]
        public string CarrierName { get; set; } = null!;

        [JsonPropertyName("carrierAddress")]
        public string CarrierAddress { get; set; } = null!;

        [JsonPropertyName("carrierEmail")]
        public string CarrierEmail { get; set; } = null!;

        [JsonPropertyName("producerName")]
        public string ProducerName { get; set; } = null!;

        [JsonPropertyName("producerAddress")]
        public string ProducerAddress { get; set; } = null!;

        [JsonPropertyName("producerEmail")]
        public string ProducerEmail { get; set; } = null!;

        [JsonPropertyName("insuredCompanyName")]
        public string InsuredCompanyName { get; set; } = null!;

        [JsonPropertyName("insuredCompanyAddress")]
        public string InsuredCompanyAddress { get; set; } = null!;

        [JsonPropertyName("insuredCompanyPhone")]
        public string InsuredCompanyPhone { get; set; } = null!;
    }
}
