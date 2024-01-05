using FluentValidation;
using Vb.Schema;

namespace Vb.Business.Validator
{
    public class CreateAddressValidator : AbstractValidator<AddressRequest>
    {
        public CreateAddressValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.Address1).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Address2).MaximumLength(150);
            RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
            RuleFor(x => x.City).NotEmpty().MaximumLength(100);
            RuleFor(x => x.County).MaximumLength(100);
            RuleFor(x => x.PostalCode).MaximumLength(10);
            RuleFor(x => x.IsDefault).NotEmpty();
        }
    }
}