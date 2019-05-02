namespace SimpleSharp.Strings
{
	/// <summary>
	/// Common <see cref="string"/> constants
	/// </summary>
	public static class StringConstants
	{
		public const char CHECK_MARK = '\u2713';
		public const char BALLOT_X   = '\u2717';

		public const string ALPHANUMERICS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		public const string NULL_STR      = "(null)";
		public const string JOIN_COMMA    = ", ";

		internal const string HEX_PREFIX = "0x";
		internal const string ZERO = "0";

		
		public const string NULL_TERMINATOR = "\0";
	}
}