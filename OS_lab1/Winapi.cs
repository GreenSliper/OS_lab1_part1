using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace OS_lab1
{
	namespace Winapi
	{
		[StructLayout(LayoutKind.Sequential)]
		public class SYSTEMTIME
		{
			public ushort year;
			public ushort month;
			public ushort dayOfWeek;
			public ushort day;
			public ushort hour;
			public ushort minutes;
			public ushort seconds;
			public ushort milliseconds;
		}
		[StructLayout(LayoutKind.Sequential)]
		public class SECURITY_ATTRIBUTES
		{
			public uint length;
			public IntPtr securityDescriptor;
			public bool inheritHandle;
		}

		[StructLayout(LayoutKind.Sequential)]
		public class BY_HANDLE_FILE_INFORMATION
		{
			public uint fileAttributes;
			public FILETIME creationTime;
			public FILETIME lastAccessTime;
			public FILETIME lastWriteTime;
			public uint volumeSerialNumber;
			public uint fileSizeHigh;
			public uint fileSizeLow;
			public uint numberOfLinks;
			public uint fileIndexHigh;
			public uint fileIndexLow;
		}

		[Flags]
		public enum MoveFlags : uint
		{
			MOVEFILE_REPLACE_EXISTING = 1,
			MOVEFILE_COPY_ALLOWED = 2,
			MOVEFILE_DELAY_UNTIL_REBOOT = 4,
			MOVEFILE_WRITE_THROUGH = 8,
			MOVEFILE_CREATE_HARDLINK = 16,
			MOVEFILE_FAIL_IF_NOT_TRACKABLE = 32
		}

		/*
		0
		Prevents other processes from opening a file or device if they request delete, read, or write access.
		
		FILE_SHARE_DELETE
		Enables subsequent open operations on a file or device to request delete access.
		Otherwise, other processes cannot open the file or device if they request delete access.
		If this flag is not specified, but the file or device has been opened for delete access, the function fails.
		Delete access allows both delete and rename operations.
 
		FILE_SHARE_READ
		Enables subsequent open operations on a file or device to request read access.
		Otherwise, other processes cannot open the file or device if they request read access.
		If this flag is not specified, but the file or device has been opened for read access, the function fails.

		FILE_SHARE_WRITE
		Enables subsequent open operations on a file or device to request write access.
		Otherwise, other processes cannot open the file or device if they request write access.
		If this flag is not specified, but the file or device has been opened for write access or has a file 
		mapping with write access, the function fails.
		*/
		[Flags]
		public enum ShareMode : uint
		{
			None = 0,
			FILE_SHARE_READ = 1,
			FILE_SHARE_WRITE = 2,
			FILE_SHARE_DELETE = 4
		}

		/*
		CREATE_ALWAYS
		Creates a new file, always.
		If the specified file exists and is writable, the function overwrites the file, the function succeeds, 
		and last-error code is set to ERROR_ALREADY_EXISTS (183).
		If the specified file does not exist and is a valid path, a new file is created, the function succeeds, 
		and the last-error code is set to zero.
		For more information, see the Remarks section of this topic.

		CREATE_NEW
		Creates a new file, only if it does not already exist.
		If the specified file exists, the function fails and the last-error code is set to ERROR_FILE_EXISTS (80).
		If the specified file does not exist and is a valid path to a writable location, a new file is created.

		OPEN_ALWAYS
		Opens a file, always.
		If the specified file exists, the function succeeds and the last-error code is set to ERROR_ALREADY_EXISTS (183).
		If the specified file does not exist and is a valid path to a writable location, the function creates a file and 
		the last-error code is set to zero.

		OPEN_EXISTING
		Opens a file or device, only if it exists.
		If the specified file or device does not exist, the function fails and the last-error code is 
		set to ERROR_FILE_NOT_FOUND (2).

		TRUNCATE_EXISTING
		Opens a file and truncates it so that its size is zero bytes, only if it exists.
		If the specified file does not exist, the function fails and the last-error code is set to ERROR_FILE_NOT_FOUND (2).
		The calling process must open the file with the GENERIC_WRITE bit set as part of the dwDesiredAccess parameter.
		 */
		public enum CreationDisposition : uint
		{
			CREATE_NEW = 1,
			CREATE_ALWAYS = 2,
			OPEN_EXISTING = 3,
			OPEN_ALWAYS = 4,
			TRUNCATE_EXISTING = 5
		}

		/*
		####### ATTRIBUTES ##########################################################################################
		FILE_ATTRIBUTE_ARCHIVE
		The file should be archived. Applications use this attribute to mark files for backup or removal.
		
		FILE_ATTRIBUTE_ENCRYPTED
		The file or directory is encrypted. For a file, this means that all data in the file is encrypted. For a directory, this means that encryption is the default for newly created files and subdirectories. For more information, see File Encryption.
		This flag has no effect if FILE_ATTRIBUTE_SYSTEM is also specified.
		This flag is not supported on Home, Home Premium, Starter, or ARM editions of Windows.

		FILE_ATTRIBUTE_HIDDEN
		The file is hidden. Do not include it in an ordinary directory listing.
		
		FILE_ATTRIBUTE_NORMAL
		The file does not have other attributes set. This attribute is valid only if used alone.
		
		FILE_ATTRIBUTE_OFFLINE
		The data of a file is not immediately available. This attribute indicates that file data is physically moved to offline storage. This attribute is used by Remote Storage, the hierarchical storage management software. Applications should not arbitrarily change this attribute.
		
		FILE_ATTRIBUTE_READONLY
		The file is read only. Applications can read the file, but cannot write to or delete it.
		
		FILE_ATTRIBUTE_SYSTEM
		The file is part of or used exclusively by an operating system.
		
		FILE_ATTRIBUTE_TEMPORARY
		The file is being used for temporary storage.

		######### FLAGS ##########################################################################################

		FILE_FLAG_BACKUP_SEMANTICS
		The file is being opened or created for a backup or restore operation. 
		The system ensures that the calling process overrides file security checks 
		when the process has SE_BACKUP_NAME and SE_RESTORE_NAME privileges.
		You must set this flag to obtain a handle to a directory. A directory
		handle can be passed to some functions instead of a file handle. 

		FILE_FLAG_DELETE_ON_CLOSE
		The file is to be deleted immediately after all of its handles are closed, 
		which includes the specified handle and any other open or duplicated handles.
		If there are existing open handles to a file, the call fails unless they were 
		all opened with the FILE_SHARE_DELETE share mode.
		Subsequent open requests for the file fail, unless the FILE_SHARE_DELETE share mode is specified.

		FILE_FLAG_NO_BUFFERING
		The file or device is being opened with no system caching for data reads and writes. 
		This flag does not affect hard disk caching or memory mapped files.
		There are strict requirements for successfully working with files opened with CreateFile 
		using the FILE_FLAG_NO_BUFFERING flag, for details see File Buffering.

		FILE_FLAG_OPEN_NO_RECALL
		The file data is requested, but it should continue to be located in remote storage. 
		It should not be transported back to local storage. This flag is for use by remote storage systems.
		
		FILE_FLAG_OPEN_REPARSE_POINT
		Normal reparse point processing will not occur; CreateFile will 
		attempt to open the reparse point. When a file is opened, a file 
		handle is returned, whether or not the filter that controls the reparse point is operational.
		This flag cannot be used with the CREATE_ALWAYS flag.
		If the file is not a reparse point, then this flag is ignored.

		FILE_FLAG_OVERLAPPED
		The file or device is being opened or created for asynchronous I/O.
		When subsequent I/O operations are completed on this handle, the event 
		specified in the OVERLAPPED structure will be set to the signaled state.
		If this flag is specified, the file can be used for simultaneous read and write operations.
		If this flag is not specified, then I/O operations are serialized, even 
		if the calls to the read and write functions specify an OVERLAPPED structure.

		FILE_FLAG_POSIX_SEMANTICS
		Access will occur according to POSIX rules. This includes allowing 
		multiple files with names, differing only in case, for file systems 
		that support that naming. Use care when using this option, because files 
		created with this flag may not be accessible by applications that are written for MS-DOS or 16-bit Windows.
		
		FILE_FLAG_RANDOM_ACCESS
		Access is intended to be random. The system can use this as a hint to optimize file caching.
		This flag has no effect if the file system does not support cached I/O and FILE_FLAG_NO_BUFFERING.

		FILE_FLAG_SESSION_AWARE
		The file or device is being opened with session awareness. If this flag is not specified, 
		then per-session devices (such as a device using RemoteFX USB Redirection) cannot be opened by 
		processes running in session 0. This flag has no effect for callers not in session 0. This flag 
		is supported only on server editions of Windows.

		FILE_FLAG_SEQUENTIAL_SCAN
		Access is intended to be sequential from beginning to end. The system can use this as a hint to optimize file caching.
		This flag should not be used if read-behind (that is, reverse scans) will be used.
		This flag has no effect if the file system does not support cached I/O and FILE_FLAG_NO_BUFFERING.

		FILE_FLAG_WRITE_THROUGH
		Write operations will not go through any intermediate cache, they will go directly to disk.

		 */
		public enum FileAttributes : uint
		{
			FILE_ATTRIBUTE_READONLY = 0x1,
			FILE_ATTRIBUTE_HIDDEN = 0x2,
			FILE_ATTRIBUTE_SYSTEM = 0x4,
			FILE_ATTRIBUTE_ARCHIVE = 0x20,
			FILE_ATTRIBUTE_NORMAL = 0x80,
			FILE_ATTRIBUTE_TEMPORARY = 0x100,
			FILE_ATTRIBUTE_OFFLINE = 0x1000,
			FILE_ATTRIBUTE_ENCRYPTED = 0x4000
		}
		public enum FileFlags : uint
		{
			FILE_FLAG_BACKUP_SEMANTICS = 0x02000000,
			FILE_FLAG_DELETE_ON_CLOSE = 0x04000000,
			FILE_FLAG_NO_BUFFERING = 0x20000000,
			FILE_FLAG_OPEN_NO_RECALL = 0x00100000,
			FILE_FLAG_OPEN_REPARSE_POINT = 0x00200000,
			FILE_FLAG_OVERLAPPED = 0x40000000,
			FILE_FLAG_POSIX_SEMANTICS = 0x01000000,
			FILE_FLAG_RANDOM_ACCESS = 0x10000000,
			FILE_FLAG_SESSION_AWARE = 0x00800000,
			FILE_FLAG_SEQUENTIAL_SCAN = 0x08000000,
			FILE_FLAG_WRITE_THROUGH = 0x80000000
		}

		public enum DesiredAccess : uint
		{
			GENERIC_READ = 0x80000000,
			GENERIC_WRITE = 0x40000000,
			GENERIC_EXECUTE = 0x20000000,
			GENERIC_ALL = 0x10000000
		}

		/*
		FILE_CASE_PRESERVED_NAMES
		The specified volume supports preserved case of file names when it places a name on disk.

		FILE_CASE_SENSITIVE_SEARCH
		The specified volume supports case-sensitive file names.

		FILE_DAX_VOLUME
		The specified volume is a direct access (DAX) volume.
		This flag was introduced in Windows 10, version 1607.

		FILE_FILE_COMPRESSION
		The specified volume supports file-based compression.

		FILE_NAMED_STREAMS
		The specified volume supports named streams.

		FILE_PERSISTENT_ACLS
		The specified volume preserves and enforces access control lists (ACL). For example, 
		the NTFS file system preserves and enforces ACLs, and the FAT file system does not.

		FILE_READ_ONLY_VOLUME
		The specified volume is read-only.

		FILE_SEQUENTIAL_WRITE_ONCE
		The specified volume supports a single sequential write.

		FILE_SUPPORTS_ENCRYPTION
		The specified volume supports the Encrypted File System (EFS). For more information, see File Encryption.

		FILE_SUPPORTS_EXTENDED_ATTRIBUTES
		The specified volume supports extended attributes. An extended attribute is a piece of application-specific metadata
		that an application can associate with a file and is not part of the file's data.
		Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP:  This value is not 
		supported until Windows Server 2008 R2 and Windows 7.

		FILE_SUPPORTS_HARD_LINKS
		The specified volume supports hard links. For more information, see Hard Links and Junctions.
		Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP:  This value is not supported
		until Windows Server 2008 R2 and Windows 7.

		FILE_SUPPORTS_OBJECT_IDS
		The specified volume supports object identifiers.

		FILE_SUPPORTS_OPEN_BY_FILE_ID
		The file system supports open by FileID. For more information, see FILE_ID_BOTH_DIR_INFO.
		Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP:  This value is not supported
		until Windows Server 2008 R2 and Windows 7.

		FILE_SUPPORTS_REPARSE_POINTS
		The specified volume supports reparse points.

		FILE_SUPPORTS_SPARSE_FILES
		The specified volume supports sparse files.

		FILE_SUPPORTS_TRANSACTIONS
		The specified volume supports transactions. For more information, see About KTM.

		FILE_SUPPORTS_USN_JOURNAL
		The specified volume supports update sequence number (USN) journals. For more information, see Change Journal Records.
		Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP:  This value is not supported until 
		Windows Server 2008 R2 and Windows 7.

		FILE_UNICODE_ON_DISK
		The specified volume supports Unicode in file names as they appear on disk.

		FILE_VOLUME_IS_COMPRESSED
		The specified volume is a compressed volume, for example, a DoubleSpace volume.

		FILE_VOLUME_QUOTAS
		The specified volume supports disk quotas.

		FILE_SUPPORTS_BLOCK_REFCOUNTING
		The specified volume supports sharing logical clusters between files on the same volume. 
		The file system reallocates on writes to shared clusters. Indicates that FSCTL_DUPLICATE_EXTENTS_TO_FILE 
		is a supported operation.*/
		public enum FileSystemFlags : uint
		{
			FILE_CASE_PRESERVED_NAMES = 0x00000002,
			FILE_CASE_SENSITIVE_SEARCH = 0x00000001,
			FILE_FILE_COMPRESSION = 0x00000010,
			FILE_NAMED_STREAMS = 0x00040000,
			FILE_PERSISTENT_ACLS = 0x00000008,
			FILE_READ_ONLY_VOLUME = 0x00080000,
			FILE_SEQUENTIAL_WRITE_ONCE = 0x00100000,
			FILE_SUPPORTS_ENCRYPTION = 0x00020000,
			FILE_SUPPORTS_EXTENDED_ATTRIBUTES = 0x00800000,
			FILE_SUPPORTS_HARD_LINKS = 0x00400000,
			FILE_SUPPORTS_OBJECT_IDS = 0x00010000,
			FILE_SUPPORTS_OPEN_BY_FILE_ID = 0x01000000,
			FILE_SUPPORTS_REPARSE_POINTS = 0x00000080,
			FILE_SUPPORTS_SPARSE_FILES = 0x00000040,
			FILE_SUPPORTS_TRANSACTIONS = 0x00200000,
			FILE_SUPPORTS_USN_JOURNAL = 0x02000000,
			FILE_UNICODE_ON_DISK = 0x00000004,
			FILE_VOLUME_IS_COMPRESSED = 0x00008000,
			FILE_VOLUME_QUOTAS = 0x00000020,
			FILE_SUPPORTS_BLOCK_REFCOUNTING = 0x08000000
		}

		public enum EMoveMethod : uint
		{
			Begin = 0,
			Current = 1,
			End = 2
		}
	}
}
