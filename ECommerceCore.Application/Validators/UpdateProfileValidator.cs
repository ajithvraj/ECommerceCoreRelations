using ECommerceCore.Application.DTOs.CustomerDTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceCore.Application.Validators
{
    public class UpdateProfileValidator : AbstractValidator<UpdateProfileDto>
    {

        public UpdateProfileValidator()
        {

            RuleFor(x => x.Name).NotEmpty().WithErrorCode("Name should not be empty").MinimumLength(4).WithMessage("Name must contain atleast 4 characters");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email cannot be empty").EmailAddress().WithMessage("Inavlid email address");
        
        
        }



    }
}
