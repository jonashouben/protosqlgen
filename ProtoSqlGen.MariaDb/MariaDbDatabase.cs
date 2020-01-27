using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ProtoSqlGen.MariaDb
{
	public class MariaDbDatabase : Database
	{
		public MariaDbDatabase(MySqlConnection connection) : base(connection)
		{
		}

		public override async Task<List<string>> GetDatabaseNames(CancellationToken cancellationToken)
		{
			return await GetStrings("SHOW DATABASES;", null, cancellationToken).Where(row => row != "information_schema" && row != "mysql" && row != "performance_schema").ToListAsync().ConfigureAwait(false);
		}

		public override async Task<List<string>> GetTableNames(string database, CancellationToken cancellationToken)
		{
			return await GetStrings($"SHOW TABLES FROM `{database}`;", Enumerable.Empty<DbParameter>(), cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);
		}

		public override async IAsyncEnumerable<IProtoField> GetTableFields(string database, string table, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			using (DbCommand cmd = CreateCommand())
			{
				cmd.CommandText = $"SHOW COLUMNS FROM `{table}` FROM `{database}`;";

				await EnsureConnected(cancellationToken).ConfigureAwait(false);

				await using (DbDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
				{
					while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
					{
						string field = reader.GetString(reader.GetOrdinal("Field"));
						string type = reader.GetString(reader.GetOrdinal("Type")).ToUpperInvariant();

						if (type == "BIT" ||
							type == "TINYINT(1)")
						{
							yield return new ProtoField(ProtoFieldType.Bool, field);
						}
						else if (
							type.StartsWith("TINYINT", StringComparison.Ordinal) ||
							type.StartsWith("SMALLINT", StringComparison.Ordinal) ||
							type.StartsWith("MEDIUMINT", StringComparison.Ordinal) ||
							type.StartsWith("INT", StringComparison.Ordinal))
						{
							yield return new ProtoField(ProtoFieldType.SInt32, field);
						}
						else if (type.StartsWith("BIGINT", StringComparison.Ordinal))
						{
							yield return new ProtoField(ProtoFieldType.SInt64, field);
						}
						else if (
							type.StartsWith("DECIMAL", StringComparison.Ordinal) ||
							type.StartsWith("FLOAT", StringComparison.Ordinal))
						{
							yield return new ProtoField(ProtoFieldType.Float, field);
						}
						else if (type.StartsWith("DOUBLE", StringComparison.Ordinal))
						{
							yield return new ProtoField(ProtoFieldType.Double, field);
						}
						else if (type.StartsWith("CHAR", StringComparison.Ordinal) ||
							type.StartsWith("VARCHAR", StringComparison.Ordinal) ||
							type.StartsWith("TINYTEXT", StringComparison.Ordinal) ||
							type.StartsWith("TEXT", StringComparison.Ordinal) ||
							type.StartsWith("MEDIUMTEXT", StringComparison.Ordinal) ||
							type.StartsWith("LONGTEXT", StringComparison.Ordinal))
						{
							yield return new ProtoField(ProtoFieldType.String, field);
						}
					}
				}
			}
		}
	}
}
