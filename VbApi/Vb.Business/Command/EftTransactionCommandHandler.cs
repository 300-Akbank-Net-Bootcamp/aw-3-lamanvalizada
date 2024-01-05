using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;

namespace Vb.Business.Command
{
    public class EftTransactionCommandHandler :
        IRequestHandler<CreateEftTransactionCommand, ApiResponse<EftTransactionResponse>>,
        IRequestHandler<UpdateEftTransactionCommand, ApiResponse>,
        IRequestHandler<DeleteEftTransactionCommand, ApiResponse>
    {
        private readonly VbDbContext dbContext;
        private readonly IMapper mapper;

        public EftTransactionCommandHandler(VbDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<ApiResponse<EftTransactionResponse>> Handle(CreateEftTransactionCommand request, CancellationToken cancellationToken)
        {
            var checkReferenceNumber = await dbContext.Set<EftTransaction>().Where(x => x.ReferenceNumber == request.Model.ReferenceNumber)
                .FirstOrDefaultAsync(cancellationToken);
            if (checkReferenceNumber != null)
            {
                return new ApiResponse<EftTransactionResponse>($"ReferenceNumber {request.Model.ReferenceNumber} is used by another EftTransaction.");
            }

            var entity = mapper.Map<EftTransactionRequest, EftTransaction>(request.Model);
            entity.InsertDate = DateTime.UtcNow;
            entity.IsActive = true;

            var entityResult = await dbContext.AddAsync(entity, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            var mapped = mapper.Map<EftTransaction, EftTransactionResponse>(entityResult.Entity);
            return new ApiResponse<EftTransactionResponse>(mapped);
        }

        public async Task<ApiResponse> Handle(UpdateEftTransactionCommand request, CancellationToken cancellationToken)
        {
            var fromDb = await dbContext.Set<EftTransaction>().Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (fromDb == null)
            {
                return new ApiResponse("Record not found");
            }

            fromDb.Amount = request.Model.Amount;
            fromDb.Description = request.Model.Description;
            fromDb.SenderName = request.Model.SenderName;

            await dbContext.SaveChangesAsync(cancellationToken);
            return new ApiResponse();
        }

        public async Task<ApiResponse> Handle(DeleteEftTransactionCommand request, CancellationToken cancellationToken)
        {
            var fromDb = await dbContext.Set<EftTransaction>().Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (fromDb == null)
            {
                return new ApiResponse("Record not found");
            }

            fromDb.IsActive = false;
            await dbContext.SaveChangesAsync(cancellationToken);
            return new ApiResponse();
        }
    }
}
