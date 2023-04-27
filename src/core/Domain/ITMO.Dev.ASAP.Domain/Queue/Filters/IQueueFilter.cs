using ITMO.Dev.ASAP.Domain.Submissions;

namespace ITMO.Dev.ASAP.Domain.Queue.Filters;

public interface IQueueFilter
{
    IQueryable<Submission> Filter(IQueryable<Submission> query);
}