using CommandLine;
using ITMO.Dev.ASAP.Commands.CommandVisitors;

namespace ITMO.Dev.ASAP.Commands.SubmissionCommands;

[Verb("/ban")]
public class BanCommand : ISubmissionCommand
{
    public BanCommand(int? submissionCode)
    {
        SubmissionCode = submissionCode;
    }

    [Value(0, Required = false, MetaName = "SubmissionCode")]
    public int? SubmissionCode { get; }

    public Task AcceptAsync(ISubmissionCommandVisitor visitor)
    {
        return visitor.VisitAsync(this);
    }
}