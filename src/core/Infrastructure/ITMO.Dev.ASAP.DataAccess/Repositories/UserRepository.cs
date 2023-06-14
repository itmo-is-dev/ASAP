using ITMO.Dev.ASAP.Application.DataAccess.Models;
using ITMO.Dev.ASAP.Application.DataAccess.Queries;
using ITMO.Dev.ASAP.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.DataAccess.Contexts;
using ITMO.Dev.ASAP.DataAccess.Mapping;
using ITMO.Dev.ASAP.DataAccess.Models.UserAssociations;
using ITMO.Dev.ASAP.DataAccess.Models.Users;
using ITMO.Dev.ASAP.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace ITMO.Dev.ASAP.DataAccess.Repositories;

public class UserRepository : RepositoryBase<User, UserModel>, IUserRepository
{
    private readonly DatabaseContext _context;

    public UserRepository(DatabaseContext context) : base(context)
    {
        _context = context;
    }

    protected override DbSet<UserModel> DbSet => _context.Users;

    public IAsyncEnumerable<User> QueryAsync(UserQuery query, CancellationToken cancellationToken)
    {
        IQueryable<UserModel> queryable = ApplyQuery(_context.Users, query);

        return queryable
            .Include(x => x.Associations)
            .AsAsyncEnumerable()
            .Select(UserMapper.MapTo);
    }

    public Task<long> CountAsync(UserQuery query, CancellationToken cancellationToken)
    {
        IQueryable<UserModel> queryable = ApplyQuery(_context.Users, query);
        return queryable.LongCountAsync(cancellationToken);
    }

    protected override UserModel MapFrom(User entity)
    {
        return UserMapper.MapFrom(entity);
    }

    protected override bool Equal(User entity, UserModel model)
    {
        return entity.Id.Equals(model.Id);
    }

    protected override void UpdateModel(UserModel model, User entity)
    {
        model.FirstName = entity.FirstName;
        model.MiddleName = entity.MiddleName;
        model.LastName = entity.LastName;
    }

    private IQueryable<UserModel> ApplyQuery(IQueryable<UserModel> queryable, UserQuery query)
    {
        if (query.Ids is not [])
        {
            queryable = queryable.Where(x => query.Ids.Contains(x.Id));
        }

        if (query.UniversityIdPattern is not null)
        {
            queryable = queryable
                .Select(x => new
                {
                    user = x,
                    association = x.Associations.OfType<IsuUserAssociationModel>().Single(),
                })
                .Where(x => EF.Functions.ILike(x.association.UniversityId.ToString(), query.UniversityIdPattern))
                .Select(x => x.user);
        }

        if (query.FullNamePattern is not null)
        {
            queryable = queryable.Where(user =>
                EF.Functions.ILike(user.FirstName, query.FullNamePattern)
                || EF.Functions.ILike(user.MiddleName, query.FullNamePattern)
                || EF.Functions.ILike(user.LastName, query.FullNamePattern));
        }

        if (query.UniversityIds is not [])
        {
            queryable = queryable
                .Select(x => new
                {
                    user = x,
                    association = x.Associations.OfType<IsuUserAssociationModel>().Single(),
                })
                .Where(x => query.UniversityIds.Contains(x.association.UniversityId))
                .Select(x => x.user);
        }

        if (query.OrderByLastName is not null)
        {
            queryable = query.OrderByLastName.Value switch
            {
                OrderDirection.Ascending => queryable.OrderBy(x => x.LastName),
                OrderDirection.Descending => queryable.OrderByDescending(x => x.LastName),
                _ => throw new ArgumentOutOfRangeException(nameof(query)),
            };
        }

        if (query.Cursor is not null)
        {
            queryable = queryable.Skip(query.Cursor.Value);
        }

        if (query.Limit is not null)
        {
            queryable = queryable.Take(query.Limit.Value);
        }

        return queryable;
    }
}