using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OS_lab1.Winapi;
using System.Runtime.InteropServices.ComTypes;

namespace OS_lab1
{
	class FileManager
	{
		[DllImport("kernel32.dll")]
		protected static extern uint GetLastError();

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		protected static extern bool CopyFile([MarshalAs(UnmanagedType.LPUTF8Str)] string fromPathName,
									[MarshalAs(UnmanagedType.LPUTF8Str)] string toPathName,
									bool failIfExists);

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		protected static extern bool MoveFile([MarshalAs(UnmanagedType.LPUTF8Str)] string fromPathName,
									[MarshalAs(UnmanagedType.LPUTF8Str)] string toPathName);

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		protected static extern bool MoveFileEx([MarshalAs(UnmanagedType.LPUTF8Str)] string fromPathName,
									  [MarshalAs(UnmanagedType.LPUTF8Str)] string toPathName, uint flags);

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		protected static extern IntPtr CreateFile([MarshalAs(UnmanagedType.LPUTF8Str)] string fileName,
									  uint desiredAccess, uint shareMode, Winapi.SECURITY_ATTRIBUTES securityAttributes,
									  uint creationDisposition, uint flagsAndAttributes, IntPtr templateFile);

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		protected static extern bool CloseHandle(IntPtr hObject);

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		protected static extern uint GetFileAttributes([MarshalAs(UnmanagedType.LPUTF8Str)] string fileName);

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		protected static extern bool SetFileAttributes([MarshalAs(UnmanagedType.LPUTF8Str)] string fileName, uint fileAttributes);

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		protected static extern bool GetFileInformationByHandle(IntPtr fileHandle, BY_HANDLE_FILE_INFORMATION fileInfo);

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		protected static extern bool GetFileTime(IntPtr fileHandle, out FILETIME creationTime, 
									   out FILETIME lastAccessTime, out FILETIME lastWriteTime);

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		protected static extern bool SetFileTime(IntPtr fileHandle, in FILETIME creationTime,
							   in FILETIME lastAccessTime, in FILETIME lastWriteTime);
		protected bool GetFilePath(out string targetPath, bool allowInterrupt, string header = "Select target file full path + name ")
		{
			Console.WriteLine(header + (allowInterrupt ?
							  "(press enter without input to abort):" : ":"));
			targetPath = Console.ReadLine();
			if (allowInterrupt && targetPath == "")
				return false;
			return true;
		}

		protected bool GetSourceTargetPaths(out string sourcePath, out string targetPath, bool allowInterrupt, bool checkSourceExists = true)
		{
			sourcePath = "";
			targetPath = "";
			
			do
			{
				if (!GetFilePath(out sourcePath, allowInterrupt, "Select source file path "))
					return false;
				if (!checkSourceExists || !File.Exists(sourcePath))
				{
					Console.WriteLine("Source file doesn't exist");
					sourcePath = "";
				}
			} while (sourcePath == "");

			if (!GetFilePath(out targetPath, allowInterrupt))
			{
				sourcePath = "";
				return false;
			}
			return true;
		}

		public void CopyFile(bool checkOverwrite, IYNmessageBox YNmessageBox)
		{
			Console.WriteLine("Copy");
			string source, target;
			if (GetSourceTargetPaths(out source, out target, true))
			{
				if (CopyFile(source, target, checkOverwrite))
					Console.WriteLine("File copied successfully");
				else
				{
					uint lastErr = GetLastError();
					if (!checkOverwrite)
						Console.WriteLine($"File copy failed. Error code: {lastErr}");
					else if (lastErr == 80 || lastErr == 183 && YNmessageBox.Show("File already exists. Overwrite?"))
					{
						if (CopyFile(source, target, false))
							Console.WriteLine("File overwritten successfully");
						else
						{
							lastErr = GetLastError();
							Console.WriteLine($"File copy failed. Error code: {lastErr}");
						}
					}
					else
						Console.WriteLine($"File copy failed. Error code: {lastErr}");
				}
			}
			else
				Console.WriteLine("Operation aborted");
		}


		public void MoveFile(bool checkOverwrite, IYNmessageBox YNmessageBox)
		{
			Console.WriteLine("Move");
			string source, target;
			if (GetSourceTargetPaths(out source, out target, true))
			{
				if (checkOverwrite)
				{
					if (MoveFile(source, target))
						Console.WriteLine("File moved successfully");
					else
					{
						uint lastErr = GetLastError();
						if (lastErr == 80 || lastErr == 183 && YNmessageBox.Show("File already exists. Overwrite?"))
							if (MoveFileEx(source, target, (uint)MoveFlags.MOVEFILE_REPLACE_EXISTING
								| (uint)MoveFlags.MOVEFILE_COPY_ALLOWED))
								Console.WriteLine("File replaced successfully");
							else
							{
								lastErr = GetLastError();
								Console.WriteLine($"File replacement failed. Error code: {lastErr}");
							}
						else
							Console.WriteLine($"File move failed. Error code: {lastErr}");
					}
				}
				else if (MoveFile(source, target))
					Console.WriteLine("File moved successfully");
				else
				{
					uint lastErr = GetLastError();
					Console.WriteLine($"File move failed. Error code: {lastErr}");
				}
			}
			else
				Console.WriteLine("Operation aborted");
		}

