using System.Text;

namespace ITMO.Dev.ASAP.Application.Dto.Submissions;

public record SubmissionRateDto(
    Guid Id,
    int Code,
    string State,
    DateTime SubmissionDate,
    double? Rating,
    double? RawPoints,
    double? MaxRawPoints,
    double? ExtraPoints,
    double? PenaltyPoints,
    double? TotalPoints)
{
    public string ToDisplayString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"Submission code: {Code} ({SubmissionDate.ToString("dd.MM.yyyy")})");

        if (Rating is not null)
            stringBuilder.AppendLine($"Rating: {Rating}");

        if (RawPoints is not null)
        {
            if (ExtraPoints is not null && ExtraPoints != 0)
                stringBuilder.AppendLine($"Points: {RawPoints}/{MaxRawPoints} (+{ExtraPoints} extra points)");
            else
                stringBuilder.AppendLine($"Points: {RawPoints}/{MaxRawPoints}");
        }

        if (PenaltyPoints is not null && PenaltyPoints != 0)
            stringBuilder.AppendLine($"Penalty points: {PenaltyPoints}");

        if (TotalPoints is not null)
            stringBuilder.AppendLine($"Total points: {TotalPoints}");

        return stringBuilder.ToString();
    }
}