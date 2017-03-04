using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace WinLock
{
	public static class NativeMethods
	{
		[DllImport("kernel32.dll")]
		public static extern IntPtr GetCurrentProcess();

		[DllImport("kernel32.dll")]
		public static extern bool DuplicateHandle(IntPtr hSourceProcessHandle, IntPtr hSourceHandle, IntPtr hTargetProcessHandle,
													out IntPtr lpTargetHandle, int dwDesiredAccess = 0x0, bool bInheritHandle = true,
													int dwOptions = 0x2);
	}
}
