using System;

namespace Tribal.Backend.CreditLine.Domain
{
    public class CreditLineRequestModel
    {
        /// <summary>
        /// Business type
        /// </summary>
        public string FoundingType { get; set; }
        /// <summary>
        /// The customer's bank account balance
        /// </summary>
        public decimal CashBalance { get; set; }
        /// <summary>
        /// The total sales revenue for the month
        /// </summary>
        public decimal MonthlyRevenue { get; set; }
        /// <summary>
        /// Requested credit line
        /// </summary>
        public decimal RequestedCreditLine { get; set; }
        /// <summary>
        /// Represents when request was made
        /// </summary>
        public DateTime RequestedDate { get; set; }
    }
}
