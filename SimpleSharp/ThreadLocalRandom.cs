using System;
using System.Threading;

namespace SimpleSharp
{
	public static class ThreadLocalRandom
	{
		private static int _seed;

		private static readonly ThreadLocal<Random> ThreadLocal = new ThreadLocal<Random>
			(() => new Random(Interlocked.Increment(ref _seed)));

		static ThreadLocalRandom()
		{
			_seed = Environment.TickCount;
		}

		public static Random Instance => ThreadLocal.Value;
	}
}