using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Github.Queries.GetCurrentUnratedSubmissionByPrNumber;

namespace Kysect.Shreks.Application.Handlers.Github;


public class GetCurrentUnratedSubmissionByPrNumberHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;
    public GetCurrentUnratedSubmissionByPrNumberHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var submission = await _context.SubmissionAssociations
            .OfType<GithubSubmissionAssociation>()
            .Where(a =>
                a.Organization == request.PullRequestDescriptor.Organization
                && a.Repository == request.PullRequestDescriptor.Repository
                && a.PrNumber == request.PullRequestDescriptor.PrNumber
                && a.Submission.Rating == null)
            .OrderByDescending(a => a.Submission.SubmissionDate)
            .Select(a => a.Submission)
            .FirstOrDefaultAsync(cancellationToken);

        if (submission is null)
        {
            var message = $"No unrated submission in pr {request.PullRequestDescriptor.Payload}";
            throw new EntityNotFoundException(message);
        }

        return new Response(_mapper.Map<SubmissionDto>(submission));
    }
}