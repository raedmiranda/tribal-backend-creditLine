using System;

namespace Tribal.Backend.CreditLine.Domain.Models
{
    public class CustomerCreditLine
    {
        public Guid Guid { get; set; }
        public Guid CustomerId { get; set; }

        // CreditLineResponseModel Begin
        public decimal AcceptedCreditLine { get; set; }
        public bool AcceptedStatus { get; set; }
        // CreditLineResponseModel End

        public DateTime CreatedAt { get; set; }
    }
}
