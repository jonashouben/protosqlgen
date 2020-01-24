using System;
using System.Collections.Generic;

namespace ProtoSqlGen
{
	public class ProtoMessage : IProtoMessage
	{
		public string Name { get; }
		public List<IProtoField> Fields { get; }

		public ProtoMessage(string name)
		{
			Name = name;
			Fields = new List<IProtoField>();
		}

		public IEnumerable<string> GetProtoLines()
		{
			yield return "message " + Name + " {";

			int fieldNumber = 1;
			foreach (IProtoField field in Fields)
			{
				//Skip reserved field numbers
				//https://developers.google.com/protocol-buffers/docs/proto3#assigning-field-numbers
				if (fieldNumber == 19000)
				{
					fieldNumber = 20000;
				}

				//Check for maximum field number
				//https://developers.google.com/protocol-buffers/docs/proto3#assigning-field-numbers
				if (fieldNumber > 536870911)
				{
					throw new OverflowException("Field Number exceeded maximum!");
				}

				foreach (string line in field.GetProtoLines(fieldNumber))
				{
					yield return "  " + line;
				}

				fieldNumber++;
			}

			yield return "}";
		}
	}
}
