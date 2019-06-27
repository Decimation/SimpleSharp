using SimpleSharp.Strings.Formatting;

namespace SimpleSharp.Extensions
{
	/// <summary>
	/// Extensions for <see cref="HexOptions"/>
	/// </summary>
	public static class FormatOptionsExt
	{
		public static bool HasFlagFast(this HexOptions value, HexOptions flag)
		{
			//return value.HasFlag(flag);
			//if ((testItem & FlagTest.Flag1) == FlagTest.Flag1)
			return ((value & flag) == flag);
		}
	}
}