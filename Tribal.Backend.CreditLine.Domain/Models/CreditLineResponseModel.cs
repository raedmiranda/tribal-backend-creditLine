using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tribal.Backend.CreditLine.Domain.Models
{
    public class CreditLineResponseModel
    {
        public decimal AuthorizedCreditLine { get; set; }
        public bool IsAccepted { get; set; }
    }
}
