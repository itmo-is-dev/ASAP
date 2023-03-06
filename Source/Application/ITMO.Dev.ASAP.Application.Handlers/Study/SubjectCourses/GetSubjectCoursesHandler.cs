using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Queries.GetSubjectCourses;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourses;

internal class GetSubjectCoursesHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;
    private readonly ICurrentUser _currentUser;

    public GetSubjectCoursesHandler(IDatabaseContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        if (_currentUser.Role is UserRoleType.Anonymous)
            throw new Exception("Anonymous user cannot access information about the subject courses");

        // AccessViolationException?
        List<SubjectCourse> subjectCourses = await _context
            .SubjectCourses
            .ToListAsync(cancellationToken);

        if (_currentUser.Role is UserRoleType.Mentor)
        {
            subjectCourses = subjectCourses
                .Where(s => s.Mentors.Any(m => m.UserId == _currentUser.Id))
                .ToList();

            if (subjectCourses.Count is 0)
                throw new Exception("Mentor has not any subject courses");
        }

        SubjectCourseDto[] dto = subjectCourses
            .Select(x => x.ToDto())
            .ToArray();

        return new Response(dto);
    }
}