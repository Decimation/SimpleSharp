using System;
using System.Collections;

// ReSharper disable SwitchStatementMissingSomeCases

// ReSharper disable UnusedMember.Global

namespace SimpleSharp.Utilities
{
	/// <summary>
	/// Extensions for <see cref="Type"/>s
	/// </summary>
	public static class TypeUtil
	{
		#region Extensions

		public static bool HasInterface(this Type type, string interfaceType)
		{
			return type.GetInterface(interfaceType) != null;
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

		#endregion
	}
}