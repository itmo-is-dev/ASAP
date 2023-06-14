using Bogus;
using ITMO.Dev.ASAP.DataAccess.Models.UserAssociations;
using ITMO.Dev.ASAP.DataAccess.Models.Users;
using ITMO.Dev.ASAP.Seeding.Extensions;
using ITMO.Dev.ASAP.Seeding.Options;

namespace ITMO.Dev.ASAP.Seeding.EntityGenerators;

public class UserGenerator : EntityGeneratorBase<UserModel>
{
    private readonly Faker _faker;

    public UserGenerator(EntityGeneratorOptions<UserModel> options, Faker faker)
        : base(options)
    {
        _faker = faker;
    }

    protected override UserModel Generate(int index)
    {
        return new UserModel(
            _faker.Random.Guid(),
            _faker.Name.FirstName(),
            _faker.Name.MiddleName(),
            _faker.Name.LastName(),
            new List<UserAssociationModel>());
    }
}