using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace WinLock.ProcessProtection
{
	class SafeHandleWrapper : SafeHandleZeroOrMinusOneIsInvalid
	{
		public SafeHandleWrapper(IntPtr existingHandle) : base(true)
		{
			SetHandle(existingHandle);
		}

		protected override bool ReleaseHandle()
		{
			return true;
		}
	}
}
