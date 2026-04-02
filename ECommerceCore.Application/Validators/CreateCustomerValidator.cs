using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentValidation;
using System.Threading.Tasks;
using ECommerceCore.Application.DTOs.CustomerDTO;

namespace ECommerceCore.Application.Validators
{
    public class CreateCustomerValidator : AbstractValidator<CreateCustomerDto>
    {
        public CreateCustomerValidator()
        {

            RuleFor(x => x.Name).NotEmpty().WithMessage("Name should not be empty").MinimumLength(3).WithMessage("Minimum 3 charectors required");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("Invalid email");

                

        }

    }
}
