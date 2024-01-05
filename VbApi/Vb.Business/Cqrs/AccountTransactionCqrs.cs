using MediatR;
using Vb.Base.Response;
using Vb.Schema;
using System.Collections.Generic;

namespace Vb.Business.Cqrs
{
    public record CreateAccountTransactionCommand(AccountTransactionRequest Model) : IRequest<ApiResponse<AccountTransactionResponse>>;
    public record UpdateAccountTransactionCommand(int Id, AccountTransactionRequest Model) : IRequest<ApiResponse>;
    public record DeleteAccountTransactionCommand(int Id) : IRequest<ApiResponse>;

    public record GetAllAccountTransactionsQuery() : IRequest<ApiResponse<List<AccountTransactionResponse>>>;
    public record GetAccountTransactionByIdQuery(int Id) : IRequest<ApiResponse<AccountTransactionResponse>>;
    public record GetAccountTransactionsByParameterQuery(string ReferenceNumber, string Description) : IRequest<ApiResponse<List<AccountTransactionResponse>>>;
}