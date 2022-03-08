using System;

namespace Tribal.Backend.CreditLine.Domain.Models
{
    public class CustomerCreditLine
    {
        public Guid Guid { get; set; }
        public Guid CustomerId { get; set; }

        // CreditLineResponseModel
        public decimal AcceptedCreditLine { get; set; }
        public bool AcceptedStatus { get; set; }
        //

        public DateTime CreatedAt { get; set; }
    }
}
