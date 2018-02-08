using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleWinceGuiAutomation.Wince;

// NOTE: adapated from OpenNETCF
namespace OpenNETCF.Windows.Forms
{
    /// <summary>
    /// Enumerates all windows associated with a thread. In the future (for CF2.0), it should be based on the EnumThreadWindows function.
    /// </summary>
    internal class ThreadWindows
    {
        IntPtr _parent;
        internal ThreadWindows previousThreadWindows;

        static string PrintMask(SimpleWinceGuiAutomation.Wince.PInvoke.WindowStyles mask)
        {
            var str = mask.ToString();
            return str;
        }

        #region P/Invokes

        private const uint GW_HWNDFIRST = 0;
        private const uint GW_HWNDNEXT = 2;

        [DllImport("coredll.dll")]
        static extern IntPtr EnableWindow(IntPtr hWnd, bool enable);

        [DllImport("coredll.dll")]
        static extern bool IsWindowEnabled(IntPtr hWnd);

        [DllImport("coredll.dll")]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("coredll.dll")]
        static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("coredll.dll")]
        static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("coredll.dll")]
        static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, IntPtr processId);
        #endregion

        /// <summary>
        /// Creates a new <see cref="ThreadWindows"/> object for a specific window handle.
        /// </summary>
        /// <param name="parent"></param>
        public ThreadWindows(IntPtr parent)
        {
            _windows = new IntPtr[16];
            _parent = parent;
            EnumThreadWindows();
        }

        /// <summary>
        /// Enables/Disables thread windows except parent window.
        /// </summary>
        /// <param name="state"></param>
        public void Enable(bool state)
        {
            foreach (IntPtr window in _windows)
            {
                EnableWindow(window, state);
            }
        }

        public IEnumerable<IntPtr> GetWindows()
        {
            _windows = new IntPtr[16];
            _windowCount = 0;
            EnumThreadWindows();
            return _windows.Take(_windowCount);
        }

        private IntPtr GetTopWindow(IntPtr hwnd)
        {
            IntPtr newHwnd;
            while (true)
            {
                // check to see what kind of window it is
                var flags = (PInvoke.WindowStyles)SimpleWinceGuiAutomation.Wince.PInvoke.GetWindowLong(hwnd, -16/*GWL_STYLE*/);

                // thanks to: https://stackoverflow.com/questions/34462445/fullscreen-vs-borderless-window
                // Windows are either WS_OVERLAPPED, WS_POPUP, or WS_CHILD. These three flags can't be combined with each other,
                // but they can be combined with other WS_XXXX flags.
                // Top windows are either WS_OVERLAPPED or WS_POPUP
                if ((flags & PInvoke.WindowStyles.WS_OVERLAPPED) != 0 || (flags & PInvoke.WindowStyles.WS_POPUP) != 0)
                    return hwnd;

                newHwnd = GetParent(hwnd);
                if (newHwnd == IntPtr.Zero) return hwnd;
                hwnd = newHwnd;
            }
        }

        public IntPtr GetForegroundForm()
        {
            int threadId = GetWindowThreadProcessId(_parent, IntPtr.Zero);

            var hWnd = SimpleWinceGuiAutomation.Wince.PInvoke.GetForegroundWindow();
            if (threadId != GetWindowThreadProcessId(hWnd, IntPtr.Zero))
                return IntPtr.Zero; // this app is not under focus!

            hWnd = GetTopWindow(hWnd);
            return hWnd;
        }

        private void EnumThreadWindows()
        {
            ArrayList al = new ArrayList();
            IntPtr hwnd = GetTopWindow(_parent);
            int threadId = GetWindowThreadProcessId(_parent, IntPtr.Zero);
            hwnd = GetWindow(hwnd, GW_HWNDFIRST);
            while (true)
            {
                // ignores parent window
                if (hwnd != _parent)
                {
                    if (threadId == GetWindowThreadProcessId(hwnd, IntPtr.Zero) && IsWindowEnabled(hwnd) && IsWindowVisible(hwnd))
                    {
                        if (_windows.Length == _windowCount)
                        {
                            IntPtr[] ar = new IntPtr[this._windowCount * 2];
                            Array.Copy(_windows, 0, ar, 0, _windowCount);
                            _windows = ar;
                        }
                        _windows[_windowCount++] = hwnd;
                    }
                }

                hwnd = GetWindow(hwnd, GW_HWNDNEXT);
                if (hwnd == IntPtr.Zero) break;
            }

            hwnd = IntPtr.Zero;
        }

        int _windowCount = 0;
        IntPtr[] _windows;
    }
}
