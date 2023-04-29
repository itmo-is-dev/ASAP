namespace ITMO.Dev.ASAP.Google.Application.DataAccess.Queries;

public class SubjectCourseQuery
{
    private SubjectCourseQuery(IReadOnlyCollection<Guid> ids, IReadOnlyCollection<string> spreadsheetIds)
    {
        Ids = ids;
        SpreadsheetIds = spreadsheetIds;
    }

    public IReadOnlyCollection<Guid> Ids { get; }

    public IReadOnlyCollection<string> SpreadsheetIds { get; }

    public static SubjectCourseQuery Build(Func<Builder, Builder> action)
    {
        return action.Invoke(new Builder()).Build();
    }

    public class Builder
    {
        private readonly List<Guid> _ids;
        private readonly List<string> _spreadsheetIds;

        public Builder()
        {
            _ids = new List<Guid>();
            _spreadsheetIds = new List<string>();
        }

        public Builder WithId(Guid id)
        {
            _ids.Add(id);
            return this;
        }

        public Builder WithIds(IEnumerable<Guid> ids)
        {
            _ids.AddRange(ids);
            return this;
        }

        public Builder WithSpreadsheetId(string spreadsheetId)
        {
            _spreadsheetIds.Add(spreadsheetId);
            return this;
        }

        public Builder WithSpreadsheetIds(IEnumerable<string> spreadsheetIds)
        {
            _spreadsheetIds.AddRange(spreadsheetIds);
            return this;
        }

        public SubjectCourseQuery Build()
        {
            return new SubjectCourseQuery(
                _ids.Distinct().ToArray(),
                _spreadsheetIds.Distinct().ToArray());
        }
    }
}