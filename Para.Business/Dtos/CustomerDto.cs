using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Para.Business.Dtos
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string InsertUser { get; set; }
        public DateTime InsertDate { get; set; }
        public bool IsActive { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdentityNumber { get; set; }
        public string Email { get; set; }
        public int CustomerNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public CustomerDetailDto CustomerDetail { get; set; }
        public List<CustomerAddressDto> CustomerAddresses { get; set; }
        public List<CustomerPhoneDto> CustomerPhones { get; set; }
    }
}
