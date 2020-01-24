using System;
using System.Collections.Generic;
using System.Text;

namespace ProtoSqlGen
{
	public interface IProtoMessage : IProtoSerializable
	{
		string Name { get; }
		List<IProtoField> Fields { get; }
	}
}
