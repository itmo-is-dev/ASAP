using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Identity.Commands;

public class UpdatePassword
{
    public record struct Command(string CurrentPassword, string NewPassword) : IRequest;
}