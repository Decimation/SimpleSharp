using System;

// ReSharper disable UnusedMember.Global

namespace SimpleSharp.Strings.Formatting
{
	public static class SystemFormatting
	{
		private const string SYSTEM_NAMESPACE = "System.";

		private const char TYPE_PARAM = '`';

		/// <summary>
		///     <example>
		///         Fixed buffer named <c>buf</c> evaluates to name <c>&lt;buf&gt;e__FixedBuffer</c>
		///     </example>
		/// </summary>
		private const string FIXED_BUFFER_NAME = TYPE_PARAM_FMT + "e__FixedBuffer";

		private const string LT_STR = "<";
		private const string GT_STR = ">";

		private const string TYPE_PARAM_FMT = LT_STR + "{0}" + GT_STR;

		/// <summary>
		///     <example>
		///         Backing field named <c>field</c> evaluates to name <c>&lt;field&gt;k__BackingField</c>
		///     </example>
		/// </summary>
		private const string BACKING_FIELD_NAME = TYPE_PARAM_FMT + BACKING_FIELD_NAME_SUFFIX;

		private const string BACKING_FIELD_NAME_SUFFIX = "k__BackingField";


		private const string GET_PREFIX = "get_";
		private const string SET_PREFIX = "set_";

		/// <summary>
		///     <example>
		///         Add operation evaluates to <c>op_Addition</c>
		///     </example>
		/// </summary>
		private const string OPERATOR_PREFIX = "op_";

		/// <summary>
		///     Pretty-prints a type with generic arguments
		/// </summary>
		public static string GenericName<T>()
		{
			return GenericName(typeof(T));
		}

		/// <summary>
		///     Pretty-prints a type with generic arguments
		/// </summary>
		public static string GenericName(Type t)
		{
			string name  = t.Name;
			int    index = name.IndexOf(TYPE_PARAM);

			if (index == -1)
				return name;

			name = name.JSubstring(0, index);

			Type[] genArgs  = t.GetGenericArguments();
			var    genNames = new string[genArgs.Length];

			for (int i = 0; i < genNames.Length; i++)
				genNames[i] = genArgs[i].Name.Erase(SYSTEM_NAMESPACE);

			name += String.Format(TYPE_PARAM_FMT, genNames.AutoJoin());
			return name;
		}

		public static string NameOfBackingField(string name)
		{
			if (name.Contains(BACKING_FIELD_NAME_SUFFIX)) {
				return name.SubstringBetween(LT_STR, GT_STR);
			}

			return null;
		}

		public static string TypeNameOfFixedBuffer(string fieldName)
		{
			return String.Format(FIXED_BUFFER_NAME, fieldName);
		}

		public static string NameOfAutoProperty(string fieldName)
		{
			if (fieldName.Contains(BACKING_FIELD_NAME_SUFFIX)) {
				string x = fieldName.JSubstring(fieldName.IndexOf(TYPE_PARAM_FMT[0]) + 1,
				                                fieldName.IndexOf(TYPE_PARAM_FMT[2]));

				return x;
			}

			return null;
		}

		/// <summary>
		///     Gets the internal name of an auto-property's backing field.
		///     <example>If the auto-property's name is <c>X</c>, the backing field name is <c>&lt;X&gt;k__BackingField</c>.</example>
		/// </summary>
		/// <param name="propName">Auto-property's name</param>
		/// <returns>Internal name of the auto-property's backing field</returns>
		public static string NameOfAutoPropertyBackingField(string propName)
		{
			return String.Format(BACKING_FIELD_NAME, propName);
		}

		public static string NameOfGetPropertyMethod(string propName)
		{
			return GET_PREFIX + propName;
		}
	}
}