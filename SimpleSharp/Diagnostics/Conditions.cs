#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using SimpleSharp.Extensions;

#endregion

// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Global
// ReSharper disable UnusedMember.Global

namespace SimpleSharp.Diagnostics
{
	#region

	#endregion


	public static class Conditions
	{
		// 3-16-19: Use string interpolation expression disabled cause it's fucking annoying


		private const string VALUE_NULL_HALT = "value:null => halt";
		private const string COND_FALSE_HALT = "condition:false => halt";
		private const string STRING_FMT_ARG  = "message";


		[StringFormatMethod(STRING_FMT_ARG)]
		private static string CreateErrorString(string err, string message = null, params object[] args)
		{
			var sb = new StringBuilder();

			sb.Append(err);

			if (!String.IsNullOrWhiteSpace(message)) {
				sb.AppendFormat(": {0}", String.Format(message, args));
			}

			return sb.ToString();
		}

		private static string GetFailedExpression()
		{
			// Skip to failed method
			var frame      = new StackFrame(2, true);
			var prevMethod = frame.GetMethod();
			var ln         = frame.GetFileLineNumber();
			var f          = frame.GetFileName();

			// ReSharper disable once AssignNullToNotNullAttribute
			var src = new FileInfo(f);

			string argSrc;

			using (var txt = src.OpenText()) {
				for (int i = 0; i < ln - 1; i++) {
					txt.ReadLine();
				}


				string callLine = txt.ReadLine();
				argSrc = callLine.SubstringBetween("(", ")");
			}

			return argSrc;
		}

		[ContractAnnotation(COND_FALSE_HALT)]
		public static void SmartAssert(bool condition)
		{
			if (!condition) {
				string src = GetFailedExpression();
				string msg = String.Format("Assertion failed. Argument: \"{0}\"", src);
				throw new ArgumentException(msg);
			}
		}

		#region NotNull

		private const string NULL_FAIL = "Value cannot be null";

		[StringFormatMethod(STRING_FMT_ARG)]
		private static string NullMessage(string message = null, params object[] args)
			=> CreateErrorString(NULL_FAIL, message, args);

		/// <summary>
		///     Checks whether <paramref name="value" /> is <c>null</c>
		///     <remarks>
		///         If <typeparamref name="T" /> is a value type, this method checks whether <paramref name="value" /> is
		///         <c>default</c>
		///     </remarks>
		/// </summary>
		[ContractAnnotation(VALUE_NULL_HALT)]
		public static void NotNull<T>(T value, string name)
		{
			bool isNullOrDefault;

			if (typeof(T).IsValueType) {
				// Value is default?
				isNullOrDefault = value.Equals(default);
			}
			else {
				// Value is null?
				isNullOrDefault = value == null;
			}

			if (isNullOrDefault) {
				throw new ArgumentNullException(name, NullMessage());
			}
		}

		/// <summary>
		///     Checks whether <paramref name="value" /> is <see cref="IntPtr.Zero" />
		/// </summary>
		[ContractAnnotation(VALUE_NULL_HALT)]
		public static void NotNull(IntPtr value, string name)
		{
			if (value == IntPtr.Zero) {
				throw new ArgumentNullException(name, NullMessage());
			}
		}

		/// <summary>
		///     Checks whether <paramref name="value" /> is <c>null</c>
		/// </summary>
		[ContractAnnotation(VALUE_NULL_HALT)]
		public static unsafe void NotNull(void* value, string name)
			=> NotNull((IntPtr) value, name);

		#endregion

		#region Require

		private const string PRECONDITION_FAIL = "Precondition failed";

		[StringFormatMethod(STRING_FMT_ARG)]
		private static string RequireMessage(string message = null, params object[] args)
			=> CreateErrorString(PRECONDITION_FAIL, message, args);


		/// <summary>
		///     Specifies a precondition
		/// </summary>
		[ContractAnnotation(COND_FALSE_HALT)]
		public static void Require(bool condition)
		{
			if (!condition) {
				throw new InvalidOperationException(RequireMessage());
			}
		}

		/// <summary>
		///     Specifies a precondition for a parameter
		/// </summary>
		[ContractAnnotation(COND_FALSE_HALT)]
		public static void Require(bool condition, string param)
		{
			if (!condition) {
				throw new ArgumentException(RequireMessage(), param);
			}
		}

		/// <summary>
		///     Specifies a precondition for a parameter
		/// </summary>
		[ContractAnnotation(COND_FALSE_HALT)]
		public static void Require(bool condition, string message, string param)
		{
			if (!condition) {
				throw new ArgumentException(RequireMessage(message), param);
			}
		}

		#endregion

		#region Ensure

		private const string POSTCONDITION_FAIL = "Postcondition failed";

		[StringFormatMethod(STRING_FMT_ARG)]
		private static string EnsureMessage(string message = null, params object[] args)
			=> CreateErrorString(POSTCONDITION_FAIL, message, args);

		/// <summary>
		///     Specifies a postcondition
		/// </summary>
		[ContractAnnotation(COND_FALSE_HALT)]
		public static void Ensure(bool condition)
		{
			if (!condition) {
				throw new InvalidOperationException(EnsureMessage());
			}
		}

		/// <summary>
		///     Specifies a postcondition for a parameter
		/// </summary>
		[ContractAnnotation(COND_FALSE_HALT)]
		public static void Ensure(bool condition, string param)
		{
			if (!condition) {
				throw new ArgumentException(EnsureMessage(), param);
			}
		}

		/// <summary>
		///     Specifies a postcondition for a parameter
		/// </summary>
		[ContractAnnotation(COND_FALSE_HALT)]
		public static void Ensure(bool condition, string message, string param)
		{
			if (!condition) {
				throw new ArgumentException(EnsureMessage(message), param);
			}
		}

		#endregion

		#region Assert

		private const string ASSERTION_FAIL    = "Assertion failed";
		private const string DEBUG_CONDITIONAL = "DEBUG";

		[StringFormatMethod(STRING_FMT_ARG)]
		private static string AssertionMessage(string message = null, params object[] args)
			=> CreateErrorString(ASSERTION_FAIL, message, args);


		[ContractAnnotation(COND_FALSE_HALT)]
		[Conditional(DEBUG_CONDITIONAL)]
		public static void AssertDebug(bool condition)
		{
			if (!condition) {
				Debug.Fail(AssertionMessage());
			}
		}

		[StringFormatMethod(STRING_FMT_ARG)]
		[ContractAnnotation(COND_FALSE_HALT)]
		[Conditional(DEBUG_CONDITIONAL)]
		public static void AssertDebug(bool condition, string message, params object[] args)
		{
			if (!condition) {
				Debug.Fail(AssertionMessage(message, args));
			}
		}

		[ContractAnnotation(COND_FALSE_HALT)]
		public static void Assert(bool condition)
		{
			if (!condition) {
				Trace.Fail(AssertionMessage());
			}
		}

		[StringFormatMethod(STRING_FMT_ARG)]
		[ContractAnnotation(COND_FALSE_HALT)]
		public static void Assert(bool condition, string message, params object[] args)
		{
			if (!condition) {
				Trace.Fail(AssertionMessage(message, args));
			}
		}

		#endregion
	}
}