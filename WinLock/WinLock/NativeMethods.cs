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

		[DllImport("ole32.dll")]
		public static extern void CoTaskMemFree(IntPtr ptr);

		[DllImport("credui.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool CredUnPackAuthenticationBuffer(int dwFlags,
	                                                             IntPtr pAuthBuffer,
	                                                             uint cbAuthBuffer,
	                                                             StringBuilder pszUserName,
	                                                             ref int pcchMaxUserName,
	                                                             StringBuilder pszDomainName,
	                                                             ref int pcchMaxDomainame,
	                                                             StringBuilder pszPassword,
	                                                             ref int pcchMaxPassword);

		[DllImport("credui.dll", CharSet = CharSet.Auto)]
		public static extern int CredUIPromptForWindowsCredentials(ref CREDUI_INFO credUIOptions,
		                                                           int authError,
		                                                           ref uint authPackage,
		                                                           IntPtr InAuthBuffer,
		                                                           uint InAuthBufferSize,
		                                                           out IntPtr refOutAuthBuffer,
		                                                           out uint refOutAuthBufferSize,
		                                                           ref bool fSave,
		                                                           WindowsCredentialUIOptions flags);

		[DllImport("advapi32.dll", SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool LogonUser([MarshalAs(UnmanagedType.LPStr)] string pszUserName, [MarshalAs(UnmanagedType.LPStr)] string pszDomain,
		[MarshalAs(UnmanagedType.LPStr)] string pszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern void SetLastError(int errorCode);
	}

	[Flags]
	public enum WindowsCredentialUIOptions
	{
		/// <summary>
		/// Generic credentials. Cannot be used with <see cref="SecurePrompt"/>.
		/// </summary>
		Generic = 0x1,
		CheckBox = 0x2,
		AuthPackageOnly = 0x10,
		InCredOnly = 0x20,
		EnumerateAdmins = 0x100,
		EnumerateCurrentUser = 0x200,
		SecurePrompt = 0x1000,
		PrePrompting = 0x2000,
		Pack32WoW = 0x10000000,
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct CREDUI_INFO
	{
		public int cbSize;
		public IntPtr hwndParent;
		public string pszMessageText;
		public string pszCaptionText;
		public IntPtr hbmBanner;
	}
}
