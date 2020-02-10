using System.Collections.Generic;

namespace ProtoSqlGen
{
	public class ProtoField : IProtoField
	{
		public ProtoFieldType Type { get; }
		public string Name { get; }
		public bool IsRepeated { get; }

		public ProtoField(ProtoFieldType type, string name)
		{
			Type = type;
			Name = name;
			IsRepeated = false;
		}

		public IEnumerable<string> GetProtoLines(int fieldNumber)
		{
			yield return (IsRepeated ? "repeated " : "") + Type.ToProtoType() + " " + Name + " = " + fieldNumber + ";";
		}
	}
}
