using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ERP.Library.Extensions
{
    public static class StoredProcedureExtension
    {
        /// <summary>
        /// 執行 Stored Procedure 並回傳查詢結果。
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="database"></param>
        /// <param name="storedProcedureName"></param>
        /// <param name="parameters"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="commandTimeout">可選的命令逾時設定。</param>
        /// <returns></returns>
        public static Task<List<TResult>> StoredProcedureQueryAsync<TResult>(
            this DatabaseFacade database,
            string storedProcedureName,
            object? parameters = null,
            CancellationToken cancellationToken = default,
            int? commandTimeout = null)
        {
            ArgumentNullException.ThrowIfNull(database);

            if (string.IsNullOrWhiteSpace(storedProcedureName))
            {
                throw new ArgumentException("Stored Procedure 名稱不可為空白。", nameof(storedProcedureName));
            }

            var (sql, args) = BuildStoredProcedureSql(storedProcedureName, parameters);

            if (args == null)
            {
                throw new ArgumentException("args 不得為空。", nameof(storedProcedureName));
            }

            if (commandTimeout.HasValue)
            {
                database.SetCommandTimeout(commandTimeout.Value);
            }
            else
            {
                database.SetCommandTimeout(3600);
            }

            return database.SqlQueryRaw<TResult>(sql, args!).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 執行 Stored Procedure 並回傳單筆結果，若無資料則回傳該型別預設值。
        /// </summary>
        /// <typeparam name="TResult">回傳型別，例如 string、int、bool 等。</typeparam>
        /// <param name="database"></param>
        /// <param name="storedProcedureName"></param>
        /// <param name="parameters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<TResult?> StoredProcedureAsync<TResult>(
            this DatabaseFacade database,
            string storedProcedureName,
            object? parameters = null,
            CancellationToken cancellationToken = default)
        {
            var results = await database.StoredProcedureQueryAsync<TResult>(
                storedProcedureName, parameters, cancellationToken);

            return results.FirstOrDefault();
        }

        private static (string Sql, object?[] Args) BuildStoredProcedureSql(string storedProcedureName, object? parameters)
        {
            if (parameters == null)
            {
                return ($"EXEC {storedProcedureName}", []);
            }

            var properties = parameters.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.CanRead)
                .ToArray();

            if (properties.Length == 0)
            {
                return ($"EXEC {storedProcedureName}", []);
            }

            var args = new object?[properties.Length];
            var parameterSql = new string[properties.Length];

            for (var i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                args[i] = property.GetValue(parameters) ?? DBNull.Value;
                parameterSql[i] = $"@{property.Name} = {{{i}}}";
            }

            return ($"EXEC {storedProcedureName} {string.Join(", ", parameterSql)}", args);
        }
    }
}
