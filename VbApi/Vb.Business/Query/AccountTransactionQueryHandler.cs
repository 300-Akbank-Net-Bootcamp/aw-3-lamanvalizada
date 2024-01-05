using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

namespace Vb.Business.Query
{
    public class AccountTransactionQueryHandler :
        IRequestHandler<GetAllAccountTransactionsQuery, ApiResponse<List<AccountTransactionResponse>>>,
        IRequestHandler<GetAccountTransactionByIdQuery, ApiResponse<AccountTransactionResponse>>,
        IRequestHandler<GetAccountTransactionsByParameterQuery, ApiResponse<List<AccountTransactionResponse>>>
    {
        private readonly VbDbContext dbContext;
        private readonly IMapper mapper;

        public AccountTransactionQueryHandler(VbDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<ApiResponse<List<AccountTransactionResponse>>> Handle(GetAllAccountTransactionsQuery request,
            CancellationToken cancellationToken)
        {
            var list = await dbContext.Set<AccountTransaction>()
                .Include(x => x.Account)
                .ToListAsync(cancellationToken);

            var mappedList = mapper.Map<List<AccountTransaction>, List<AccountTransactionResponse>>(list);
            return new ApiResponse<List<AccountTransactionResponse>>(mappedList);
        }

        public async Task<ApiResponse<AccountTransactionResponse>> Handle(GetAccountTransactionByIdQuery request,
            CancellationToken cancellationToken)
        {
            var entity = await dbContext.Set<AccountTransaction>()
                .Include(x => x.Account)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                return new ApiResponse<AccountTransactionResponse>("Record not found");
            }

            var mapped = mapper.Map<AccountTransaction, AccountTransactionResponse>(entity);
            return new ApiResponse<AccountTransactionResponse>(mapped);
        }

        public async Task<ApiResponse<List<AccountTransactionResponse>>> Handle(GetAccountTransactionsByParameterQuery request,
            CancellationToken cancellationToken)
        {
            var list = await dbContext.Set<AccountTransaction>()
                .Include(x => x.Account)
                .Where(x =>
                    x.ReferenceNumber.ToUpper().Contains(request.ReferenceNumber.ToUpper()) ||
                    x.Description.ToUpper().Contains(request.Description.ToUpper())
                ).ToListAsync(cancellationToken);

            var mappedList = mapper.Map<List<AccountTransaction>, List<AccountTransactionResponse>>(list);
            return new ApiResponse<List<AccountTransactionResponse>>(mappedList);
        }
    }
}