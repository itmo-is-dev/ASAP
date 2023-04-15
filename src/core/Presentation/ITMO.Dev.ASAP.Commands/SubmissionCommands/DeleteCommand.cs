using CommandLine;
using ITMO.Dev.ASAP.Commands.CommandVisitors;

namespace ITMO.Dev.ASAP.Commands.SubmissionCommands;

[Verb("/delete")]
public class DeleteCommand : ISubmissionCommand
{
    public Task AcceptAsync(ISubmissionCommandVisitor visitor)
    {
        return visitor.VisitAsync(this);
    }
}