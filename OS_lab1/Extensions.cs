using OS_lab1.Winapi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace OS_lab1
{
	public static class Extensions
	{
		[DllImport("kernel32.dll")]
		public static extern uint GetLastError();

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		static extern bool FileTimeToSystemTime(in FILETIME filetime, SYSTEMTIME systemTime);

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		static extern bool FileTimeToLocalFileTime(in FILETIME filetime, out FILETIME localFileTime);

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		static extern bool GetSystemTime(SYSTEMTIME systemTime);

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		static extern bool SystemTimeToFileTime(SYSTEMTIME systemTime, out FILETIME filetime);
		public static uint EnumToUint<TValue>(this TValue value) where TValue : Enum
				=> (uint)(object)value;
		public static IEnumerable<T> ParseFlags<T>(uint fileSystemFlags, IEnumerable<T> allFlags = null) where T : Enum
		{
			if (allFlags == null)
				allFlags = (T[])Enum.GetValues(typeof(T));
			foreach (var flag in allFlags)
			{
				if ((fileSystemFlags & flag.EnumToUint()) == flag.EnumToUint())
					yield return flag;
			}
		}

		public static string FileTimeToString(this FILETIME t)
		{
			FILETIME localT;
			if (FileTimeToLocalFileTime(in t, out localT))
			{
				SYSTEMTIME time = new SYSTEMTIME();
				if (FileTimeToSystemTime(in localT, time))
				{
					return $"{time.day}/{time.month}/{time.year} {time.hour}:{time.minutes}:{time.seconds}:{time.milliseconds}";
				}
			}
			Console.WriteLine($"Error: code {GetLastError()}");
			return "";
		}

		public static FILETIME GetSystemFileTime()
		{
			SYSTEMTIME sysTime = new SYSTEMTIME();
			if (GetSystemTime(sysTime))
			{
				FILETIME ftime;
				if (SystemTimeToFileTime(sysTime, out ftime))
				{
					return ftime;
				}
			}
			return new FILETIME();
		}
	}
}
