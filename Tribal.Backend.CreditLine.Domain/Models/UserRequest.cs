using System;

namespace Tribal.Backend.CreditLine.Domain.Models
{
    public class UserRequest
    {
        public Guid Guid { get; set; }
        public Guid CustomerId { get; set; }
        public object Request { get; set; }
        public string RequestType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
