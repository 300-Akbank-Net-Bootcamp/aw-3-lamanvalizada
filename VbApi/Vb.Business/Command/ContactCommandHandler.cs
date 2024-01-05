using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

namespace Vb.Business.Command
{
    public class ContactCommandHandler :
        IRequestHandler<CreateContactCommand, ApiResponse<ContactResponse>>,
        IRequestHandler<UpdateContactCommand, ApiResponse>,
        IRequestHandler<DeleteContactCommand, ApiResponse>
    {
        private readonly VbDbContext dbContext;
        private readonly IMapper mapper;

        public ContactCommandHandler(VbDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<ApiResponse<ContactResponse>> Handle(CreateContactCommand request, CancellationToken cancellationToken)
        {
           
            var entity = mapper.Map<ContactRequest, Contact>(request.Model);

            var entityResult = await dbContext.AddAsync(entity, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            var mapped = mapper.Map<Contact, ContactResponse>(entityResult.Entity);
            return new ApiResponse<ContactResponse>(mapped);
        }

        public async Task<ApiResponse> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
        {
            var fromDb = await dbContext.Set<Contact>().Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (fromDb == null)
            {
                return new ApiResponse("Record not found");
            }

            fromDb.ContactType = request.Model.ContactType;
            fromDb.Information = request.Model.Information;

            await dbContext.SaveChangesAsync(cancellationToken);
            return new ApiResponse();
        }

        public async Task<ApiResponse> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
        {
            var fromDb = await dbContext.Set<Contact>().Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (fromDb == null)
            {
                return new ApiResponse("Record not found");
            }

            // dbContext.Set<Contact>().Remove(fromDb);

            fromDb.IsActive = false;
            await dbContext.SaveChangesAsync(cancellationToken);
            return new ApiResponse();
        }
    }
}