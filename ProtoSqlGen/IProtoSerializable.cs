using System.Collections.Generic;

namespace ProtoSqlGen
{
	public interface IProtoSerializable
	{
		IEnumerable<string> GetProtoLines();
	}
}
