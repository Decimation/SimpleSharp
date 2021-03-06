using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// ReSharper disable UnusedMember.Global

namespace SimpleSharp.Utilities
{
	/// <summary>
	/// Provides utilities for working with <see cref="FileSystemInfo"/>-related types
	/// </summary>
	public static class Files
	{
		private const char   EQUALS     = '=';
		private const string EQUALS_STR = "=";

		public static FileInfo FindFile(string dir, string name)
		{
			var cd = new DirectoryInfo(dir);
			return Search(cd, name, out var file) ? file : null;
		}

		public static bool Search(DirectoryInfo dir, string name, out FileInfo fi)
		{
			var files = dir.GetFiles();

			foreach (var file in files) {
				if (file.Name.Contains(name)) {
					fi = file;
					return true;
				}
			}

			fi = null;
			return false;
		}


		/// <summary>
		/// Reads a <see cref="Dictionary{TKey,TValue}"/> from a file
		/// </summary>
		/// <param name="file">File previously created with <see cref="WriteDictionary{TKey,TValue}"/></param>
		/// <param name="keyFunc">Function to parse the key into a type of <typeparamref name="TKey"/></param>
		/// <param name="valueFunc">Function to parse the value into a type of <typeparamref name="TValue"/></param>
		public static Dictionary<TKey, TValue> ReadDictionary<TKey, TValue>(FileInfo             file,
		                                                                    Func<string, TKey>   keyFunc,
		                                                                    Func<string, TValue> valueFunc)
		{
			string[]   lines   = File.ReadAllLines(file.FullName);
			string[][] strings = lines.Select(l => l.Split(EQUALS)).ToArray();
			var        dict    = new Dictionary<TKey, TValue>();

			for (int i = 0; i < strings.GetLength(0); i++) {
				string keyString   = strings[i][0];
				string valueString = strings[i][1];

				dict.Add(keyFunc(keyString), valueFunc(valueString));
			}

			return dict;
		}

		/// <summary>
		/// Writes a <see cref="Dictionary{TKey,TValue}"/> to <paramref name="file"/>.
		/// This <see cref="Dictionary{TKey,TValue}"/> can later be read using <see cref="ReadDictionary{TKey,TValue}"/>
		/// </summary>
		public static void WriteDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict, FileInfo file)
		{
			string[] lines = dict.Select(kvp => kvp.Key + EQUALS_STR + kvp.Value).ToArray();
			File.WriteAllLines(file.FullName, lines);
		}


		/// <summary>
		/// Gets or creates a file.
		/// </summary>
		/// <param name="name">Name of the file</param>
		/// <param name="file">The file that was found or created</param>
		/// <returns><c>true</c> if the file already existed. <c>false</c> if the file didn't exist and was created.</returns>
		public static bool GetOrCreateTempFile(string name, out FileInfo file)
		{
			var tmp     = Path.GetTempPath();
			var tmpFile = new FileInfo(tmp + name);

			bool exists = tmpFile.Exists;

			if (!exists) {
				tmpFile.Create();
			}

			file = tmpFile;
			return exists;
		}

		#region Extensions

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

		#endregion
	}
}