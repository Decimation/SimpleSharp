using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// ReSharper disable UnusedMember.Global

namespace SimpleSharp.Extensions
{
	/// <summary>
	/// Extensions for <see cref="string"/>s
	/// </summary>
	public static class StringExt
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string AsSingleString(this IEnumerable<string> strings) 
			=> strings.QuickJoin(Environment.NewLine);

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
	}
}