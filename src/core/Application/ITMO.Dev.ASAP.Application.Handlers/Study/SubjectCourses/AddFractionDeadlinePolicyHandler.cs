using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.Specifications;
using ITMO.Dev.ASAP.Domain.Deadlines.DeadlinePenalties;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses;
using ITMO.Dev.ASAP.Domain.Study.SubjectCourses.Events;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Commands.AddFractionDeadlinePolicy;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourses;

internal class AddFractionDeadlinePolicyHandler : IRequestHandler<Command>
{
    private readonly IPersistenceContext _context;

    public AddFractionDeadlinePolicyHandler(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses
            .GetByIdAsync(request.SubjectCourseId, cancellationToken);

        var penalty = new FractionDeadlinePenalty(request.SpanBeforeActivation, request.Fraction);
        DeadlinePenaltyAddedEvent evt = subjectCourse.AddDeadlinePenalty(penalty);

        await _context.SubjectCourses.ApplyAsync(evt, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}