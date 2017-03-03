using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinLock.CredentialDialog
{
	public enum LockScreenError
	{
		None = 0x00,
		AuthenticationError = 0x01,
		UserCancelled = 0x02,
	}
}
