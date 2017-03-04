using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WinLock
{
	public class LockScreenForm : Form
	{

		public event PreAttemptUnlockHandler PreAttemptUnlock;

		public delegate void PreAttemptUnlockHandler(object sender, EventArgs e);

		private bool AllowClose = Program.Debug; // yea i know

		public LockScreenForm(Size size, bool createControls = true)
		{
			if (!Program.Debug)
			{
				ProcessModule objCurrentModule = Process.GetCurrentProcess().MainModule;
				objKeyboardProcess = new LowLevelKeyboardProc(captureKey);
				ptrHook = SetWindowsHookEx(13, objKeyboardProcess, GetModuleHandle(objCurrentModule.ModuleName), 0);
			}

			this.BackColor = Properties.Settings.Default.LockScreenBgColor;
			this.Size = size;
			this.FormClosing += LockScreenForm_FormClosing;
			this.KeyDown += LockScreenForm_KeyDown;
			this.StartPosition = FormStartPosition.Manual;
			this.FormBorderStyle = FormBorderStyle.None;
			this.WindowState = FormWindowState.Maximized;
			if (Properties.Settings.Default.Semitransparent)
			{
				this.Opacity = 0.8;
			}
			else
			{
				LoadWallpaper();
			}
			this.Icon = SystemIcons.Application;
			this.TopMost = true;
			if (createControls) CreateControls();
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int SystemParametersInfo(UInt32 uAction, int uParam, string lpvParam, int fuWinIni);

		private void LoadWallpaper()
		{
            const UInt32 SPI_GETDESKWALLPAPER = 0x73;
			const int MAX_PATH = 260;
			string currentWallpaper = new string('\0', MAX_PATH);
			SystemParametersInfo(SPI_GETDESKWALLPAPER, currentWallpaper.Length, currentWallpaper, 0);
			try
			{
				this.BackgroundImage = Image.FromFile(currentWallpaper.Substring(0, currentWallpaper.IndexOf('\0')));
				this.BackgroundImageLayout = ImageLayout.Stretch;
			}
			catch (Exception)
			{
				return;
			}
	}

	private void LockScreenForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.L)
			{
				if (PreAttemptUnlock != null)
				{
					PreAttemptUnlock(sender, e);
				}
			}
		}

		private void CreateControls()
		{
			Label uiLabel = CreateLabelCentered(Properties.Resources.UIText, (Height / 3) - 25, 24);
			String lowerLabelText = Properties.Resources.UITextLower.Replace("{0}", Environment.UserDomainName);
			lowerLabelText = lowerLabelText.Replace("{1}", Environment.UserName);
			Label uiLabelLower = CreateLabelCentered(lowerLabelText, (Height / 3) + 25, 14);
			Controls.Add(uiLabel);
			Controls.Add(uiLabelLower);
		}

		private Label CreateLabelCentered(String text, int top, int fontSize)
		{
			int leftValue = 10;
			Label textLabel = new Label();
			textLabel.BackColor = Color.Transparent;
			textLabel.Text = text;
			textLabel.Font = new Font("Segoe UI", fontSize);
			textLabel.ForeColor = Properties.Settings.Default.LockScreenForeTextColor;
			textLabel.AutoSize = false;
			textLabel.TextAlign = ContentAlignment.MiddleCenter;
			textLabel.Dock = DockStyle.None;
			textLabel.Left = leftValue;
			textLabel.Top = top;
			textLabel.Width = Width - leftValue;
			textLabel.Height = textLabel.PreferredHeight;
			return textLabel;
		}

		public void Unlock()
		{
			AllowClose = true;
			Close();
		}

		private void LockScreenForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!AllowClose) e.Cancel = true;
		}
		
		#region "Disable WinKey, AltTab, CtrlEsc"
		// Structure contain information about low-level keyboard input event 
		[StructLayout(LayoutKind.Sequential)]
		private struct KBDLLHOOKSTRUCT
		{
			public Keys key;
			public int scanCode;
			public int flags;
			public int time;
			public IntPtr extra;
		}
		//System level functions to be used for hook and unhook keyboard input  
		private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyboardProc callback, IntPtr hMod, uint dwThreadId);
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool UnhookWindowsHookEx(IntPtr hook);
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wp, IntPtr lp);
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string name);
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern short GetAsyncKeyState(Keys key);
		//Declaring Global objects     
		private IntPtr ptrHook;
		private LowLevelKeyboardProc objKeyboardProcess;

		private IntPtr captureKey(int nCode, IntPtr wp, IntPtr lp)
		{
			if (nCode >= 0)
			{
				KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));

				// Disabling Windows keys 

				if (objKeyInfo.key == Keys.RWin || objKeyInfo.key == Keys.LWin || objKeyInfo.key == Keys.Tab && HasAltModifier(objKeyInfo.flags) || objKeyInfo.key == Keys.Escape && (ModifierKeys & Keys.Control) == Keys.Control)
				{
					return (IntPtr)1; // if 0 is returned then All the above keys will be enabled
				}
			}
			return CallNextHookEx(ptrHook, nCode, wp, lp);
		}

		bool HasAltModifier(int flags)
		{
			return (flags & 0x20) == 0x20;
		}

#endregion
	}
}
