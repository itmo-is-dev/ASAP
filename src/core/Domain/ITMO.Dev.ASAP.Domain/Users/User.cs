using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Domain.UserAssociations;
using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Domain.Users;

public partial class User : IEntity<Guid>
{
    private readonly HashSet<UserAssociation> _associations;

    public User(
        Guid id,
        string firstName,
        string middleName,
        string lastName,
        HashSet<UserAssociation> associations) : this(id)
    {
        ArgumentNullException.ThrowIfNull(firstName);
        ArgumentNullException.ThrowIfNull(middleName);
        ArgumentNullException.ThrowIfNull(lastName);

        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;

        _associations = associations;
    }

    public User(
        Guid id,
        string firstName,
        string middleName,
        string lastName)
        : this(id, firstName, middleName, lastName, new HashSet<UserAssociation>()) { }

    public string FirstName { get; set; }

    public string MiddleName { get; set; }

    public string LastName { get; set; }

    public IReadOnlyCollection<UserAssociation> Associations => _associations;

    public override string ToString()
    {
        return $"{FirstName} {MiddleName} {LastName}";
    }

    public void AddAssociation(UserAssociation association)
    {
        ArgumentNullException.ThrowIfNull(association);

        Type associationType = association.GetType();

        if (Associations.Any(a => a.GetType() == associationType))
            throw new DomainInvalidOperationException($"User {this} already has {associationType} association");

        _associations.Add(association);
    }

    public T? FindAssociation<T>() where T : UserAssociation
    {
        return Associations.OfType<T>().SingleOrDefault();
    }
}