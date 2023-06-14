using ITMO.Dev.ASAP.Application.DataAccess.Repositories;
using ITMO.Dev.ASAP.DataAccess.Contexts;
using ITMO.Dev.ASAP.DataAccess.Mapping;
using ITMO.Dev.ASAP.DataAccess.Models.UserAssociations;
using ITMO.Dev.ASAP.Domain.UserAssociations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ITMO.Dev.ASAP.DataAccess.Repositories;

public class UserAssociationRepository :
    RepositoryBase<UserAssociation, UserAssociationModel>,
    IUserAssociationRepository
{
    private readonly DatabaseContext _context;
    private readonly ILogger<UserAssociationRepository> _logger;

    public UserAssociationRepository(DatabaseContext context, ILogger<UserAssociationRepository> logger) : base(context)
    {
        _context = context;
        _logger = logger;
    }

    protected override DbSet<UserAssociationModel> DbSet => _context.UserAssociations;

    protected override UserAssociationModel MapFrom(UserAssociation entity)
    {
        return UserAssociationsMapper.MapFrom(entity);
    }

    protected override bool Equal(UserAssociation entity, UserAssociationModel model)
    {
        return entity.Id.Equals(model.Id);
    }

    protected override void UpdateModel(UserAssociationModel model, UserAssociation entity)
    {
        if ((entity, model) is (IsuUserAssociation association, IsuUserAssociationModel associationModel))
        {
            associationModel.UniversityId = association.UniversityId;
        }
        else
        {
            _logger.LogWarning(
                "Attempt to update incompatible user association types: Model = {ModelType}, Entity = {EntityType}",
                model.GetType(),
                entity.GetType());
        }
    }
}