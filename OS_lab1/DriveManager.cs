using OS_lab1.Winapi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OS_lab1
{
	class DriveManager
	{
		[DllImport("kernel32.dll")]
		static extern uint GetLogicalDrives();

		static string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		public List<char> GetDrives()
		{
			var drives = GetLogicalDrives();
			var driveLetters = new List<char>();
			for (int i = 0; i < alphabet.Length; i++)
			{
				if ((drives & 1) == 1)
					driveLetters.Add(alphabet[i]);
				drives = drives >> 1;
			}
			return driveLetters;
		}

		public void PrintAllDrives()
		{
			var drives = GetDrives();
			Console.WriteLine($"Found {drives.Count} hard drives. Drive names: ");
			foreach (var d in GetDrives())
			{
				Console.Write(d + " ");
			}
			Console.WriteLine();
		}

		/// <param name="rootPathName">Format: 'A:\'</param>
		[DllImport("kernel32.dll")]
		static extern uint GetDriveType([MarshalAs(UnmanagedType.LPUTF8Str)] string rootPathName);

		public enum DriveType { 
			TypeUndefined, 
			RootDirectoryDoesntExist, 
			DRIVE_REMOVABLE, 
			DRIVE_FIXED, 
			DRIVE_REMOTE,
			DRIVE_CDROM,
			DRIVE_RAMDISK
		}

		[DllImport("kernel32.dll")]
		static extern uint GetVolumeInformation([MarshalAs(UnmanagedType.LPUTF8Str)] string rootPathName,
												StringBuilder nameBuffer, uint nameBufferLength, out uint volumeSerialNumber,
												out uint maximumComponentLength, out uint fileSystemFlags,
												StringBuilder fileSystemNameBuffer, uint fileSystemNameBufferLength);
		public DriveType GetDriveType(char driveLetter)
		{
			return (DriveType)GetDriveType(driveLetter + ":\\");
		}

		void PrintVolumeInfo(char driveLetter)
		{
			StringBuilder nameBuffer = new StringBuilder((int)nameBufferLength),
				fileSystemNameBuffer = new StringBuilder((int)nameBufferLength);
			uint volumeSerialNumber = 0, maximumComponentLength = 0, fileSystemFlags = 0;
			GetVolumeInformation(driveLetter + ":\\", nameBuffer, nameBufferLength,
				out volumeSerialNumber, out maximumComponentLength, out fileSystemFlags,
				fileSystemNameBuffer, nameBufferLength).ToString();
			Console.WriteLine("Volume name: " + nameBuffer.ToString());
			Console.WriteLine("File system name: " + fileSystemNameBuffer.ToString());
			Console.WriteLine("Serial: " + volumeSerialNumber);
			Console.WriteLine("Maximum component length: " + maximumComponentLength);
			Console.WriteLine("File system flags:");
			foreach (var flag in Extensions.ParseFlags<FileSystemFlags>(fileSystemFlags))//ParseFileSystemFlags(fileSystemFlags))
				Console.WriteLine("\t- " + flag.ToString());
		}

		[DllImport("kernel32.dll")]
		static extern uint GetDiskFreeSpace([MarshalAs(UnmanagedType.LPUTF8Str)] string rootPathName,
												out uint sectorsPerCluster, out uint bytesPerSector,
												out uint numberOfFreeClusters, out uint totalNumberOfClusters);

		void PrintDiskFreeSpaceInfo(char driveLetter)
		{
			uint sectorsPerCluster = 0, bytesPerSector = 0,
				numberOfFreeClusters = 0, totalNumberOfClusters = 0;
			GetDiskFreeSpace(driveLetter + ":\\", out sectorsPerCluster, out bytesPerSector,
				out numberOfFreeClusters, out totalNumberOfClusters);
			Console.WriteLine("Disk Free Space ");
			Console.WriteLine("Sectors per cluster: " + sectorsPerCluster);
			Console.WriteLine("Bytes per sector: " + bytesPerSector);
			Console.WriteLine("Number of free clusters: " + numberOfFreeClusters);
			Console.WriteLine("Total number of clusters: " + totalNumberOfClusters);
		}

		const uint nameBufferLength = 256;
		public void PrintDriveInfo(char driveLetter)
		{
			Console.WriteLine(GetDriveType(driveLetter).ToString());
			PrintVolumeInfo(driveLetter);
			Console.WriteLine();
			PrintDiskFreeSpaceInfo(driveLetter);
		}
	}
}
