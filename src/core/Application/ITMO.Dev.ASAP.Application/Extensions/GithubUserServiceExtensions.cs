using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Domain.Students;
using ITMO.Dev.ASAP.Domain.Submissions;
using ITMO.Dev.ASAP.Github.Application.Dto.Users;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using ITMO.Dev.ASAP.Mapping.Mappings;

namespace ITMO.Dev.ASAP.Application.Extensions;

public static class GithubUserServiceExtensions
{
    public static async Task<IReadOnlyCollection<StudentDto>> MapToStudentDtosAsync(
        this IGithubUserService service,
        IReadOnlyCollection<Student> students,
        CancellationToken cancellationToken)
    {
        IEnumerable<Guid> userIds = students.Select(x => x.UserId);

        IReadOnlyCollection<GithubUserDto> githubUsers = await service
            .FindByIdsAsync(userIds, cancellationToken);

        return students
            .GroupJoin(
                githubUsers,
                x => x.UserId,
                x => x.Id,
                (s, u) => (student: s, user: u.FirstOrDefault()))
            .Select(x => x.student.ToDto(x.user?.Username))
            .ToArray();
    }

    public static async Task<IReadOnlyList<QueueSubmissionDto>> MapToQueueSubmissionDto(
        this IGithubUserService service,
        IReadOnlyCollection<RatedSubmission> submissions,
        CancellationToken cancellationToken)
    {
        IEnumerable<Guid> userIds = submissions.Select(x => x.Submission.Student.UserId).Distinct();

        IReadOnlyCollection<GithubUserDto> githubUsers = await service
            .FindByIdsAsync(userIds, cancellationToken);

        return submissions
            .GroupJoin(
                githubUsers,
                x => x.Submission.Student.UserId,
                x => x.Id,
                (s, u) => (submission: s, user: u.FirstOrDefault()))
            .Select(x => x.submission.Submission.ToQueueDto(x.user?.Username, x.submission.Points))
            .ToArray();
    }

    public static async Task<StudentDto> MapToStudentDtoAsync(
        this IGithubUserService service,
        Student student,
        CancellationToken cancellationToken)
    {
        GithubUserDto? username = await service.FindByIdAsync(student.UserId, cancellationToken);
        return student.ToDto(username?.Username);
    }
}