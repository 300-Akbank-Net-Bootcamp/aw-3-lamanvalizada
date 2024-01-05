using FluentValidation;
using Vb.Schema;

namespace Vb.Business.Validator
{
    public class CreateAccountValidator : AbstractValidator<AccountRequest>
    {
        public CreateAccountValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.AccountNumber).NotEmpty();
            RuleFor(x => x.IBAN).NotEmpty().MaximumLength(34);
            RuleFor(x => x.Balance).NotEmpty().GreaterThan(0);
            RuleFor(x => x.CurrencyType).NotEmpty().MaximumLength(3);
            RuleFor(x => x.Name).MaximumLength(100);
            RuleFor(x => x.OpenDate).NotEmpty();

            RuleForEach(x => x.AccountTransactions).SetValidator(new CreateAccountTransactionValidator());
            RuleForEach(x => x.EftTransactions).SetValidator(new CreateEftTransactionValidator());
        }
    }
}