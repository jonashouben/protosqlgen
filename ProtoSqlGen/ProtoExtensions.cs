using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ProtoSqlGen
{
	public static class ProtoExtensions
	{
		public static string ToProtoType(this ProtoFieldType type)
		{
			switch (type)
			{
				case ProtoFieldType.Double:
					return "double";
				case ProtoFieldType.Float:
					return "float";
				case ProtoFieldType.Int32:
					return "int32";
				case ProtoFieldType.Int64:
					return "int64";
				case ProtoFieldType.UInt32:
					return "uint32";
				case ProtoFieldType.UInt64:
					return "uint64";
				case ProtoFieldType.SInt32:
					return "sint32";
				case ProtoFieldType.SInt64:
					return "sint64";
				case ProtoFieldType.Fixed32:
					return "fixed32";
				case ProtoFieldType.Fixed64:
					return "fixed64";
				case ProtoFieldType.SFixed32:
					return "sfixed32";
				case ProtoFieldType.SFixed64:
					return "sfixed64";
				case ProtoFieldType.Bool:
					return "bool";
				case ProtoFieldType.String:
					return "string";
				case ProtoFieldType.Bytes:
					return "bytes";
				default:
					throw new ArgumentOutOfRangeException(nameof(type));
			}
		}
	}
}
