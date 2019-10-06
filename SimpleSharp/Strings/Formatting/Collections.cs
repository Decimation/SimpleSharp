#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleSharp.Utilities;

#endregion

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace SimpleSharp.Strings.Formatting
{
	/// <summary>
	/// Provides utilities to create <see cref="string"/> representations of collection types.
	/// </summary>
	public static class Collections
	{
		public delegate string ToString<in T>(T value);

		#region Join

		#region Simple

		public static string SimpleJoin<T>(this IEnumerable<T> value, string delim) 
			=> String.Join(delim, value);

		public static string SimpleJoin<T>(this IEnumerable<T> value) 
			=> value.SimpleJoin(StringConstants.JOIN_COMMA);

		#endregion

		#region Quick

		public static string QuickJoin<T>(this IEnumerable<T> value, string delim)
			=> String.Join(delim, value);

		public static string QuickJoin<T>(this IEnumerable<T> value)
			=> String.Join(StringConstants.JOIN_COMMA, value);

		#endregion

		#region Auto

		/// <summary>
		/// Creates a <see cref="string"/> representation of an <see cref="IEnumerable{T}"/>.
		/// <remarks>
		/// <seealso cref="string.Join{T}"/>
		/// </remarks>
		/// </summary>
		/// <param name="value">Collection</param>
		/// <param name="options"><see cref="HexOptions"/> if the <see cref="IEnumerable{T}"/> is a collection of numbers.</param>
		public static string AutoJoin(this IEnumerable<byte> value, HexOptions options = HexOptions.HEX)
			=> CreateString(value, options);

		/// <summary>
		/// Creates a <see cref="string"/> representation of an <see cref="IEnumerable{T}"/>.
		/// <remarks>
		/// <seealso cref="string.Join{T}"/>
		/// </remarks>
		/// </summary>
		/// <param name="value">Collection</param>
		/// <param name="options"><see cref="HexOptions"/> if the <see cref="IEnumerable{T}"/> is a collection of numbers.</param>
		public static string AutoJoin(this IEnumerable value, HexOptions options = HexOptions.NONE)
			=> CreateString(value, options);

		/// <summary>
		/// Creates a <see cref="string"/> representation of an <see cref="IEnumerable{T}"/>.
		/// <remarks>
		/// <seealso cref="string.Join{T}"/>
		/// </remarks>
		/// </summary>
		/// <param name="value">Collection</param>
		/// <param name="delim">Delimiter</param>
		/// <param name="options"><see cref="HexOptions"/> if the <see cref="IEnumerable{T}"/> is a collection of numbers.</param>
		public static string AutoJoin(this IEnumerable value, string delim,
		                              HexOptions       options = HexOptions.NONE)
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
			format = Formatting.CreateFormatString(format);

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

		public static string FormatJoin<T>(this IEnumerable<T> list, string format)
			=> FormatJoinInternal(list, StringConstants.JOIN_COMMA, null, format);

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

		public static string FormatJoin<T>(this IEnumerable<T> list, IFormatProvider formatter, string format)
			=> FormatJoinInternal(list, StringConstants.JOIN_COMMA, formatter, format);

		#endregion

		#endregion


		#region CreateString

		private static string CreateStringElement(object value, HexOptions options)
		{
			return options.HasFlagFast(HexOptions.HEX) ? Hex.TryCreateHex(value, options) : value.ToString();
		}

		private static string CreateString(IEnumerable value, HexOptions options)
			=> CreateString(value, StringConstants.JOIN_COMMA, options);

		// todo: WIP
		private static string CreateString(IEnumerable value, string delim, HexOptions options)
		{
			// Ignore strings
			if (value is string s) {
				return s;
			}

			var sb = new StringBuilder();

			foreach (var element in value) {
				var elemType = element.GetType();
				if (elemType.IsEnumerableType()) {
					var    elemObj = (IEnumerable) element;
					string elemStr = CreateString(elemObj, delim, options);

					if (elemType != typeof(string)) {
						sb.Append('[')
						  .Append(elemStr)
						  .Append(']');
					}
					else {
						sb.Append(elemStr);
					}
				}
				else {
					sb.Append(CreateStringElement(element, options));
				}

				sb.Append(delim);
			}

			return sb.RemoveTrailingDelimeter(delim).ToString();
		}

		#endregion
	}
}