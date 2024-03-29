using MediatR;
using System.Collections.Generic;
using Vb.Base.Response;
using Vb.Schema;

namespace Vb.Business.Cqrs
{
    public record CreateAddressCommand(AddressRequest Model) : IRequest<ApiResponse<AddressResponse>>;
    public record UpdateAddressCommand(int Id, AddressRequest Model) : IRequest<ApiResponse>;
    public record DeleteAddressCommand(int Id) : IRequest<ApiResponse>;

    public record GetAllAddressesQuery() : IRequest<ApiResponse<List<AddressResponse>>>;
    public record GetAddressByIdQuery(int Id) : IRequest<ApiResponse<AddressResponse>>;
    public record GetAddressesByParameterQuery(string City, string PostalCode) : IRequest<ApiResponse<List<AddressResponse>>>;
}
