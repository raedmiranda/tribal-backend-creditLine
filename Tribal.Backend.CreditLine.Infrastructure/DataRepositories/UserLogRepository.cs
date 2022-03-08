using System.Collections.Generic;
using Tribal.Backend.CreditLine.Domain.Intefaces.Repositories;
using Tribal.Backend.CreditLine.Domain.Models;

namespace Tribal.Backend.CreditLine.Infrastructure.DataRepositories
{
    public class UserLogRepository : Repository<UserRequest>, IUserLogRepository
    {
        public UserLogRepository(List<UserRequest> context) : base(context)
        {
        }
    }
}
