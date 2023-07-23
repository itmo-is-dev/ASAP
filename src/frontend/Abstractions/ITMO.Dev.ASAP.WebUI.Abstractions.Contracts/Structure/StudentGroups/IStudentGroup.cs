using ITMO.Dev.ASAP.Application.Dto.Users;

namespace ITMO.Dev.ASAP.WebUI.Abstractions.Contracts.Structure.StudentGroups;

public interface IStudentGroup
{
    Guid Id { get; }

    IObservable<string> Name { get; }

    IObservable<IEnumerable<StudentDto>> Students { get; }

    IObservable<bool> IsVisible { get; }

    ValueTask UpdateNameAsync(string name, CancellationToken cancellationToken);
}