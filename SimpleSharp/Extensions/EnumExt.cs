using System;
using System.Collections.Generic;
using System.Linq;
using SimpleSharp.Strings;

namespace SimpleSharp.Extensions
{
	/// <summary>
	/// Extensions for <c>enum</c>s
	/// </summary>
	public static class EnumExt
	{
		public static IEnumerable<Enum> GetFlags(this Enum value)
		{
			return Enum.GetValues(value.GetType())
			           .Cast<Enum>()
			           .Where(value.HasFlag)
			           .Distinct();
		}

		public static string JoinFlags(this Enum value, string delim = StringConstants.JOIN_COMMA)
			=> String.Join(delim, value.GetFlags());
	}
}