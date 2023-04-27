using ITMO.Dev.ASAP.Application.Dto.Users;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Students.Commands;

internal static class TransferStudent
{
    public record Command(Guid StudentId, Guid GroupId) : IRequest<Response>;

    public record Response(StudentDto Student);
}