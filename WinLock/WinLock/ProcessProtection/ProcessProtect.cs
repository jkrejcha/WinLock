using System;
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

		public static IdentityReference GetIdentityReference(WellKnownSidType SidType)
		{
			SecurityIdentifier sid = new SecurityIdentifier(SidType, null);
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
				IntPtr handle;
				bool success = DuplicateHandle(currentProcess, currentProcess, currentProcess, out handle);
				while (true)
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

		private static void SetProcessPermissions(IntPtr rawHandle, bool AllowTermination)
		{
			ProcessAccessRights noAccess = ProcessAccessRights.Terminate | ProcessAccessRights.SuspendResume;
			SafeHandle handle = new SafeHandlerWrapper(rawHandle);
			ProcessSecurity sec = new ProcessSecurity(handle);
			ProcessAccessRule noTerminateSuspend = new ProcessAccessRule(Everyone, noAccess,
																		 false, InheritanceFlags.None,
																		 PropagationFlags.NoPropagateInherit,
																		 (AccessControlType)Convert.ToInt32(!AllowTermination));
			sec.AddAccessRule(noTerminateSuspend);
			sec.SaveChanges(handle);
		}
	}
}
