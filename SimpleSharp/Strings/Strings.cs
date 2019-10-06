#region

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using SimpleSharp.Utilities;

// ReSharper disable UnusedMember.Global

#endregion

namespace SimpleSharp.Strings
{
	/// <summary>
	/// Provides utilities for working with <see cref="string"/>s
	/// </summary>
	public static class Strings
	{
		#region StringBuilder extensions

		public static StringBuilder AppendLineFormat(this StringBuilder sb, string s, params object[] args)
			=> sb.AppendFormat(s, args).AppendLine();

		public static StringBuilder RemoveTrailingDelimeter(this StringBuilder sb, string delim)
		{
			if (sb.Length > 0) {
				sb.Remove(sb.Length - delim.Length, delim.Length);
			}

			return sb;
		}

		#endregion

		/// <summary>
		///     Converts the boolean <paramref name="b" /> to a <see cref="char" /> representation.
		/// </summary>
		/// <param name="b"><see cref="bool" /> value</param>
		/// <returns>
		///     <see cref="StringConstants.CHECK_MARK" /> if <paramref name="b" /> is <c>true</c>; <see cref="StringConstants.BALLOT_X" /> if
		///     <paramref name="b" />
		///     is <c>false</c>
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static char Prettify(this bool b)
		{
			return b ? StringConstants.CHECK_MARK : StringConstants.BALLOT_X;
		}

		public static string ToLiteralExpression(string value)
		{
			using (var writer = new StringWriter()) {
				using (var provider = CodeDomProvider.CreateProvider("CSharp")) {
					provider.GenerateCodeFromExpression(new CodePrimitiveExpression(value), writer, null);
					return writer.ToString();
				}
			}
		}

		public static string ToLiteral(string value)
		{
			var literal = new StringBuilder(value.Length + 2);
			literal.Append("\"");
			foreach (char c in value) {
				// @formatter:off
				switch (c) {
					case '\'': literal.Append(@"\'");  break;
					case '\"': literal.Append("\\\""); break;
					case '\\': literal.Append(@"\\");  break;
					case '\0': literal.Append(@"\0");  break;
					case '\a': literal.Append(@"\a");  break;
					case '\b': literal.Append(@"\b");  break;
					case '\f': literal.Append(@"\f");  break;
					case '\n': literal.Append(@"\n");  break;
					case '\r': literal.Append(@"\r");  break;
					case '\t': literal.Append(@"\t");  break;
					case '\v': literal.Append(@"\v");  break;
					default:
						// ASCII printable character
						if (c >= 0x20 && c <= 0x7E) {
							literal.Append(c);
						// As UTF16 escaped character
						} else {
							literal.Append(@"\u");
							literal.Append(((int) c).ToString("x4").ToUpper());
						}
						break;
				}
				// @formatter:on
			}

			literal.Append("\"");
			return literal.ToString();
		}

		public static byte[] ParseByteArray(string szPattern)
		{
//			List<byte> patternbytes = new List<byte>();
//			foreach (string szByte in szPattern.Split(' '))
//				patternbytes.Add(szByte == "?" ? (byte) 0x0 : Convert.ToByte(szByte, 16));
//			return patternbytes.ToArray();


			string[] strByteArr   = szPattern.Split(' ');
			var      patternBytes = new byte[strByteArr.Length];

			for (int i = 0; i < strByteArr.Length; i++) {
				patternBytes[i] = strByteArr[i] == "?"
					? (byte) 0x0
					: Byte.Parse(strByteArr[i], NumberStyles.HexNumber);
			}


			return patternBytes;
		}

		public static string Superscript(int i)
		{
			var sb = new StringBuilder();
			foreach (char v in i.ToString())
				switch (v) {
					case '1':
						sb.Append('\x0B9');
						break;
					case '2':
						sb.Append('\x0B2');
						break;
					case '3':
						sb.Append('\x0B3');
						break;
					default:
						sb.Append((char) ('\u2070' + (v - '0')));
						break;
				}

			return sb.ToString();
		}

