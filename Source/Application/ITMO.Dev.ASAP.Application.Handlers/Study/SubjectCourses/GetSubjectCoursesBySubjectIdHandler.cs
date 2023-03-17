using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Application.Common.Exceptions;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Queries.GetSubjectCoursesBySubjectId;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourses;

internal class GetSubjectCoursesBySubjectIdHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;
    private readonly ICurrentUser _currentUser;

    public GetSubjectCoursesBySubjectIdHandler(IDatabaseContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        Subject subject = await _context.Subjects
            .Include(x => x.Courses)
            .GetByIdAsync(request.SubjectId, cancellationToken);

        if (_currentUser.HasAccessToSubject(subject) is false)
            throw new AccessViolationException();

        var availableSubjectCourses = subject.Courses
            .Where(_currentUser.HasAccessToSubjectCourse)
            .ToList();

        if (availableSubjectCourses.Count is 0)
            throw UserHasNotAccessException.EmptyAvailableList(_currentUser.Id);

        SubjectCourseDto[] dto = availableSubjectCourses
            .Select(x => x.ToDto())
            .ToArray();

        return new Response(dto);
    }
}