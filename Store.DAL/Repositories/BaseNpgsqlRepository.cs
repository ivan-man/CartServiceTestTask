using Common.Extensions;
using Dapper;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Store.DAL
{
    public abstract class BaseNpgsqlRepository : IBaseRepository
    {
        protected readonly string ConnectionString;

        protected readonly int SqlTimeout = 3600;

        protected abstract string SchemaName { get; }


        protected static readonly string n = Environment.NewLine;

        protected BaseNpgsqlRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected static string BuildLimitClause(int? page, int? pageSize)
        {
            return page > 0 && pageSize > 0
                ? $@" LIMIT {pageSize} OFFSET {(page - 1) * pageSize}"
                : string.Empty;
        }

        protected static string BuildOrderingClause<T>(string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
            {
                return string.Empty;
            }

            var orderParams = orderByQueryString.Trim().Split(',');

            for (var i = 0; i < orderParams.Length; i++)
            {
                var isDesc = orderParams[i].EndsWith(" desc");

                var columnName = Regex.Replace(orderParams[i], "asc|desc| ", string.Empty);

                orderParams[i] = isDesc
                    ? _ = $"{GetColumnName<T>(columnName)} desc"
                    : GetColumnName<T>(columnName);
            }

            return $"ORDER BY {string.Join(", ", orderParams)}";
        }

        protected static string GetColumnName<TEntity>(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException($"{nameof(propertyName)} is null or empty.");
            }

            var type = typeof(TEntity);

            var pi = type.GetProperties()
                .FirstOrDefault(q => q.Name.ToLower().Equals(propertyName.ToLower()));

            if (pi == null)
            {
                throw new ArgumentException($@"Invalid name of propery - {propertyName}. Type ""{type.Name}"" does not contain such properties.");
            }

            return pi.Name.ToSnakeCaseFromCamel();
        }

        protected static Task StartTransaction(NpgsqlConnection connection, TransactionIsolationLevel isolationLevel = TransactionIsolationLevel.ReadCommitted)
        {
            string isolationLevelName;
            switch (isolationLevel)
            {
                case TransactionIsolationLevel.Serializable:
                    isolationLevelName = "SERIALIZABLE";
                    break;
                case TransactionIsolationLevel.RepeatableRead:
                    isolationLevelName = "REPEATABLE READ";
                    break;
                case TransactionIsolationLevel.ReadUncommitted:
                    isolationLevelName = "READ UNCOMMITTED";
                    break;
                default:
                    isolationLevelName = "READ COMMITTED";
                    break;
            }
            return Exec(connection, $"START TRANSACTION ISOLATION LEVEL {isolationLevelName}");
        }

        protected static Task Commit(NpgsqlConnection connection)
        {
            return Exec(connection, "COMMIT");
        }

        protected static Task Rollback(NpgsqlConnection connection)
        {
            using var command = connection.CreateCommand();
            command.Connection = connection;
            command.CommandText = "ROLLBACK;";
            return command.ExecuteNonQueryAsync();
        }

        protected static Task Exec(NpgsqlConnection connection, string cmd)
        {
            using var command = connection.CreateCommand();
            command.Connection = connection;
            command.CommandText = cmd;

            return command.ExecuteNonQueryAsync();
        }
    }
}
