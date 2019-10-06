using System;
using System.Diagnostics;
using System.Security.Principal;

// ReSharper disable UnusedMember.Global

namespace SimpleSharp.Utilities
{
	public static class Common
	{
		/// <summary>
		/// Creates a <see cref="Process"/> to execute <paramref name="cmd"/>
		/// </summary>
		/// <param name="cmd">Command to run</param>
		/// <param name="autoStart">Whether to automatically start the <c>cmd.exe</c> process</param>
		/// <returns><c>cmd.exe</c> process</returns>
		public static Process Shell(string cmd, bool autoStart = false)
		{
			const string CMD_EXE = "cmd.exe";

			var startInfo = new ProcessStartInfo
			{
				FileName               = CMD_EXE,
				Arguments              = String.Format("/C {0}", cmd),
				RedirectStandardOutput = true,
				RedirectStandardError  = true,
				UseShellExecute        = false,
				CreateNoWindow         = true
			};

			var process = new Process
			{
				StartInfo           = startInfo,
				EnableRaisingEvents = true
			};

			if (autoStart)
				process.Start();

			return process;
		}

		public static bool IsAdministrator()
		{
			return (new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator);
		}  
	}
}