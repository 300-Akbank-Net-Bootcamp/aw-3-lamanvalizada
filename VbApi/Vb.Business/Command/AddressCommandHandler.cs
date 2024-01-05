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
using Vb.Schema;

namespace Vb.Business.Command
{
    public class AddressCommandHandler :
        IRequestHandler<CreateAddressCommand, ApiResponse<AddressResponse>>,
        IRequestHandler<UpdateAddressCommand, ApiResponse>,
        IRequestHandler<DeleteAddressCommand, ApiResponse>
    {
        private readonly VbDbContext dbContext;
        private readonly IMapper mapper;

        public AddressCommandHandler(VbDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<ApiResponse<AddressResponse>> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
        {
            var entity = mapper.Map<AddressRequest, Address>(request.Model);

            var customerExists = await dbContext.Customers.AnyAsync(c => c.CustomerId == entity.CustomerId, cancellationToken);
            if (!customerExists)
            {
                return new ApiResponse<AddressResponse>($"Customer with ID {entity.CustomerId} not found.");
            }

            var addressExists = await dbContext.Addresses.AnyAsync(a => a.CustomerId == entity.CustomerId && a.IsDefault, cancellationToken);
            if (request.Model.IsDefault && addressExists)
            {
                return new ApiResponse<AddressResponse>("Default address already exists for this customer.");
            }

            await dbContext.AddAsync(entity, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            var mapped = mapper.Map<Address, AddressResponse>(entity);
            return new ApiResponse<AddressResponse>(mapped);
        }

        public async Task<ApiResponse> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
        {
            var fromDb = await dbContext.Addresses
                .Where(a => a.AddressId == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (fromDb == null)
            {
                return new ApiResponse("Record not found");
            }

            mapper.Map(request.Model, fromDb);

            var addressExists = await dbContext.Addresses
                .AnyAsync(a => a.CustomerId == fromDb.CustomerId && a.AddressId != fromDb.AddressId && a.IsDefault, cancellationToken);

            if (request.Model.IsDefault && addressExists)
            {
                return new ApiResponse("Default address already exists for this customer.");
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            return new ApiResponse();
        }

        public async Task<ApiResponse> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
        {
            var fromDb = await dbContext.Addresses
                .Where(a => a.AddressId == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (fromDb == null)
            {
                return new ApiResponse("Record not found");
            }

            dbContext.Addresses.Remove(fromDb);
            await dbContext.SaveChangesAsync(cancellationToken);
            return new ApiResponse();
        }
    }
}
