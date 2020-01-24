using System.Collections.Generic;

namespace ProtoSqlGen
{
	public interface IProtoFile : IProtoSerializable
	{
		string Package { get; }
		List<IProtoMessage> Messages { get; }

		string GetProto();
	}
}
