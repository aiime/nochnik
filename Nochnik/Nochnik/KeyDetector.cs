using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Nochnik
{
    class KeyDetector
    {
        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x0100;
        static LowLevelKeyboardProc proc = HookCallback;
        static IntPtr hookID = IntPtr.Zero;
        delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(int idHook,LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        static int firstDigit;
        static int secondDigit;
        static bool ctrlAltMode;
        static readonly AlarmClock alarmClock = new AlarmClock();
        const int CTRL_ALT = 164;
        const int ZERO = 48;
        const int NINE = 57;
        const int TILDE = 192;
        static bool firstDigitSelected;

        public static void Start(Form mainForm)
        {
            hookID = SetHook(proc);
            Application.Run(mainForm);
            UnhookWindowsHookEx(hookID);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int currentKeyCode = Marshal.ReadInt32(lParam);
                Console.WriteLine(currentKeyCode);

                if (firstDigitSelected && currentKeyCode >= ZERO && currentKeyCode <= NINE)
                {
                    secondDigit = currentKeyCode - 48;
                    alarmClock.AddAlarm(firstDigit * 10 + secondDigit);

                    firstDigit = 0;
                    secondDigit = 0;
                    ctrlAltMode = false;
                    firstDigitSelected = false;
                }
                else
                {
                    firstDigitSelected = false;
                }

                if (ctrlAltMode && currentKeyCode >= ZERO && currentKeyCode <= NINE)
                {
                    firstDigit = currentKeyCode - 48;
                    firstDigitSelected = true;
                }

                if (ctrlAltMode && currentKeyCode == TILDE)
                {
                    alarmClock.RemoveAlarms();
                }

                if (currentKeyCode == CTRL_ALT)
                {
                    ctrlAltMode = true;
                }
                else
                {
                    ctrlAltMode = false;
                }
            }

            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }
    }
}