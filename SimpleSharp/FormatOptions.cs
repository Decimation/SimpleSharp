using System;
using SimpleSharp.Strings;

namespace SimpleSharp
{
	// todo: this is really just HexOptions
	
	[Flags]
	public enum FormatOptions
	{
		NONE = 1,

		/// <summary>
		///     Represent the number in hexadecimal format
		/// </summary>
		HEX = 2,


		/// <summary>
		///     Pad single-digit hex with <see cref="StringConstants.ZERO"/>
		/// </summary>
		ZERO_PAD_HEX = HEX | 4,


		/// <summary>
		///     Prefix hex with <see cref="StringConstants.HEX_PREFIX"/>
		/// </summary>
		PREFIX_HEX = HEX | 8,
		
		// Decimal = 16
	}
}