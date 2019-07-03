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
	public static class StringUtil
	{
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
	}
}