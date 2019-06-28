using System;
using System.IO;

namespace SimpleSharp.Files
{
	/// <summary>
	/// Extensions for <see cref="FileInfo"/>
	/// </summary>
	public static class FileExt
	{
		private const int BYTES_TO_READ = sizeof(long);
		
		/// <summary>
		/// https://stackoverflow.com/questions/1358510/how-to-compare-2-files-fast-using-net
		/// </summary>
		public static bool ContentEquals(this FileInfo first, FileInfo second)
		{
			if (first.Length != second.Length)
				return false;

			if (String.Equals(first.FullName, second.FullName, StringComparison.OrdinalIgnoreCase))
				return true;

			int iterations = (int) Math.Ceiling((double) first.Length / BYTES_TO_READ);

			using (var fs1 = first.OpenRead())
			using (var fs2 = second.OpenRead()) {
				byte[] one = new byte[BYTES_TO_READ];
				byte[] two = new byte[BYTES_TO_READ];

				for (int i = 0; i < iterations; i++) {
					fs1.Read(one, 0, BYTES_TO_READ);
					fs2.Read(two, 0, BYTES_TO_READ);
					

					if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
						return false;
				}
			}

			return true;
		}
	}
}