		public static string Subscript(int i)
		{
			var sb = new StringBuilder();
			foreach (char v in i.ToString())
				sb.Append((char) ('\u2080' + (v - '0')));

			return sb.ToString();
		}

		public static string Random(int len)
		{
			var sb = new StringBuilder();
			for (int i = 0; i < len; i++)
				sb.Append((StringConstants.ALPHANUMERICS.ToCharArray()).Random());

			return sb.ToString();
		}

		/// <summary>
		///     Aligns subsequent newlines directly under the string before the newlines.
		/// </summary>
		public static string Align(string formatted, int length)
		{
			// Align newlines on the same column

			string[] nl = formatted.Split(new[] {Environment.NewLine}, StringSplitOptions.None);

			var postString = new StringBuilder();
			postString.Append(nl[0]);

			for (int i = 1; i < nl.Length; i++) {
				string pad = new string(' ', length);
				postString.AppendFormat("{0}{1}", Environment.NewLine + pad, nl[i]);
			}

			return postString.ToString();
		}

		public static string FindClosest(string str, IEnumerable<string> list)
		{
			return list.OrderBy(s => LevenshteinDistance.Compute(s, str)).First();
		}

		#region Extensions

		/// <summary>
		/// Replaces occurrences of <paramref name="oldValue"/> with <see cref="String.Empty"/>
		/// </summary>
		public static string Erase(this string s, string oldValue)
			=> s.Replace(oldValue, String.Empty);

		/// <summary>
		/// Simulates Java substring function
		/// </summary>
		public static string JSubstring(this string s, int beginIndex)
			=> s.Substring(beginIndex, s.Length);

		/// <summary>
		/// Simulates Java substring function
		/// </summary>
		public static string JSubstring(this string s, int beginIndex, int endIndex)
			=> s.Substring(beginIndex, endIndex - beginIndex);

		/// <summary>
		///     <returns>String value after [last] <paramref name="a"/></returns>
		/// </summary>
		public static string SubstringAfter(this string value, string a)
		{
			int posA = value.LastIndexOf(a, StringComparison.Ordinal);
			if (posA == -1)
				return String.Empty;

			int adjustedPosA = posA + a.Length;
			return adjustedPosA >= value.Length ? String.Empty : value.Substring(adjustedPosA);
		}

		/// <summary>
		///     <returns>String value after [first] <paramref name="a"/></returns>
		/// </summary>
		public static string SubstringBefore(this string value, string a)
		{
			int posA = value.IndexOf(a, StringComparison.Ordinal);
			return posA == -1 ? String.Empty : value.Substring(0, posA);
		}

		/// <summary>
		///     <returns>String value between [first] <paramref name="a"/> and [last] <paramref name="b"/></returns>
		/// </summary>
		public static string SubstringBetween(this string value, string a, string b)
		{
			int posA = value.IndexOf(a, StringComparison.Ordinal);
			int posB = value.LastIndexOf(b, StringComparison.Ordinal);

			if (posA == -1 || posB == -1)
				return String.Empty;


			int adjustedPosA = posA + a.Length;
			return adjustedPosA >= posB ? String.Empty : value.Substring(adjustedPosA, posB - adjustedPosA);
		}

		public static IEnumerable<int> AllIndexesOf(this string str, string searchStr)
		{
			int minIndex = str.IndexOf(searchStr, StringComparison.Ordinal);
			while (minIndex != -1) {
				yield return minIndex;
				minIndex = str.IndexOf(searchStr, minIndex + searchStr.Length, StringComparison.Ordinal);
			}
		}

		public static string Split(this string value, string s)
		{
			return value.Split(new[] {s}, StringSplitOptions.None)[0];
		}

		public static bool IsInterned(this string text)
		{
			return String.IsInterned(text) != null;
		}

		#endregion
	}
}