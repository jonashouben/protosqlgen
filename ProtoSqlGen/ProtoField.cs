using System.Collections.Generic;

namespace ProtoSqlGen
{
	public class ProtoField : IProtoField
	{
		public string Type { get; }
		public string Name { get; }
		public bool IsRepeated { get; }

		public IEnumerable<string> GetProtoLines(int fieldNumber)
		{
			yield return (IsRepeated ? "repeated " : "") + Type + " " + Name + " = " + fieldNumber + ";";
		}
	}
}
