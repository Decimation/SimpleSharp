using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using SimpleSharp.Extensions;

// ReSharper disable UnusedMember.Global

namespace SimpleSharp.Strings.Formatting
{
	/// <summary>
	///     <para>Creates hex representations of pointers</para>
	/// </summary>
	public static unsafe class Hex
	{
		public const HexOptions DEFAULT = HexOptions.HEX | HexOptions.PREFIX_HEX;

		#region Methods

		public static string ToHex(IntPtr p, HexOptions options = DEFAULT)
			=> ToHexInternal(p.ToInt64(), options);

		public static string ToHex(long l, HexOptions options = DEFAULT)
			=> ToHexInternal(l, options);

		public static string ToHex(ulong l, HexOptions options = DEFAULT)
			=> ToHexInternal((long) l, options);

		public static string ToHex(void* v, HexOptions options = DEFAULT)
			=> ToHexInternal((long) v, options);

		#endregion

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void CheckOptions(HexOptions options)
		{
			if (!options.HasFlagFast(HexOptions.HEX)) {
				string msg = String.Format("\"{0}\" must have the \"{1}\" flag set",
				                           nameof(options), nameof(HexOptions.HEX));

				throw new ArgumentException(msg, nameof(options));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string ToHexInternal(long l, HexOptions options = DEFAULT)
		{
			CheckOptions(options);

			string s = l.ToString("X");

			if (options.HasFlagFast(HexOptions.ZERO_PAD_HEX) && s.Length == 1) {
				s = StringConstants.ZERO + s;
			}

			if (options.HasFlagFast(HexOptions.PREFIX_HEX)) {
				s = StringConstants.HEX_PREFIX + s;
			}

			return s;
		}

		public static bool TryCreateHex<T>(T value, out string s, HexOptions options = DEFAULT)
		{
			string str = String.Empty;

			if (value is Pointer) {
				var ptr = Pointer.Unbox(value);
				s = ToHex(ptr);
				return true;
			}

			if (Int64.TryParse(value.ToString(), out long l)) {
				str = ToHexInternal(l, options);
			}

			s = str;
			return !String.IsNullOrWhiteSpace(str);
		}

		public static string TryCreateHex<T>(T value, HexOptions options = DEFAULT)
		{
			return !TryCreateHex(value, out string str, options) ? value.ToString() : str;
		}
	}
}