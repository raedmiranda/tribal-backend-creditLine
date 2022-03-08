namespace Tribal.Backend.CreditLine.Domain.Models
{
    public class CreditLineResponseModel
    {
        public decimal AuthorizedCreditLine { get; set; }
        public bool IsAccepted { get; set; }
    }
}
