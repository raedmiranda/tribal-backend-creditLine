using System.Collections.Generic;
using Tribal.Backend.CreditLine.Domain.Intefaces.Repositories;
using Tribal.Backend.CreditLine.Domain.Models;

namespace Tribal.Backend.CreditLine.Infrastructure.DataRepositories
{
    public class CreditRepository : Repository<CustomerCreditLine>, ICreditRepository
    {
        public CreditRepository(List<CustomerCreditLine> context) : base(context)
        {
        }
    }
}
