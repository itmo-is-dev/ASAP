using ITMO.Dev.ASAP.Application.Dto.SubjectCourseAssociations;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Github.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Github.Presentation.Contracts.Services;
using ITMO.Dev.ASAP.Mapping.Mappings;

namespace ITMO.Dev.ASAP.Application.Handlers.Extensions;

public static class GithubSubjectCourseExtensions
{
    public static async Task<IReadOnlyCollection<SubjectCourseDto>> MapToSubjectCourseDtoAsync(
        this IGithubSubjectCourseService service,
        IReadOnlyCollection<SubjectCourse> subjectCourses,
        CancellationToken cancellationToken)
    {
        IEnumerable<Guid> courseIds = subjectCourses.Select(x => x.Id);

        IReadOnlyCollection<GithubSubjectCourseDto> githubSubjectCourses = await service
            .FindByIdsAsync(courseIds, cancellationToken);

        return subjectCourses
            .GroupJoin(
                githubSubjectCourses,
                x => x.Id,
                x => x.Id,
                (c, g) =>
                {
                    IEnumerable<GithubSubjectCourseAssociationDto> associations = g
                        .Select(x => new GithubSubjectCourseAssociationDto(
                            x.OrganizationName,
                            x.TemplateRepositoryName,
                            x.MentorTeamName));

                    return c.ToDto(associations);
                })
            .ToArray();
    }
}