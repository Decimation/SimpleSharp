using System;
using System.Threading.Tasks;

namespace SimpleSharp.Utilities
{
	public static class Tasks
	{
		private static readonly Exception Timeout = new TimeoutException();
		
		public static void Run(Action action, TimeSpan timeSpan, bool throwOnFail = false)
		{
			var task = Task.Run(action);
			
			if (task.Wait(timeSpan))
				return;
			else {
				if (throwOnFail)
					throw Timeout;
			}
		}
		
		public static T Run<T>(Func<T> func, TimeSpan timeSpan, bool throwOnFail = false)
		{
			var task = Task.Run(func);
			
			if (task.Wait(timeSpan))
				return task.Result;
			else {
				if (throwOnFail)
					throw Timeout;
			}

			return default;
		}
	}
}