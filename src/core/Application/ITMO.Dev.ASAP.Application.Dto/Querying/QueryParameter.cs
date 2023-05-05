namespace ITMO.Dev.ASAP.Application.Dto.Querying;

public class QueryParameter<T>
{
    public QueryParameter(T type, string pattern)
    {
        Type = type;
        Pattern = pattern;
    }

    public T Type { get; set; }

    public string Pattern { get; set; }
}