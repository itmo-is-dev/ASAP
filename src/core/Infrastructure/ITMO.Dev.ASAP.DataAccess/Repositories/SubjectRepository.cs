using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.DataAccess.Context;
using ITMO.Dev.ASAP.DataAccess.Models;
using ITMO.Dev.ASAP.Domain.Study;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ITMO.Dev.ASAP.DataAccess.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly DatabaseContext _context;

    public SubjectRepository(DatabaseContext context)
    {
        _context = context;
    }

    public IAsyncEnumerable<Subject> QueryAsync(SubjectQuery query, CancellationToken cancellationToken)
    {
        IQueryable<SubjectModel> queryable = _context.SubjectsSet;

        if (query.Ids.Count is not 0)
        {
            queryable = queryable.Where(x => query.Ids.Contains(x.Id));
        }

        if (query.Names.Count is not 0)
        {
            queryable = queryable.Where(x => query.Names.Contains(x.Title));
        }

        return queryable.AsAsyncEnumerable().Select(MapTo);
    }

    public void Add(Subject subject)
    {
        SubjectModel model = MapFrom(subject);
        _context.SubjectsSet.Add(model);
    }

    public void AddRange(IEnumerable<Subject> subjects)
    {
        IEnumerable<SubjectModel> models = subjects.Select(MapFrom);
        _context.SubjectsSet.AddRange(models);
    }

    public void Update(Subject subject)
    {
        EntityEntry<SubjectModel> entry = GetEntry(subject);

        entry.Entity.Title = subject.Title;
        entry.State = EntityState.Modified;
    }

    public void Remove(Subject subject)
    {
        EntityEntry<SubjectModel> entry = GetEntry(subject);
        entry.State = entry.State is EntityState.Added ? EntityState.Detached : EntityState.Deleted;
    }

    private static Subject MapTo(SubjectModel model)
    {
        return new Subject(model.Id, model.Title);
    }

    private static SubjectModel MapFrom(Subject subject)
    {
        return new SubjectModel
        {
            Id = subject.Id,
            Title = subject.Title,
        };
    }

    private EntityEntry<SubjectModel> GetEntry(Subject subject)
    {
        SubjectModel? existing = _context.SubjectsSet.Local.FirstOrDefault(x => x.Id.Equals(subject.Id));

        return existing is not null
            ? _context.Entry(existing)
            : _context.SubjectsSet.Attach(MapFrom(subject));
    }
}