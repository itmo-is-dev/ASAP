using ITMO.Dev.ASAP.Domain.Study;
using ITMO.Dev.ASAP.Domain.Submissions;

namespace ITMO.Dev.ASAP.Domain.Queue.Filters;

public class GroupQueueFilter : IQueueFilter
{
    public GroupQueueFilter(IReadOnlyCollection<StudentGroup> groups)
    {
        Groups = groups;
    }

    public IReadOnlyCollection<StudentGroup> Groups { get; }

    public IQueryable<Submission> Filter(IQueryable<Submission> query)
    {
        return query.Where(s => Groups.Contains(s.Student.Group));
    }
}