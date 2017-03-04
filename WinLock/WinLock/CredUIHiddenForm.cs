using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinLock
{
	internal class CredUIHiddenForm : Form
	{
		internal CredUIHiddenForm()
		{
			this.Left = Screen.PrimaryScreen.Bounds.Left / 2;
			this.Top = Screen.PrimaryScreen.Bounds.Top / 4;
			this.StartPosition = FormStartPosition.CenterScreen;
			this.TopMost = true;
		}
	}
}
