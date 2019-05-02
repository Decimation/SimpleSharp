using System;
using System.Collections;

// ReSharper disable SwitchStatementMissingSomeCases

// ReSharper disable UnusedMember.Global

namespace SimpleSharp.Extensions
{
	/// <summary>
	/// Extensions for <see cref="Type"/>s
	/// </summary>
	public static class TypeExt
	{
		public static bool HasInterface(this Type type, string interfaceType)
		{
			return type.GetInterface(interfaceType) != null;
		}

		public static bool IsIntegerType(this Type type)
		{
			switch (Type.GetTypeCode(type)) {
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.UInt16:
				case TypeCode.Int16:
				case TypeCode.UInt32:
				case TypeCode.Int32:
				case TypeCode.UInt64:
				case TypeCode.Int64:
					return true;
				default:
					return false;
			}
		}

		public static bool IsFloatType(this Type type)
		{
			switch (Type.GetTypeCode(type)) {
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					return true;
				default:
					return false;
			}
		}
		
		public static bool IsNumericType(this Type type)
		{
			return type.IsIntegerType() || type.IsFloatType();
		}

		public static bool IsIListType(this Type type)
		{
			return type.HasInterface(nameof(IList));
		}

		public static bool IsEnumerableType(this Type type)
		{
			return type.HasInterface(nameof(IEnumerable));
		}

		public static bool ContainsAnyGenericParameters(this Type type)
		{
			// There is probably a method that already does this but Microsoft's naming
			// system makes no fucking sense lmao
			return type.GenericTypeArguments.Length != 0;
		}
	}
}