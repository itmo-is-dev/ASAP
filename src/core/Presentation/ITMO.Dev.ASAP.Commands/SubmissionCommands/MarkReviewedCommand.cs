using CommandLine;
using ITMO.Dev.ASAP.Commands.CommandVisitors;

namespace ITMO.Dev.ASAP.Commands.SubmissionCommands;

[Verb("/mark-reviewed")]
public class MarkReviewedCommand : ISubmissionCommand
{
    public Task AcceptAsync(ISubmissionCommandVisitor visitor)
    {
        return visitor.VisitAsync(this);
    }
}