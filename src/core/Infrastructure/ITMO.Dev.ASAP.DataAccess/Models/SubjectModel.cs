namespace ITMO.Dev.ASAP.DataAccess.Models;

public class SubjectModel
{
    public virtual Guid Id { get; set; }

    public virtual string Title { get; set; } = string.Empty;
}