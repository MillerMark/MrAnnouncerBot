#nullable  enable
using System;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Timers;
using System.Windows;

namespace DHDM
{

    public class MyMouseEventArgs
    {
        public Point Position { get; set; }
        public MouseButtonState LeftButton { get; set; }
        public MouseButtonState MiddleButton { get; set; }
        public MouseButtonState RightButton { get; set; }
        public Int16 WheelSpinDelta { get; set; }

        public MyMouseEventArgs()
        {
            
        }
    }

    public static class GlobalHooks
    {
        static Point savedMousePosition;
        public static Point SavedMousePosition { get => savedMousePosition; }
        public static event EventHandler<bool>? ControlKeyStateChanged;
        public static event EventHandler<MyMouseEventArgs> MouseThingHappened;
        static System.Timers.Timer keyProcessingTimer = new(50);
        static bool? ctrlKeyHitOrReleased = null;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;

        private const int WH_MOUSE_LL = 14;

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData, flags, time;
            public IntPtr dwExtraInfo;
        }


        private static IntPtr _keyboardHookID = IntPtr.Zero;
        private static IntPtr _mouseHookID = IntPtr.Zero;

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };


        public static Point GetMousePosition()
        {
            var w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        public static void SaveMousePosition()
        {
            savedMousePosition = GetMousePosition();
        }


        private static IntPtr SetHook(LowLevelProc proc, int idHook)
        {
            using (Process curProcess = Process.GetCurrentProcess())
                using (ProcessModule? curModule = curProcess.MainModule)
                {
                    nint hMod = 0;
                    if (curModule != null)
                        hMod = GetModuleHandle(curModule.ModuleName);

                    return SetWindowsHookEx(idHook, proc, hMod, 0);
                }
        }

        public static void Start()
        {
            keyProcessingTimer.Elapsed += KeyProcessingTimer_Elapsed;
            _keyboardHookID = SetHook(KeyboardHookCallback, WH_KEYBOARD_LL);
            _mouseHookID = SetHook(MouseHookCallback, WH_MOUSE_LL);
        }

        public static void Stop()
        {
            keyProcessingTimer.Stop();
            UnhookWindowsHookEx(_mouseHookID);
            UnhookWindowsHookEx(_keyboardHookID);
        }

        private static IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            object? msLlHookStruct = Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
            if (msLlHookStruct != null)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)msLlHookStruct;
                MyMouseEventArgs myMouseEventArgs = new MyMouseEventArgs();
                myMouseEventArgs.Position = new Point(hookStruct.pt.x, hookStruct.pt.y);
                myMouseEventArgs.LeftButton = Mouse.LeftButton;
                myMouseEventArgs.MiddleButton = Mouse.MiddleButton;
                myMouseEventArgs.RightButton = Mouse.RightButton;

                if (nCode >= 0 && (MouseMessages)wParam == MouseMessages.WM_MOUSEWHEEL)
                {
                    uint highWord = hookStruct.mouseData >> 16;
                    myMouseEventArgs.WheelSpinDelta = (Int16)highWord;
                    uint lowWord = hookStruct.mouseData & 0xffff;
                    //System.Diagnostics.Debugger.Break();
                }
                MouseThingHappened?.Invoke(null, myMouseEventArgs);
            }


            return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
        }

        private static void KeyProcessingTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            keyProcessingTimer.Stop();
            if (ctrlKeyHitOrReleased.HasValue)
                ControlKeyStateChanged?.Invoke(null, ctrlKeyHitOrReleased.Value);
        }

        private static IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            ctrlKeyHitOrReleased = null;
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_KEYUP))
            {
                keyProcessingTimer.Stop();
                int vkCode = Marshal.ReadInt32(lParam);
                Key wpfKey = KeyInterop.KeyFromVirtualKey(vkCode);
                ctrlKeyHitOrReleased = wpfKey == Key.LeftCtrl || wpfKey == Key.RightCtrl;
            }

            nint result = CallNextHookEx(_keyboardHookID, nCode, wParam, lParam);
            if (ctrlKeyHitOrReleased.HasValue)
                keyProcessingTimer.Start();
            return result;
        }

        private delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);
    }
}
