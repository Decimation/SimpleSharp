using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

// ReSharper disable UnusedMember.Global

namespace SimpleSharp.Extensions
{
	/// <summary>
	/// Extensions for <see cref="IEnumerable{T}"/>
	/// </summary>
	public static class EnumerableExt
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsOutOfBounds<T>(this IList<T> list, int index)
			=> index >= list.Count || index < 0;

		/*public static IEnumerable<T> ForEach<T>(this IEnumerable<T> src, Action<T> fn)
		{
			var list = src.ToList();
			list.ForEach(fn);
			return list;
		}*/


		public static T Random<T>(this IEnumerable<T> ls) => Arrays.Random(ls.ToArray());
		
		/// <summary>
		/// Removes all values from <paramref name="list"/> specified by the indices in <paramref name="indexes"/>
		/// </summary>
		/// <param name="list"><see cref="List{T}"/> from which to remove values</param>
		/// <param name="indexes">Indices from which to remove values in <paramref name="list"/></param>
		public static IList<T> RemoveAtRange<T>(this IList<T> list, params int[] indexes)
		{
			int cnt = 0;
			foreach (int i in indexes) {
				list.RemoveAt(i - cnt++);
			}

			return list;
		}

		public static int IndexOf<T>(this IEnumerable<T> source, T value)
		{
			int                 index    = 0;
			EqualityComparer<T> comparer = EqualityComparer<T>.Default; // or pass in as a parameter
			foreach (var item in source) {
				if (comparer.Equals(item, value))
					return index;
				index++;
			}

			return -1;
		}

		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
		                                                             Func<TSource, TKey>       keySelector)
		{
			var knownKeys = new HashSet<TKey>();
			return source.Where(element => knownKeys.Add(keySelector(element)));
		}

		public static bool ContainsBy<TSource, TKey>(this IEnumerable<TSource> src,
		                                             Func<TSource, TKey>       selector,
		                                             TKey                      k)
		{
			return src.Select(selector).Contains(k);
		}

		public static bool ContainsAll(this IEnumerable lhs, IEnumerable rhs)
		{
			IList list1 = (IList) lhs;
			IList list2 = (IList) rhs;

			foreach (var elem in list1) {
				if (!list2.Contains(elem)) {
					return false;
				}
			}

			return true;
		}

		public static bool ContainsAll<T>(this IEnumerable<T> list1, IEnumerable<T> list2)
		{
			return list1.Intersect(list2).Count() == list1.Count();
		}

		public static IEnumerable<T> OrderBySequenceDictionary<T, TId>(
			this IEnumerable<T> source,
			IEnumerable<TId>    order,
			Func<T, TId>        idSelector)
		{
			var lookup = source.ToDictionary(idSelector, t => t);
			foreach (var id in order) {
				yield return lookup[id];
			}
		}

		public static IEnumerable<T> OrderBySequenceLookup<T, TId>(
			this IEnumerable<T> source,
			IEnumerable<TId>    order,
			Func<T, TId>        idSelector)
		{
			var lookup = source.ToLookup(idSelector, t => t);
			foreach (var id in order) {
				foreach (var t in lookup[id]) {
					yield return t;
				}
			}
		}
	}
}