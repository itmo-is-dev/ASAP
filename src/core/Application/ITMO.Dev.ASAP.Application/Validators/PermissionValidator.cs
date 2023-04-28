using ITMO.Dev.ASAP.Application.Abstractions.Permissions;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ITMO.Dev.ASAP.Application.Validators;

public class PermissionValidator : IPermissionValidator
{
    private readonly IDatabaseContext _context;

    public PermissionValidator(IDatabaseContext context)
    {
        _context = context;
    }

    public Task<bool> IsSubmissionMentorAsync(Guid userId, Guid submissionId, CancellationToken cancellationToken)
    {
        return _context.Submissions
            .Where(x => x.Id.Equals(submissionId))
            .Select(x => x.GroupAssignment.Assignment.SubjectCourse)
            .SelectMany(x => x.Mentors)
            .AnyAsync(x => x.UserId.Equals(userId), cancellationToken);
    }

    public async Task EnsureSubmissionMentorAsync(
        Guid userId,
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        if (await IsSubmissionMentorAsync(userId, submissionId, cancellationToken) is false)
            throw new UnauthorizedException();
    }
}