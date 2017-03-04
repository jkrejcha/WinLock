using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;

namespace WinLock.ProcessProtection
{
	public class ProcessSecurity : NativeObjectSecurity
	{
		public ProcessSecurity(SafeHandle processHandle)
			: base(false, ResourceType.KernelObject, processHandle, AccessControlSections.Access)
		{
			
		}

		public void AddAccessRule(ProcessAccessRule rule)
		{
			base.AddAccessRule(rule);
		}

		// this is not a full impl- it only supports writing DACL changes
		public void SaveChanges(SafeHandle processHandle)
		{
			Persist(processHandle, AccessControlSections.Access);
		}

		public override Type AccessRightType
		{
			get { return typeof(ProcessAccessRights); }
		}

		public override AccessRule AccessRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
		{
			return new ProcessAccessRule(identityReference, (ProcessAccessRights)accessMask, isInherited, inheritanceFlags, propagationFlags, type);
		}

		public override Type AccessRuleType
		{
			get { return typeof(ProcessAccessRule); }
		}

		public override AuditRule AuditRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
		{
			throw new NotImplementedException();
		}

		public override Type AuditRuleType
		{
			get { throw new NotImplementedException(); }
		}
	}

	public class ProcessAccessRule : AccessRule
	{
		public ProcessAccessRule(IdentityReference identityReference, ProcessAccessRights accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
			: base(identityReference, (int)accessMask, isInherited, inheritanceFlags, propagationFlags, type)
		{
		}

		public ProcessAccessRights ProcessAccessRights { get { return (ProcessAccessRights)AccessMask; } }
	}

	[Flags]
	public enum ProcessAccessRights
	{
		StandardRightsRequired = (0x000F0000),
		Delete = (0x00010000), // Required to delete the object. 
		ReadControl = (0x00020000), // Required to read information in the security descriptor for the object, not including the information in the SACL. To read or write the SACL, you must request the ACCESS_SYSTEM_SECURITY access right. For more information, see SACL Access Right. 
		WriteControl = (0x00040000), // Required to modify the DACL in the security descriptor for the object. 
		WriteOwner = (0x00080000), // Required to change the owner in the security descriptor for the object. 

		AllAccess = StandardRightsRequired | Synchronize | 0xFFF, //All possible access rights for a process object.
		/// <summary>
		/// Required to create a process.
		/// </summary>
		CreateProcess = 0x0080, // Required to create a process.
		/// <summary>
		/// Required to create a thread.
		/// </summary>
		CreateThread = 0x0002, // Required to create a thread.
		/// <summary>
		/// Required to duplicate a handle using DuplicateHandle. 
		/// </summary>
		DuplicateHandle = 0x0040,
		/// <summary>
		/// Required to retrieve certain information about a process such as
		/// its token, exit code, and priority class. See OpenProcessToken, 
		/// GetExitCodeProcess, GetPriorityClass, and IsProcessInJob.
		/// </summary>
		QueryInformation = 0x0400,
		QueryLimitedInformation = 0x1000,
		/// <summary>
		/// Required to set certain information about a process, such as its priority class (see SetPriorityClass).
		/// </summary>
		SetInformation = 0x0200,
		/// <summary>
		/// Required to set memory limits using SetProcessWorkingSetSize.
		/// </summary>
		SetQuota = 0x0100,
		/// <summary>
		/// Required to suspend or resume a process. 
		/// </summary>
		SuspendResume = 0x0800,
		/// <summary>
		/// Required to terminate a process using TerminateProcess.
		/// </summary>
		Terminate = 0x0001,
		/// <summary>
		/// Required to perform an operation on the address space of a process (see VirtualProtectEx and WriteProcessMemory).
		/// </summary>
		VMOperation = 0x0008,
		/// <summary>
		/// Required to read memory in a process using ReadProcessMemory. 
		/// </summary>
		VMRead = 0x0010,
		/// <summary>
		/// Required to write to memory in a process using WriteProcessMemory.
		/// </summary>
		VMWrite = 0x0020,
		/// <summary>
		/// Required to wait for the process to terminate using the wait functions.
		/// </summary>
		Synchronize = 0x00100000,
	}
}
