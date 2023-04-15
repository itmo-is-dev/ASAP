using ITMO.Dev.ASAP.Commands.CommandVisitors;

namespace ITMO.Dev.ASAP.Commands.SubmissionCommands;

public interface ISubmissionCommand
{
    Task AcceptAsync(ISubmissionCommandVisitor visitor);
}