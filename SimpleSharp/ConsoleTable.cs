using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleSharp
{
	/// <summary>
	///     Source: https://github.com/khalidabuhakmeh/ConsoleTables
	/// </summary>
	public class ConsoleTable
	{
		public ConsoleTable(params string[] columns)
			: this(new ConsoleTableOptions
			{
				Columns = new List<string>(columns)
			}) { }

		public ConsoleTable(ConsoleTableOptions options)
		{
			Options = options ?? throw new ArgumentNullException(nameof(options));
			Rows    = new List<object[]>();
			Columns = new List<object>(options.Columns);
		}

		public List<object>   Columns { get; protected set; }
		public List<object[]> Rows    { get; protected set; }

		public ConsoleTableOptions Options { get; protected set; }

		public ConsoleTable AddColumn(object name)
		{
			Columns.Add(name);
			return this;
		}

		public ConsoleTable AddColumn(string name)
		{
			Columns.Add(name);
			return this;
		}

		public ConsoleTable Attach(int columnIndex, object column, params object[] rowColumn)
		{
			if (rowColumn.Length != Rows.Count) {
				throw new ArgumentOutOfRangeException(CreateOutOfRangeMessage(rowColumn.Length, Rows.Count));
			}

			Columns.Insert(columnIndex, column);


			var cpy = new List<object[]>(Rows.Count);
			int lim = rowColumn.Length;

			for (int i = 0; i < lim; i++) {
				var row   = Rows[i];
				var rgCpy = Arrays.Insert(row, columnIndex, rowColumn[i]);

				cpy.Add(rgCpy);
			}

			Rows.Clear();
			Rows.AddRange(cpy);

			return this;
		}

		public ConsoleTable Attach(object column, params object[] rowColumn)
		{
			return Attach(0, column, rowColumn);
		}
		
		public ConsoleTable AttachEnd(object column, params object[] rowColumn)
		{
			return Attach(Columns.Count, column, rowColumn);
		}

		public ConsoleTable AddColumn(IEnumerable<string> names)
		{
			foreach (string name in names)
				Columns.Add(name);
			return this;
		}

		public ConsoleTable AddColumn(IEnumerable<object> names)
		{
			foreach (string name in names)
				Columns.Add(name);
			return this;
		}

		private static string CreateOutOfRangeMessage(int colCnt, int len)
		{
			// $"The number columns in the row ({Columns.Count}) does not match the values ({values.Length}";
			return String.Format("The number columns in the row ({0}) does not match the values ({1})", colCnt, len);
		}

		public ConsoleTable AddRow(params object[] values)
		{
			if (values == null)
				throw new ArgumentNullException(nameof(values));

			if (!Columns.Any())
				throw new Exception("Please set the columns first");

			if (Columns.Count != values.Length)
				throw new Exception(CreateOutOfRangeMessage(Columns.Count, values.Length));

			Rows.Add(values);
			return this;
		}


		public ConsoleTable RemoveColumn(int index)
		{
			Columns.RemoveAt(index);
			for (int i = Rows.Count - 1; i >= 0; i--)
				Rows[i] = Arrays.RemoveAt(Rows[i], index);

			return this;
		}


		public static ConsoleTable From<T>(IEnumerable<T> values)
		{
			var table = new ConsoleTable();

			IEnumerable<string> columns = GetColumns<T>();

			table.AddColumn(columns);

			var val = values.Select(value => columns.Select(column => GetColumnValue<T>(value, column)));

			foreach (IEnumerable<object> propertyValues in val)
				table.AddRow(propertyValues.ToArray());

			return table;
		}

		public override string ToString() => ToMarkDownString();

		public string ToStringDefault()
		{
			var builder = new StringBuilder();

			// find the longest column by searching each row
			List<int> columnLengths = ColumnLengths();

			// create the string format with padding
			string format = Enumerable.Range(0, Columns.Count)
			                          .Select(i => " | {" + i + ",-" + columnLengths[i] + "}")
			                          .Aggregate((s, a) => s + a) + " |";

			// find the longest formatted line
			int    maxRowLength  = Math.Max(0, Rows.Any() ? Rows.Max(row => String.Format(format, row).Length) : 0);
			string columnHeaders = String.Format(format, Columns.ToArray());

			// longest line is greater of formatted columnHeader and longest row
			int longestLine = Math.Max(maxRowLength, columnHeaders.Length);

			// add each row
			List<string> results = Rows.Select(row => String.Format(format, row)).ToList();

			// create the divider
			string divider = " " + String.Join("", Enumerable.Repeat("-", longestLine - 1)) + " ";

			builder.AppendLine(divider);
			builder.AppendLine(columnHeaders);

			foreach (string row in results) {
				builder.AppendLine(divider);
				builder.AppendLine(row);
			}

			builder.AppendLine(divider);

			if (Options.EnableCount) {
				builder.AppendLine();
				builder.AppendFormat(" Count: {0}", Rows.Count);
			}

			return builder.ToString();
		}

		public string ToMarkDownString() => ToMarkDownString('|');

		private string ToMarkDownString(char delimiter)
		{
			var builder = new StringBuilder();

			// find the longest column by searching each row
			List<int> columnLengths = ColumnLengths();

			// create the string format with padding
			string format = Format(columnLengths, delimiter);

			// find the longest formatted line
			string columnHeaders = String.Format(format, Columns.ToArray());


			// add each row
			List<string> results = Rows.Select(row => String.Format(format, row)).ToList();

			// create the divider
			string divider = Regex.Replace(columnHeaders, @"[^|]", "-");

			// custom subroutine:
			// remove the first delimiter if the first column is empty
			if (Columns[0].ToString() == String.Empty) columnHeaders = ' ' + columnHeaders.Substring(1);

			builder.AppendLine(columnHeaders);
			builder.AppendLine(divider);
			results.ForEach(row => builder.AppendLine(row));

			return builder.ToString();
		}

		public string ToMinimalString() => ToMarkDownString(Char.MinValue);

		public string ToStringAlternative()
		{
			var builder = new StringBuilder();

			// find the longest column by searching each row
			List<int> columnLengths = ColumnLengths();

			// create the string format with padding
			string format = Format(columnLengths);

			// find the longest formatted line
			string columnHeaders = String.Format(format, Columns.ToArray());

			// add each row
			List<string> results = Rows.Select(row => String.Format(format, row)).ToList();

			// create the divider
			string divider     = Regex.Replace(columnHeaders, @"[^|]", "-");
			string dividerPlus = divider.Replace("|", "+");

			builder.AppendLine(dividerPlus);
			builder.AppendLine(columnHeaders);

			foreach (string row in results) {
				builder.AppendLine(dividerPlus);
				builder.AppendLine(row);
			}

			builder.AppendLine(dividerPlus);

			return builder.ToString();
		}

		private string Format(IReadOnlyList<int> columnLengths, char delimiter = '|')
		{
			string delimiterStr = delimiter == Char.MinValue ? String.Empty : delimiter.ToString();
			string format = (Enumerable.Range(0, Columns.Count)
			                           .Select(i => " " + delimiterStr + " {" + i + ",-" + columnLengths[i] + "}")
			                           .Aggregate((s, a) => s + a) + " " + delimiterStr).Trim();
			return format;
		}

		private List<int> ColumnLengths()
		{
			List<int> columnLengths = Columns
			                         .Select((t, i) => Rows.Select(x => x[i])
			                                               .Union(new[] {Columns[i]})
			                                               .Where(x => x != null)
			                                               .Select(x => x.ToString().Length).Max())
			                         .ToList();
			return columnLengths;
		}

		public void Write(TableFormat tableFormat = TableFormat.Default)
		{
			switch (tableFormat) {
				case SimpleSharp.TableFormat.Default:
					Console.WriteLine(ToString());
					break;
				case SimpleSharp.TableFormat.MarkDown:
					Console.WriteLine(ToMarkDownString());
					break;
				case SimpleSharp.TableFormat.Alternative:
					Console.WriteLine(ToStringAlternative());
					break;
				case SimpleSharp.TableFormat.Minimal:
					Console.WriteLine(ToMinimalString());
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(tableFormat), tableFormat, null);
			}
		}

		private static IEnumerable<string> GetColumns<T>()
		{
			return typeof(T).GetProperties().Select(x => x.Name).ToArray();
		}

		private static object GetColumnValue<T>(object target, string column)
		{
			return typeof(T).GetProperty(column).GetValue(target, null);
		}
	}

	public class ConsoleTableOptions
	{
		public IEnumerable<string> Columns     { get; set; } = new List<string>();
		public bool                EnableCount { get; set; } = false;
	}

	public enum TableFormat
	{
		Default     = 0,
		MarkDown    = 1,
		Alternative = 2,
		Minimal     = 3
	}
}