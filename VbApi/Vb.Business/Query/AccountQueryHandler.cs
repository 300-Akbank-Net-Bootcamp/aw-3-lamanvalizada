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
    public class AccountQueryHandler :
        IRequestHandler<GetAllAccountsQuery, ApiResponse<List<AccountResponse>>>,
        IRequestHandler<GetAccountByIdQuery, ApiResponse<AccountResponse>>,
        IRequestHandler<GetAccountsByParameterQuery, ApiResponse<List<AccountResponse>>>
    {
        private readonly VbDbContext dbContext;
        private readonly IMapper mapper;

        public AccountQueryHandler(VbDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<ApiResponse<List<AccountResponse>>> Handle(GetAllAccountsQuery request,
            CancellationToken cancellationToken)
        {
            var list = await dbContext.Set<Account>()
                .Include(x => x.Customer)
                .Include(x => x.AccountTransactions)
                .Include(x => x.EftTransactions)
                .ToListAsync(cancellationToken);

            var mappedList = mapper.Map<List<Account>, List<AccountResponse>>(list);
            return new ApiResponse<List<AccountResponse>>(mappedList);
        }

        public async Task<ApiResponse<AccountResponse>> Handle(GetAccountByIdQuery request,
            CancellationToken cancellationToken)
        {
            var entity = await dbContext.Set<Account>()
                .Include(x => x.Customer)
                .Include(x => x.AccountTransactions)
                .Include(x => x.EftTransactions)
                .FirstOrDefaultAsync(x => x.AccountNumber == request.Id, cancellationToken);

            if (entity == null)
            {
                return new ApiResponse<AccountResponse>("Record not found");
            }

            var mapped = mapper.Map<Account, AccountResponse>(entity);
            return new ApiResponse<AccountResponse>(mapped);
        }

        public async Task<ApiResponse<List<AccountResponse>>> Handle(GetAccountsByParameterQuery request,
            CancellationToken cancellationToken)
        {
            var list = await dbContext.Set<Account>()
                .Include(x => x.Customer)
                .Include(x => x.AccountTransactions)
                .Include(x => x.EftTransactions)
                .Where(x =>
                    x.Name.ToUpper().Contains(request.Name.ToUpper()) ||
                    x.IBAN.ToUpper().Contains(request.IBAN.ToUpper())
                ).ToListAsync(cancellationToken);

            var mappedList = mapper.Map<List<Account>, List<AccountResponse>>(list);
            return new ApiResponse<List<AccountResponse>>(mappedList);
        }
    }
}