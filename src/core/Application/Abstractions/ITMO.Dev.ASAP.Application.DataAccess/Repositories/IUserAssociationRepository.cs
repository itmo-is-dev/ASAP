using ITMO.Dev.ASAP.Domain.UserAssociations;

namespace ITMO.Dev.ASAP.Application.DataAccess.Repositories;

public interface IUserAssociationRepository
{
    void Add(UserAssociation entity);

    void Update(UserAssociation entity);

    void AddRange(IEnumerable<UserAssociation> entities);
}