#nullable  enable
using System;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Timers;

namespace DHDM
{
    public static class GlobalHooks
    {
        public static event EventHandler<bool>? ControlKeyStateChanged;
        static System.Timers.Timer keyProcessingTimer = new(50);
        static bool? ctrlKeyHitOrReleased = null;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private static IntPtr _hookID = IntPtr.Zero;

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule? curModule = curProcess.MainModule)
            {
                nint hMod = 0;
                if (curModule != null)
                    hMod = GetModuleHandle(curModule.ModuleName);

                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, hMod, 0);
            }
        }

        public static void Start()
        {
            keyProcessingTimer.Elapsed += KeyProcessingTimer_Elapsed;
            LowLevelKeyboardProc _proc;
            _proc = HookCallback;
            _hookID = SetHook(_proc);
        }

        private static void KeyProcessingTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            keyProcessingTimer.Stop();
            if (ctrlKeyHitOrReleased.HasValue)
                ControlKeyStateChanged?.Invoke(null, ctrlKeyHitOrReleased.Value);
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            ctrlKeyHitOrReleased = null;
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_KEYUP))
            {
                keyProcessingTimer.Stop();
                int vkCode = Marshal.ReadInt32(lParam);
                Key wpfKey = KeyInterop.KeyFromVirtualKey(vkCode);
                ctrlKeyHitOrReleased = wpfKey == Key.LeftCtrl || wpfKey == Key.RightCtrl;
            }

            nint result = CallNextHookEx(_hookID, nCode, wParam, lParam);
            if (ctrlKeyHitOrReleased.HasValue)
                keyProcessingTimer.Start();
            return result;
        }

        private delegate IntPtr LowLevelKeyboardProc(
        int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
        LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public static void Stop()
        {
            keyProcessingTimer.Stop();
            UnhookWindowsHookEx(_hookID);
        }
    }
}
