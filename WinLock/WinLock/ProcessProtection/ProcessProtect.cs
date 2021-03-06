﻿using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using static WinLock.NativeMethods;

namespace WinLock.ProcessProtection
{
	public static class ProcessProtect
	{
		public static IdentityReference Everyone { get { return GetIdentityReference(WellKnownSidType.WorldSid); } }

		public static IdentityReference GetIdentityReference(WellKnownSidType sidType)
		{
			SecurityIdentifier sid = new SecurityIdentifier(sidType, null);
			return ((NTAccount)sid.Translate(typeof(NTAccount)));
		}

		public static Thread Start()
		{
			Thread t = new Thread(Run);
			t.Start();
			return t;
		}

		private static void Run()
		{
			try
			{
				IntPtr currentProcess = GetCurrentProcess();
				bool success = DuplicateHandle(currentProcess, currentProcess, currentProcess, out IntPtr handle);
				while (Thread.CurrentThread.ThreadState == ThreadState.Running)
				{
					if (success)
					{
						try { SetProcessPermissions(handle, false); } catch { success = false; }
					}
					else
					{
						currentProcess = GetCurrentProcess();
						success = DuplicateHandle(currentProcess, currentProcess, currentProcess, out handle);
					}
					Thread.Sleep(2000);
				}
			}
			catch (ThreadAbortException) { }
		}

		private static void SetProcessPermissions(IntPtr rawHandle, bool allowTermination)
		{
			ProcessAccessRights noAccess = ProcessAccessRights.Terminate | ProcessAccessRights.SuspendResume;
			SafeHandle handle = new SafeHandleWrapper(rawHandle);
			ProcessSecurity sec = new ProcessSecurity(handle);
			ProcessAccessRule noTerminateSuspend = new ProcessAccessRule(Everyone, noAccess,
																		 false, InheritanceFlags.None,
																		 PropagationFlags.NoPropagateInherit,
																		 (AccessControlType)Convert.ToInt32(!allowTermination));
			sec.AddAccessRule(noTerminateSuspend);
			sec.SaveChanges(handle);
		}
	}
}
