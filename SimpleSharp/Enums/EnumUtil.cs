using System;
using System.Linq;
using System.Runtime.CompilerServices;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace SimpleSharp.Enums
{
	/// <summary>
	/// Provides utilities for working with <c>enum</c>s
	/// </summary>
	public static class EnumUtil
	{
		// todo: rewrite this util class


		private delegate TEnum GetValue<out TEnum>(int i);

		/*
		[Flags]
		private enum Flags
		{
			One   = 1,
			Two   = 2,
			Three = 4
		}

		private static bool HasFlagFast(this Flags v, Flags f)
		{
			return (v & f) == f;
		}
		*/


		public static bool Same<TInteger>(Type a, Type b)
		{
			var aValues = GetValues<TInteger>(a)
			             .OrderBy(x => x).ToArray();

			var bValues = GetValues<TInteger>(b)
			             .OrderBy(x => x).ToArray();

			var longest = aValues.Length > bValues.Length ? aValues : bValues;

			for (int i = 0; i < longest.Length; i++) {
				var av = aValues[i];
				var bv = bValues[i];
				if (!av.Equals(bv)) {
					return false;
				}
			}

			return true;
		}

		// todo: rewrite these using TypedReferences
		public static TInteger[] GetValues<TInteger>(Type enumType)
		{
			var values = (Array) Convert.ChangeType(Enum.GetValues(enumType), enumType.MakeArrayType());
			return GetValuesArray<TInteger, object>(values.Length, i => values.GetValue(i));
		}

		private static TInteger[] GetValuesArray<TInteger, TEnum>(int length, GetValue<TEnum> getValue)
		{
			var intValues = new TInteger[length];

			for (int i = 0; i < intValues.Length; i++) {
				var val = (TInteger) Convert.ChangeType(getValue(i), typeof(TInteger));
				intValues[i] = val;
			}

			return intValues;
		}

		public static TInteger[] GetValues<TEnum, TInteger>() where TEnum : Enum
		{
			TEnum[] values = ToArray<TEnum>();
			return GetValuesArray<TInteger, TEnum>(values.Length, i => values[i]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string CreateFlagsString(object num, Enum e)
		{
			string join = e.JoinFlags();
			return join == String.Empty ? $"{num}" : $"{num} ({e.JoinFlags()})";
		}

		public static string CreateString<T>(T e) where T : Enum
		{
			var    asNum    = AsNumeric(e);
			string join     = e.ToString();
			string asNumStr = String.Format("({0})", asNum);

			if (asNum.ToString() == join)
				return asNumStr;

			return join.Length == 0 ? asNumStr : String.Format("{0} {1}", join, asNumStr);
		}

		public static object AsNumeric<T>(T e) where T : Enum
		{
			var underlyingType = Enum.GetUnderlyingType(typeof(T));
			return Convert.ChangeType(e, underlyingType);
		}

		public static T[] ToArray<T>() where T : Enum
		{
			return Enum.GetValues(typeof(T)) as T[];
		}

		public static int IndexOf<T>(T e) where T : Enum
		{
			return Array.IndexOf(Enum.GetValues(e.GetType()), e);
		}

		public static T FromIndex<T>(int i) where T : Enum
		{
			return (T) Enum.GetValues(typeof(T)).GetValue(i);
		}
	}
}