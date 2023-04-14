using ITMO.Dev.ASAP.Application.Abstractions.Submissions;
using ITMO.Dev.ASAP.Application.Dto.Submissions;
using ITMO.Dev.ASAP.Presentation.Contracts.Services;

namespace ITMO.Dev.ASAP.Presentation.Services.Implementations;

public class AsapSubmissionWorkflowService : IAsapSubmissionWorkflowService
{
    private readonly ISubmissionWorkflowService _service;

    public AsapSubmissionWorkflowService(ISubmissionWorkflowService service)
    {
        _service = service;
    }

    public async Task<SubmissionActionMessageDto> SubmissionApprovedAsync(
        Guid issuerId,
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        ISubmissionWorkflow workflow = await _service.GetSubmissionWorkflowAsync(submissionId, cancellationToken);
        return await workflow.SubmissionApprovedAsync(issuerId, submissionId, cancellationToken);
    }

    public async Task<SubmissionActionMessageDto> SubmissionReactivatedAsync(
        Guid issuerId,
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        ISubmissionWorkflow workflow = await _service.GetSubmissionWorkflowAsync(submissionId, cancellationToken);
        return await workflow.SubmissionReactivatedAsync(issuerId, submissionId, cancellationToken);
    }

    public async Task<SubmissionUpdateResult> SubmissionUpdatedAsync(
        Guid issuerId,
        Guid userId,
        Guid assignmentId,
        string payload,
        CancellationToken cancellationToken)
    {
        ISubmissionWorkflow workflow = await _service.GetAssignmentWorkflowAsync(assignmentId, cancellationToken);
        return await workflow.SubmissionUpdatedAsync(issuerId, userId, assignmentId, payload, cancellationToken);
    }

    public async Task<SubmissionActionMessageDto> SubmissionAcceptedAsync(
        Guid issuerId,
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        ISubmissionWorkflow workflow = await _service.GetSubmissionWorkflowAsync(submissionId, cancellationToken);
        return await workflow.SubmissionAcceptedAsync(issuerId, submissionId, cancellationToken);
    }

    public async Task<SubmissionActionMessageDto> SubmissionRejectedAsync(
        Guid issuerId,
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        ISubmissionWorkflow workflow = await _service.GetSubmissionWorkflowAsync(submissionId, cancellationToken);
        return await workflow.SubmissionRejectedAsync(issuerId, submissionId, cancellationToken);
    }

    public async Task<SubmissionActionMessageDto> SubmissionAbandonedAsync(
        Guid issuerId,
        Guid submissionId,
        bool isTerminal,
        CancellationToken cancellationToken)
    {
        ISubmissionWorkflow workflow = await _service.GetSubmissionWorkflowAsync(submissionId, cancellationToken);
        return await workflow.SubmissionAbandonedAsync(issuerId, submissionId, isTerminal, cancellationToken);
    }

    public async Task<SubmissionActionMessageDto> SubmissionNotAcceptedAsync(
        Guid issuerId,
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        ISubmissionWorkflow workflow = await _service.GetSubmissionWorkflowAsync(submissionId, cancellationToken);
        return await workflow.SubmissionNotAcceptedAsync(issuerId, submissionId, cancellationToken);
    }
}