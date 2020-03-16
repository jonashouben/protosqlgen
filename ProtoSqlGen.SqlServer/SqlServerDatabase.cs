using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ProtoSqlGen.SqlServer
{
	public class SqlServerDatabase : Database
	{
		/// <inheritdoc />
		public SqlServerDatabase(SqlConnection connection) : base(connection)
		{
		}

		/// <inheritdoc />
		public override async Task<List<string>> GetDatabaseNames(CancellationToken cancellationToken = default)
		{
			return await GetStrings("SELECT name FROM sys.databases WHERE database_id > 4;", null, cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);
		}

		/// <inheritdoc />
		public override async Task<List<string>> GetTableNames(string database, CancellationToken cancellationToken = default)
		{
			return await GetStrings("USE [" + database + "]; SELECT name FROM sys.tables;", null, cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);
		}

		/// <inheritdoc />
		public override async IAsyncEnumerable<IProtoField> GetTableFields(string database, string table, [EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			using (DbCommand cmd = CreateCommand())
			{
				cmd.CommandText = "USE [" + database + "]; SELECT cols.name AS col, t.name AS type FROM sys.tables tbl INNER JOIN sys.columns cols ON cols.object_id = tbl.object_id INNER JOIN sys.types t ON t.system_type_id = cols.system_type_id WHERE tbl.name = '" + table + "' ORDER BY cols.column_id;";

				await EnsureConnected(cancellationToken).ConfigureAwait(false);

				await using (DbDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
				{
					while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
					{
						string field = reader.GetString(reader.GetOrdinal("col"));
						string type = reader.GetString(reader.GetOrdinal("type")).ToUpperInvariant();

						if (type == "BIGINT" ||
							type == "DECIMAL" ||
							type == "NUMERIC")
						{
							yield return new ProtoField(ProtoFieldType.SInt64, field);
						}
						else if (type == "BINARY" ||
								type == "VARBINARY" ||
								type == "IMAGE")
						{
							yield return new ProtoField(ProtoFieldType.Bytes, field);
						}
						else if (type == "BIT")
						{
							yield return new ProtoField(ProtoFieldType.Bool, field);
						}
						else if (type == "CHAR" ||
								type == "VARCHAR" ||
								type == "TEXT" ||
								type == "NCHAR" ||
								type == "NVARCHAR" ||
								type == "NTEXT" ||
								type == "DATE" ||
								type == "DATETIME" ||
								type == "DATETIME2" ||
								type == "DATETIMEOFFSET" ||
								type == "SMALLDATETIME" ||
								type == "TIME")
						{
							yield return new ProtoField(ProtoFieldType.String, field);
						}
						else if (type == "FLOAT" ||
								type == "SMALLMONEY" ||
								type == "REAL")
						{
							yield return new ProtoField(ProtoFieldType.Float, field);
						}
						else if (type == "INT" ||
								type == "SMALLINT" ||
								type == "TINYINT")
						{
							yield return new ProtoField(ProtoFieldType.SInt32, field);
						}
						else if (type == "MONEY")
						{
							yield return new ProtoField(ProtoFieldType.Double, field);
						}
					}
				}
			}
		}
	}
}
