using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ProtoSqlGen
{
	public abstract class Database : IAsyncDisposable
	{
		private readonly DbConnection _connection;

		protected Database(DbConnection connection)
		{
			_connection = connection;
		}

		public DbCommand CreateCommand()
		{
			return _connection.CreateCommand();
		}

		public async IAsyncEnumerable<IProtoFile> GetDatabases([EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			await foreach (string database in GetDatabaseNames(cancellationToken))
			{
				yield return new ProtoFile(database, await GetTables(database, cancellationToken).ToListAsync(cancellationToken));
			}
		}
		public abstract IAsyncEnumerable<string> GetDatabaseNames(CancellationToken cancellationToken = default);
		public async IAsyncEnumerable<IProtoMessage> GetTables(string database, [EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			await foreach (string table in GetTableNames(database, cancellationToken))
			{
				yield return await GetTable(database, table, cancellationToken).ConfigureAwait(false);
			}
		}
		public abstract IAsyncEnumerable<string> GetTableNames(string database, CancellationToken cancellationToken = default);
		public async Task<IProtoMessage> GetTable(string database, string table, CancellationToken cancellationToken = default)
		{
			return new ProtoMessage(table, await GetTableFields(database, table, cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false));
		}
		public abstract IAsyncEnumerable<IProtoField> GetTableFields(string database, string table, CancellationToken cancellationToken = default);

		protected async IAsyncEnumerable<string> GetStrings(string query, IEnumerable<DbParameter> parameters = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			using (DbCommand cmd = _connection.CreateCommand())
			{
				cmd.CommandText = query;

				if (parameters != null)
				{
					foreach (DbParameter parameter in parameters)
					{
						cmd.Parameters.Add(parameter);
					}
				}

				await EnsureConnected(cancellationToken).ConfigureAwait(false);

				await using (DbDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
				{
					while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
					{
						yield return reader.GetString(0);
					}
				}
			}
		}

		protected async IAsyncEnumerable<T> SingleItemAsyncEnumerable<T>(T item)
		{
			yield return await new ValueTask<T>(item).ConfigureAwait(false);
		}

		protected async ValueTask EnsureConnected(CancellationToken cancellationToken = default)
		{
			if (_connection.State == ConnectionState.Open)
			{
				return;
			}

			if (_connection.State != ConnectionState.Closed)
			{
				await _connection.CloseAsync().ConfigureAwait(false);
			}

			await _connection.OpenAsync(cancellationToken).ConfigureAwait(false);
		}

		public async ValueTask DisposeAsync()
		{
			await _connection.DisposeAsync().ConfigureAwait(false);
		}
	}
}