		protected readonly static IntPtr INVALID_HANDLE_VALUE = (IntPtr)(-1);
		public void CreateFile(bool allowInterrupt)
		{
			string targetPath = "";
			if (!GetFilePath(out targetPath, allowInterrupt))
				return;
			var handle = CreateFile(targetPath, (uint)DesiredAccess.GENERIC_WRITE | (uint)DesiredAccess.GENERIC_READ,
						(uint)ShareMode.FILE_SHARE_READ | (uint)ShareMode.FILE_SHARE_WRITE | (uint)ShareMode.FILE_SHARE_DELETE,
						null, (uint)CreationDisposition.CREATE_ALWAYS, (uint)Winapi.FileAttributes.FILE_ATTRIBUTE_NORMAL,
						IntPtr.Zero);
			if (handle != INVALID_HANDLE_VALUE)
			{
				Console.WriteLine("File created successfully!");
				CloseHandle(handle);
			}
			else
				Console.WriteLine($"Error creating file. Error code: { GetLastError() }");
		}

		const uint INVALID_FILE_ATTRIBUTES = uint.MaxValue;
		public void PrintFileAttributes(string targetPath)
		{
			uint attributes = GetFileAttributes(targetPath);
			if (attributes != INVALID_FILE_ATTRIBUTES)
				foreach (var f in Extensions.ParseFlags<Winapi.FileAttributes>(attributes))
					Console.WriteLine("\t-" + f);
			else
				Console.WriteLine($"Error occured. Error code {GetLastError()}");
		}

		public void SetFileAttributes(string targetPath, Winapi.FileAttributes setAttribute)
		{
			if (SetFileAttributes(targetPath, (uint)setAttribute))
				Console.WriteLine("File attribute set successfully!");
			else
				Console.WriteLine($"Error setting file attribute. Error code {GetLastError()}");
		}

		bool GetFileReadHandle(string targetPath, out IntPtr handle)
		{
			handle = CreateFile(targetPath, (uint)DesiredAccess.GENERIC_READ,
				(uint)ShareMode.FILE_SHARE_READ | (uint)ShareMode.FILE_SHARE_WRITE,
				null, (uint)CreationDisposition.OPEN_EXISTING,
				0, IntPtr.Zero);
			if (handle == INVALID_HANDLE_VALUE)
			{
				Console.WriteLine($"Error occured opening file. Error code: {GetLastError()}");
				return false;
			}
			return true;
		}

		bool GetFileWriteHandle(string targetPath, out IntPtr handle)
		{
			handle = CreateFile(targetPath, (uint)DesiredAccess.GENERIC_WRITE | (uint)DesiredAccess.GENERIC_READ,
						(uint)ShareMode.FILE_SHARE_READ | (uint)ShareMode.FILE_SHARE_WRITE | (uint)ShareMode.FILE_SHARE_DELETE,
						null, (uint)CreationDisposition.OPEN_EXISTING, (uint)Winapi.FileAttributes.FILE_ATTRIBUTE_NORMAL,
						IntPtr.Zero);
			if (handle == INVALID_HANDLE_VALUE)
			{
				Console.WriteLine($"Error occured opening file. Error code: {GetLastError()}");
				return false;
			}
			return true;
		}

		public void PrintFileAttributesByHandle(string targetPath)
		{
			IntPtr handle;
			if (!GetFileReadHandle(targetPath, out handle))
				return;

			BY_HANDLE_FILE_INFORMATION fileInfo = new BY_HANDLE_FILE_INFORMATION();
			
			if (GetFileInformationByHandle(handle, fileInfo))
			{
				Console.WriteLine("File attributes:");
				foreach (var f in Extensions.ParseFlags<Winapi.FileAttributes>(fileInfo.fileAttributes))
					Console.WriteLine("\t-" + f);
				Console.WriteLine($"Volume serial: {fileInfo.volumeSerialNumber}");
				Console.WriteLine($"Filesize high id: {fileInfo.fileSizeHigh}");
				Console.WriteLine($"Filesize low id: {fileInfo.fileSizeLow}");
				Console.WriteLine($"File index high id: {fileInfo.fileIndexHigh}");
				Console.WriteLine($"File index low id: {fileInfo.fileIndexLow}");
				Console.WriteLine($"File links number: {fileInfo.numberOfLinks}");
			}
			else
				Console.WriteLine($"Error occured reading file attributes. Error code: {GetLastError()}");
			CloseHandle(handle);
		}

		public void PrintFileTimeAttributes(string targetPath)
		{
			IntPtr handle;
			if (!GetFileReadHandle(targetPath, out handle))
				return;
			FILETIME creationTime, accessTime, writeTime;
			if (GetFileTime(handle, out creationTime, out accessTime, out writeTime))
			{
				Console.WriteLine($"File creation time: {creationTime.FileTimeToString()}");
				Console.WriteLine($"Last file access time: {accessTime.FileTimeToString()}");
				Console.WriteLine($"Last file write time: {writeTime.FileTimeToString()}");
			}
			else
				Console.WriteLine($"Error occured reading file time attributes. Error code: {GetLastError()}");
			CloseHandle(handle);
		}

		public void SetFileTimeAttributes(string targetPath)
		{
			IntPtr handle;
			if (!GetFileWriteHandle(targetPath, out handle))
				return;
			var t = Extensions.GetSystemFileTime();
			if (SetFileTime(handle, in t, in t, in t))
				Console.WriteLine("File time attributes set successfully");
			else
				Console.WriteLine($"Error occured writing file time attributes. Error code: {GetLastError()}");
			CloseHandle(handle);
		}
	}
}
