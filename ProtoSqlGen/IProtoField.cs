using System.Collections.Generic;

namespace ProtoSqlGen
{
	public interface IProtoField
	{
		string Type { get; }
		string Name { get; }
		bool IsRepeated { get; }

		IEnumerable<string> GetProtoLines(int fieldNumber);
	}
}
