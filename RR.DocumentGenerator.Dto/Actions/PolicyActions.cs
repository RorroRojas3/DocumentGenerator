using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.DocumentGenerator.Dto.Actions
{
    public class CreatePolicyActionDto
    {
        public string PolicyNumber { get; set; } = null!;

        public string CertificateNumber { get; set; } = null!;

        public DateOnly PolicyEffectiveDate { get; set; }

        public DateOnly PolicyExpirationDate { get; set; }

        public DateOnly IssueDate { get; set; }

        public string CarrierName { get; set; } = null!;

        public string CarrierAddress { get; set; } = null!;

        public string CarrierEmail { get; set; } = null!;

        public string ProducerName { get; set; } = null!;

        public string ProducerAddress { get; set; } = null!;

        public string ProducerEmail { get; set; } = null!;

        public string InsuredCompanyName { get; set; } = null!;

        public string InsuredCompanyAddress { get; set; } = null!;

        public string InsuredCompanyPhone { get; set; } = null!;
    }
}
