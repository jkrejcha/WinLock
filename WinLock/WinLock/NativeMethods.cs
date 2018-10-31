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
		public static extern bool LogonUser([MarshalAs(UnmanagedType.LPStr)] string pszUserName, 
		                                    [MarshalAs(UnmanagedType.LPStr)] string pszDomain,
                                    		[MarshalAs(UnmanagedType.LPStr)] string pszPassword,
                                    		int dwLogonType,
                                    		int dwLogonProvider,
                                    		out IntPtr phToken);

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
		/// <summary>
		/// Enumerate admins.
		/// </summary>
		EnumerateAdmins = 0x100,
		/// <summary>
		/// Enumerate the current user.
		/// </summary>
		EnumerateCurrentUser = 0x200,
		/// <summary>
		/// Prompt on the secure desktop.
		/// </summary>
		SecurePrompt = 0x1000,
		PrePrompting = 0x2000,
		Pack32WoW = 0x10000000,
	}

	/// <summary>
	/// References types of logons that may occur.
	/// </summary>
	public enum LogonType : int
	{
		/// <summary>
		/// A user logged on to this computer.
		/// </summary>
		Interactive = 2,
		/// <summary>
		/// A user or computer logged on to this computer from the network.
		/// </summary>
		Network = 3,
		/// <summary>
		/// Batch logon type is used by batch servers, where processes may be
		/// executing on behalf of a user without their direct intervention.
		/// </summary>
		Batch = 4,
		/// <summary>
		/// A service was started by the Service Control Manager.
		/// </summary>
		Service = 5,
		/// <summary>
		/// This workstation was unlocked.
		/// </summary>
		Unlock = 7,
		/// <summary>
		/// A user logged on to this computer from the network. The user's 
		/// password was passed to the authentication package in its unhashed 
		/// form. The built-in authentication packages all hash credentials 
		/// before sending them across the network. The credentials do not 
		/// traverse the network in plaintext (also called cleartext).
		/// </summary>
		NetworkCleartext = 8,
		/// <summary>
		/// A caller cloned its current token and specified new
		/// credentials for outbound connections. The new logon session
		/// has the same local identity, but uses different credentials
		/// for other network connections.
		/// </summary>
		NewCredentials = 9,
		/// <summary>
		/// A user logged on to this computer remotely using Terminal 
		/// Services or Remote Desktop.
		/// </summary>
		RemoteInteractive = 10,
		/// <summary>
		/// A user logged on to this computer with network credentials 
		/// that were stored locally on the computer. The domain controller
		/// was not contacted to verify the credentials.
		/// </summary>
		CachedInteractive = 11,
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
