using System.Collections.Generic;
using System.Linq;

namespace ProtoSqlGen
{
	public class ProtoFile : IProtoFile
	{
		public string Package { get; }
		public List<IProtoMessage> Messages { get; }

		public ProtoFile(string package = null)
		{
			Package = package;
			Messages = new List<IProtoMessage>();
		}

		public string GetProto()
		{
			return string.Join("", GetProtoLines());
		}

		public IEnumerable<string> GetProtoLines()
		{
			if (Package != null)
			{
				yield return "package " + Package + ";";
			}

			foreach (string line in Messages.SelectMany(row => row.GetProtoLines()))
			{
				yield return line;
			}
		}
	}
}
