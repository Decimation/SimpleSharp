using System.Text;

// ReSharper disable UnusedMember.Global

namespace SimpleSharp.Strings.Formatting
{
	/// <summary>
	/// Extensions for <see cref="StringBuilder"/>s
	/// </summary>
	public static class StringBuilderExt
	{
		public static StringBuilder AppendLineFormat(this StringBuilder sb, string s, params object[] args)
			=> sb.AppendFormat(s, args).AppendLine();

		public static StringBuilder RemoveTrailingDelimeter(this StringBuilder sb, string delim)
		{
			if (sb.Length > 0) {
				sb.Remove(sb.Length - delim.Length, delim.Length);
			}

			return sb;
		}
	}
}