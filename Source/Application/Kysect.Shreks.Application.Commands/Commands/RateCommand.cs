using CommandLine;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Core.Submissions;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.Commands.Commands;

[Verb("/rate", aliases: new []{ "/assess" })]
public class RateCommand : IShreksCommand
{
    public RateCommand(double ratingPercent, double? extraPoints)
    {
        RatingPercent = ratingPercent;
        ExtraPoints = extraPoints;
    }

    [Value(0, Required = true, MetaName = "RatingPercent")]
    public double RatingPercent { get; }

    [Value(1, Required = false, Default = 0.0, MetaName = "ExtraPoints")]
    public double? ExtraPoints { get; }

    public async Task<Submission> ExecuteAsync(SubmissionContext context, ILogger logger, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Handle /rate command from {context.IssuerId} with arguments: {ToLogLine()}");

        return await context.SubmissionService.RateSubmissionAsync(
            context.SubmissionId,
            context.IssuerId,
            RatingPercent,
            ExtraPoints,
            cancellationToken);
    }

    public string ToLogLine()
    {
        return $"RatingPercent: {RatingPercent}, ExtraPoints: {ExtraPoints}";
    }
}