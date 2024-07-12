using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Para.Business.Dtos
{
    public class CustomerAddressDto
    {
        public int Id { get; set; }
        public string InsertUser { get; set; }
        public DateTime InsertDate { get; set; }
        public bool IsActive { get; set; }
        public long CustomerId { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string AddressLine { get; set; }
        public string ZipCode { get; set; }
        public bool IsDefault { get; set; }
    }
}
