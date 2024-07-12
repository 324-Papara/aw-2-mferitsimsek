using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Para.Bussiness.Validatiors
{
    public class CustomerAddressValidator : AbstractValidator<CustomerAddress>
    {
        public CustomerAddressValidator()
        {
            RuleFor(x => x.InsertUser).NotEmpty().MaximumLength(50);
            RuleFor(x => x.InsertDate).NotEmpty();
            RuleFor(x => x.IsActive).NotNull();
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.Country).NotEmpty().MaximumLength(50);
            RuleFor(x => x.City).NotEmpty().MaximumLength(50);
            RuleFor(x => x.AddressLine).NotEmpty().MaximumLength(250);
            RuleFor(x => x.ZipCode).MaximumLength(6);
            RuleFor(x => x.IsDefault).NotNull();
        }
    }
}
