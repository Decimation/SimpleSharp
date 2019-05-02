using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable ParameterTypeCanBeEnumerable.Global

namespace SimpleSharp
{
	public static class Arrays
	{
		public static T Random<T>(T[] array)
		{
			int r = ThreadLocalRandom.Instance.Next(array.Length);
			return array[r];
		}

		public static T[] Add<T>(T[] arr, T value) => arr.Append(value).ToArray();

		public static void Add<T>(ref T[] arr, T value)
			=> arr = TransientOperate(arr, list => list.Add(value));

		public static void OrderBy<TSource, TKey>(ref TSource[] arr, Func<TSource, TKey> order)
		{
			arr = arr.OrderBy(order).ToArray();
		}

		public static int Max(int[,] arr)
		{
			return arr.Cast<int>()
			          .Concat(new[] {Int32.MinValue})
			          .Max();
		}

		/// <summary>
		/// Runs the <see cref="Action{T}"/> specified by <paramref name="fn"/> on <paramref name="arr"/>
		/// while it is a <see cref="List{T}"/>
		/// </summary>
		public static T[] TransientOperate<T>(T[] arr, Action<List<T>> fn)
		{
			List<T> list = arr.ToList();
			fn(list);
			arr = list.ToArray();
			return arr;
		}
		
		public static T[] RemoveAt<T>(T[] arr, int index)
		{
			return TransientOperate(arr, x => x.RemoveAt(index));
		}

		public static void RemoveAll<T>(ref T[] arr, Predicate<T> match)
		{
			arr = TransientOperate(arr, x => x.RemoveAll(match));
		}

		public static T[] Insert<T>(T[] arr, int index, T val)
		{
			return TransientOperate(arr, list =>
			{
				list.AddRange(arr);
				list.Insert(index, val);
			});
		}
	}
}