using AutoMapper;
using Para.Business.Dtos;
using Para.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Para.Business.Mappings
{
    public class CustomerProfile:Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<CustomerDetail, CustomerDetailDto>().ReverseMap();
            CreateMap<CustomerAddress, CustomerAddressDto>().ReverseMap();
            CreateMap<CustomerPhone, CustomerPhoneDto>().ReverseMap();
        }
    }
}
