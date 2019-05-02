namespace SimpleSharp.Extensions
{
	/// <summary>
	/// Extensions for <see cref="FormatOptions"/>
	/// </summary>
	public static class FormatOptionsExt
	{
		public static bool HasFlagFast(this FormatOptions value, FormatOptions flag)
		{
			//return value.HasFlag(flag);
			//if ((testItem & FlagTest.Flag1) == FlagTest.Flag1)
			return ((value & flag) == flag);
		}
	}
}