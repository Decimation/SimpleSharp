namespace SimpleSharp.Strings.Formatting
{
	/// <summary>
	/// Extensions for <see cref="HexOptions"/>
	/// </summary>
	public static class HexOptionsExt
	{
		public static bool HasFlagFast(this HexOptions value, HexOptions flag)
		{
			//return value.HasFlag(flag);
			//if ((testItem & FlagTest.Flag1) == FlagTest.Flag1)
			return ((value & flag) == flag);
		}
	}
}