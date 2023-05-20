using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;

namespace ITMO.Dev.ASAP.Github.DataAccess.Migrations;

public abstract class SqlMigration : IMigration
{
    public void GetUpExpressions(IMigrationContext context)
    {
        context.Expressions.Add(new ExecuteSqlStatementExpression
        {
            SqlStatement = GetUpSql(context.ServiceProvider),
        });
    }

    public void GetDownExpressions(IMigrationContext context)
    {
        context.Expressions.Add(new ExecuteSqlStatementExpression
        {
            SqlStatement = GetDownSql(context.ServiceProvider),
        });
    }

    protected abstract string GetUpSql(IServiceProvider services);

    protected abstract string GetDownSql(IServiceProvider services);

    public object ApplicationContext => throw new NotSupportedException();

    public string ConnectionString => throw new NotSupportedException();
}