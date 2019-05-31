using System;
using System.Runtime.CompilerServices;
using SimpleSharp.Extensions;

// ReSharper disable UnusedMember.Global

namespace SimpleSharp.Strings
{
	/// <summary>
	///     <para>Creates hex representations of pointers</para>
	/// </summary>
	public static unsafe class Hex
	{
		public const FormatOptions DEFAULT = FormatOptions.HEX | FormatOptions.PREFIX_HEX;

		#region Methods

		public static string ToHex(IntPtr p, FormatOptions options = DEFAULT)
			=> ToHexInternal(p.ToInt64(), options);

		public static string ToHex(long l, FormatOptions options = DEFAULT)
			=> ToHexInternal(l, options);

		public static string ToHex(ulong l, FormatOptions options = DEFAULT)
			=> ToHexInternal((long) l, options);

		public static string ToHex(void* v, FormatOptions options = DEFAULT)
			=> ToHexInternal((long) v, options);

		#endregion

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void CheckOptions(FormatOptions options)
		{
			if (!options.HasFlagFast(FormatOptions.HEX)) {
				string msg = String.Format("\"{0}\" must have the \"{1}\" flag set",
				                           nameof(options), nameof(FormatOptions.HEX));

				throw new ArgumentException(msg, nameof(options));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string ToHexInternal(long l, FormatOptions options = DEFAULT)
		{
			CheckOptions(options);

			string s = l.ToString("X");

			if (options.HasFlagFast(FormatOptions.ZERO_PAD_HEX) && s.Length == 1) {
				s = StringConstants.ZERO + s;
			}

			if (options.HasFlagFast(FormatOptions.PREFIX_HEX)) {
				s = StringConstants.HEX_PREFIX + s;
			}

			return s;
		}

		public static bool TryCreateHex<T>(T value, out string s, FormatOptions options = DEFAULT)
		{
			string str = String.Empty;
			if (Int64.TryParse(value.ToString(), out long l)) {
				str = ToHexInternal(l, options);
			}

			s = str;
			return !String.IsNullOrWhiteSpace(str);
		}

		public static string TryCreateHex<T>(T value, FormatOptions options = DEFAULT)
		{
			return !TryCreateHex(value, out string str, options) ? value.ToString() : str;
		}
	}
}