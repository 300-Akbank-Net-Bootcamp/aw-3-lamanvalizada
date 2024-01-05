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
    public class ContactQueryHandler :
        IRequestHandler<GetAllContactsQuery, ApiResponse<List<ContactResponse>>>,
        IRequestHandler<GetContactByIdQuery, ApiResponse<ContactResponse>>,
        IRequestHandler<GetContactsByParameterQuery, ApiResponse<List<ContactResponse>>>
    {
        private readonly VbDbContext dbContext;
        private readonly IMapper mapper;

        public ContactQueryHandler(VbDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<ApiResponse<List<ContactResponse>>> Handle(GetAllContactsQuery request,
            CancellationToken cancellationToken)
        {
            var list = await dbContext.Set<Contact>()
                .Include(x => x.Customer)
                .ToListAsync(cancellationToken);

            var mappedList = mapper.Map<List<Contact>, List<ContactResponse>>(list);
            return new ApiResponse<List<ContactResponse>>(mappedList);
        }

        public async Task<ApiResponse<ContactResponse>> Handle(GetContactByIdQuery request,
            CancellationToken cancellationToken)
        {
            var entity = await dbContext.Set<Contact>()
                .Include(x => x.Customer)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                return new ApiResponse<ContactResponse>("Record not found");
            }

            var mapped = mapper.Map<Contact, ContactResponse>(entity);
            return new ApiResponse<ContactResponse>(mapped);
        }

        public async Task<ApiResponse<List<ContactResponse>>> Handle(GetContactsByParameterQuery request,
            CancellationToken cancellationToken)
        {
            var list = await dbContext.Set<Contact>()
                .Include(x => x.Customer)
                .Where(x =>
                    x.Information.ToUpper().Contains(request.Information.ToUpper()) ||
                    x.ContactType.ToUpper().Contains(request.ContactType.ToUpper())
                ).ToListAsync(cancellationToken);

            var mappedList = mapper.Map<List<Contact>, List<ContactResponse>>(list);
            return new ApiResponse<List<ContactResponse>>(mappedList);
        }
    }
}