#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleSharp.Extensions;
using SimpleSharp.Strings;

#endregion

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace SimpleSharp
{
	/// <summary>
	/// Provides utilities to create <see cref="string"/> representations of collection types.
	/// </summary>
	public static partial class Collections
	{
		public delegate string ToString<in T>(T value);

		#region Join

		#region Quick

		public static string QuickJoin<T>(this IEnumerable<T> value, string delim)
			=> String.Join(delim, value);

		public static string QuickJoin<T>(this IEnumerable<T> value)
			=> String.Join(StringConstants.JOIN_COMMA, value);

		#endregion

		#region Auto

		public static string AutoJoin(this IEnumerable<byte> value, FormatOptions options = FormatOptions.HEX)
			=> CreateString(value, options);

		public static string AutoJoin(this IEnumerable value, FormatOptions options = FormatOptions.NONE)
			=> CreateString(value, options);

		public static string AutoJoin(this IEnumerable value, string delim,
		                              FormatOptions  options = FormatOptions.NONE)
			=> CreateString(value, delim, options);

		#endregion

		#region Specific

		/// <summary>
		/// Extended <see cref="String.Join{T}(string,IEnumerable{T})"/>
		/// </summary>
		/// <param name="seq">Sequence of elements</param>
		/// <param name="toString">Function to convert each element into a <see cref="string"/></param>
		/// <typeparam name="T">Type of element</typeparam>
		public static string SpecificJoin<T>(this IEnumerable<T> seq, ToString<T> toString)
			=> seq.SpecificJoin(StringConstants.JOIN_COMMA, toString);

		/// <summary>
		/// Extended <see cref="String.Join{T}(string,IEnumerable{T})"/>
		/// </summary>
		/// <param name="seq">Sequence of elements</param>
		/// <param name="delim">Delimiter</param>
		/// <param name="toString">Function to convert each element into a <see cref="string"/></param>
		/// <typeparam name="T">Type of element</typeparam>
		public static string SpecificJoin<T>(this IEnumerable<T> seq, string delim, ToString<T> toString)
		{
			var sb = new StringBuilder();

			foreach (var v in seq) {
				sb.Append(toString(v))
				  .Append(delim);
			}

			return sb.RemoveTrailingDelimeter(delim).ToString();
		}

		#endregion

		#region Format

		private static string FormatJoinInternal<T>(IEnumerable<T>  list,      string delim,
		                                            IFormatProvider formatter, string format)
		{
			format = CreateFormatString(format);

			Func<T, string> func;

			if (formatter == null) {
				func = arg => String.Format(format, arg);
			}
			else {
				func = arg => String.Format(formatter, format, arg);
			}

			return String.Join(delim, list.Select(func));
		}

		/// <summary>
		/// Extended <see cref="String.Join{T}(string,IEnumerable{T})"/>
		/// </summary>
		/// <param name="list">Sequence of elements</param>
		/// <param name="delim">Delimiter</param>
		/// <param name="format">Format specifier</param>
		/// <typeparam name="T">Type of element</typeparam>
		public static string FormatJoin<T>(this IEnumerable<T> list, string delim, string format)
			=> FormatJoinInternal(list, delim, null, format);

		/// <summary>
		/// Extended <see cref="String.Join{T}(string,IEnumerable{T})"/>
		/// </summary>
		/// <param name="list">Sequence of elements</param>
		/// <param name="delim">Delimiter</param>
		/// <param name="formatter">Format provider</param>
		/// <param name="format">Format specifier</param>
		/// <typeparam name="T">Type of element</typeparam>
		public static string FormatJoin<T>(this IEnumerable<T> list,      string delim,
		                                   IFormatProvider     formatter, string format)
			=> FormatJoinInternal(list, delim, formatter, format);

		#endregion

		#endregion


		#region CreateString

		private static string CreateStringElement(object value, FormatOptions options)
		{
			string s;
			if (options.HasFlagFast(FormatOptions.HEX)) {
				s = Hex.TryCreateHex(value, options);
			}
			else {
				s = value.ToString();
			}

			return s;
		}

		public static string CreateString(IEnumerable value, FormatOptions options)
			=> CreateString(value, StringConstants.JOIN_COMMA, options);

		// todo: WIP
		public static string CreateString(IEnumerable value, string delim, FormatOptions options)
		{
			// Ignore strings
			if (value is string s) {
				return s;
			}
			
			var sb = new StringBuilder();

			foreach (var element in value) {
				if (element.GetType().IsEnumerableType()) {
					var    elemObj = (IEnumerable) element;
					string elemStr = CreateString(elemObj, delim, options);

					sb.Append('[')
					  .Append(elemStr)
					  .Append(']');
				}
				else {
					sb.Append(CreateStringElement(element, options));
				}

				sb.Append(delim);
			}

			return sb.RemoveTrailingDelimeter(delim).ToString();
		}

		#endregion


		private static string CreateFormatString(string formatString)
		{
			const string FMT = "{0}";
			return String.IsNullOrWhiteSpace(formatString) ? FMT : formatString;
		}
	}
}