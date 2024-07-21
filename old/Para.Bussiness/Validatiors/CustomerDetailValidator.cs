using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Para.Bussiness.Validatiors
{
    public class CustomerDetailValidator : AbstractValidator<CustomerDetail>
    {
        public CustomerDetailValidator()
        {
            RuleFor(x => x.InsertUser).NotEmpty().MaximumLength(50);
            RuleFor(x => x.InsertDate).NotEmpty();
            RuleFor(x => x.IsActive).NotNull();
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.FatherName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.MotherName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.MontlyIncome).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Occupation).NotEmpty().MaximumLength(50);
            RuleFor(x => x.EducationStatus).NotEmpty().MaximumLength(50);
        }
    }
}
