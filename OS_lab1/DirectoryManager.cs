using OS_lab1.Winapi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OS_lab1
{
	class DirectoryManager
	{
		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Cdecl)]
		static extern bool CreateDirectory([MarshalAs(UnmanagedType.LPUTF8Str)] string pathName,
										SECURITY_ATTRIBUTES securityAttributes);

		public void CreateDirectory()
		{
			Console.WriteLine("Please, input target directory full name");
			string path = Console.ReadLine();
			if (CreateDirectory(path, null))
				Console.WriteLine($"Directory at path {path} was created successfully");
			else
				Console.WriteLine($"Error creating directory at path {path}");
		}

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Cdecl)]
		static extern bool RemoveDirectory([MarshalAs(UnmanagedType.LPUTF8Str)] string pathName);

		public void RemoveDirectory()
		{
			Console.WriteLine("Please, input target directory full name");
			string path = Console.ReadLine();
			if (RemoveDirectory(path))
				Console.WriteLine($"Directory at path {path} was deleted successfully");
			else
				Console.WriteLine($"Error deleting directory at path {path}");
		}
	}
}
