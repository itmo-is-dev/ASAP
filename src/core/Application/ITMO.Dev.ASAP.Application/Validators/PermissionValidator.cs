using ITMO.Dev.ASAP.Application.Abstractions.Permissions;
using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses;

namespace ITMO.Dev.ASAP.Application.Validators;

public class PermissionValidator : IPermissionValidator
{
    private readonly IPersistenceContext _context;

    public PermissionValidator(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task<bool> IsSubmissionMentorAsync(Guid userId, Guid submissionId, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses
            .GetBySubmissionIdAsync(submissionId, cancellationToken);

        return subjectCourse.Mentors.Any(x => x.UserId.Equals(userId));
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