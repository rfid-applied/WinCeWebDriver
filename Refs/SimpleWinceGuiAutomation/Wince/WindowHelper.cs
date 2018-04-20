using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SimpleWinceGuiAutomation.Wince
{
    public static class WindowHelper
    {
        public static String GetText(IntPtr handle)
        {
            var length = PInvoke.GetWindowTextLength(handle);
            var sb = new StringBuilder(length + 1);
            PInvoke.GetWindowText(handle, sb, sb.Capacity);
            return sb.ToString();
        }

        public static void SetText(IntPtr handle, string value)
        {
            PInvoke.SetWindowText(handle, value);
        }

        public static void Click(IntPtr handle, Location location)
        {
            if (location != null)
            {
                var p = new PInvoke.POINT(); // DOESN'T WORK! not getting clicked!
                p.X = location.X;
                p.Y = location.Y;
                PInvoke.ScreenToClient(handle, ref p);
                var lParam = p.X | (p.Y << 16);

                unsafe
                {
                    PInvoke.SendMessage(handle, PInvoke.WM_LBUTTONDOWN, (IntPtr)0x1, new IntPtr(&p));
                    PInvoke.SendMessage(handle, PInvoke.WM_LBUTTONUP, (IntPtr)0x1, new IntPtr(&p));
                }
            }
            else {
                PInvoke.SendMessage(handle, PInvoke.WM_LBUTTONDOWN, (IntPtr)0x1, (IntPtr)0);
                PInvoke.SendMessage(handle, PInvoke.WM_LBUTTONUP, (IntPtr)0x1, (IntPtr)0);
            }
        }

        [DllImport("coredll.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

        private static RECT GetRect(IntPtr handle)
        {
            RECT r;
            if (!GetWindowRect(handle, out r))
            {
                throw new Exception("");
            }
            return r;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public class Location
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        public class Size
        {
            public int Width { get; set; }
            public int Height { get; set; }
        }

        public static Location GetLocation(IntPtr handle)
        {
            var rect = GetRect(handle);
            return new Location { X = rect.left, Y = rect.top };
        }

        public static Size GetSize(IntPtr handle)
        {
            var rect = GetRect(handle);
            return new Size { Height = rect.bottom - rect.top, Width = rect.right - rect.left };
        }

        public static void Focus(IntPtr handle)
        {
            Click(handle, null);
        }

        // get Name property of a control (winforms only)
        // not supported by WinMo controls, always empty
        public static string GetControlName(IntPtr handle)
        {
            var sb = new StringBuilder();
            PInvoke.SendMessage(handle, (int)PInvoke.GetControlNameMessage, (IntPtr)(sb.Capacity * 2), sb);
            var res = sb.ToString();
            return res;
        }

        public static void Scroll(IntPtr hWnd, int vertical, int horizontal)
        {
            const int WM_VSCROLL = 0x115; // Vertical scroll
            const int WM_HSCROLL = 0x0114;
            const int SB_LINELEFT = 0;
            const int SB_LINEUP = 0; // Scrolls one line up
            const int SB_LINEDOWN = 1; // Scrolls one line down
            const int SB_LINERIGHT = 1;

            if (vertical != 0)
            {
                if (vertical > 0)
                    PInvoke.SendMessage(hWnd, WM_VSCROLL, (IntPtr)SB_LINEUP, IntPtr.Zero);
                else
                    PInvoke.SendMessage(hWnd, WM_VSCROLL, (IntPtr)SB_LINEDOWN, IntPtr.Zero);
            }

            if (horizontal != 0)
            {
                if (horizontal > 0)
                    PInvoke.SendMessage(hWnd, WM_HSCROLL, (IntPtr)SB_LINERIGHT, IntPtr.Zero);
                else
                    PInvoke.SendMessage(hWnd, WM_HSCROLL, (IntPtr)SB_LINELEFT, IntPtr.Zero);
            }
        }

        static void ComputeRectRelativeToParent(IntPtr hWnd, IntPtr hwndParent, out PInvoke.RECT clientParent, out PInvoke.RECT client)
        {
            PInvoke.RECT rw, rc;
            PInvoke.GetWindowRect(hWnd, out rw);
            PInvoke.GetClientRect(hWnd, out rc);

            PInvoke.RECT rpw, rpc;
            PInvoke.GetWindowRect(hwndParent, out rpw);
            PInvoke.GetClientRect(hwndParent, out rpc);

            PInvoke.POINT p0 = new PInvoke.POINT()
            {
                X = rw.Left,
                Y = rw.Top
            };
            PInvoke.ScreenToClient(hwndParent, ref p0);
            PInvoke.POINT p1 = new PInvoke.POINT()
            {
                X = rw.Right,
                Y = rw.Bottom
            };
            PInvoke.ScreenToClient(hwndParent, ref p1);

            clientParent = rpc;
            client = new PInvoke.RECT()
            {
                Left = p0.X,
                Top = p0.Y,
                Right = p1.X,
                Bottom = p1.Y
            };
        }

        public static void ScrollIntoView(IntPtr hWnd)
        {
            PInvoke.RECT rc, rpc;

            IntPtr hwndParent = PInvoke.GetParent(hWnd);
            if (hwndParent == IntPtr.Zero)
            {
                // no parent, cannot be scrolled
                return;
            }

            // is the parent window vertically or horizontally scrollable?
            var style = (PInvoke.WindowStyles)PInvoke.GetWindowLong(hwndParent, -16/*GWL_STYLE*/);
            var vscroll = (style & PInvoke.WindowStyles.WS_VSCROLL) != 0;
            var hscroll = (style & PInvoke.WindowStyles.WS_HSCROLL) != 0;

            ComputeRectRelativeToParent(hWnd, hwndParent, out rpc, out rc);

            while (hscroll && rc.Left < rpc.Left)
            {
                Scroll(hwndParent, 0, 1); // move right
                ComputeRectRelativeToParent(hWnd, hwndParent, out rpc, out rc);
            }
            while (vscroll && rc.Top < rpc.Top)
            {
                Scroll(hwndParent, 1, 0); // move up
                ComputeRectRelativeToParent(hWnd, hwndParent, out rpc, out rc);
            }
            while (hscroll && rc.Right > rpc.Right)
            {
                Scroll(hwndParent, 0, -1); // move leftP
                ComputeRectRelativeToParent(hWnd, hwndParent, out rpc, out rc);
            }
            while (vscroll && rc.Bottom > rpc.Bottom)
            {
                Scroll(hwndParent, -1, 0); // move down
                ComputeRectRelativeToParent(hWnd, hwndParent, out rpc, out rc);
            }
        }

        public static bool ElementVisibleOnScreen(IntPtr hWnd)
        {
            IntPtr hdc = PInvoke.GetDC(hWnd);
            PInvoke.RECT r;
            var cb = PInvoke.GetClipBox(hdc, out r);
            var flag = false;

            switch ((PInvoke.ClipBoxComplexity)cb)
            {
                case PInvoke.ClipBoxComplexity.NullRegion:
                    Console.WriteLine("window covered completely");
                    flag = false;
                    break;
                case PInvoke.ClipBoxComplexity.Error:
                    Console.WriteLine("error: {0]", Marshal.GetLastWin32Error());
                    flag = false;
                    break;
                case PInvoke.ClipBoxComplexity.SimpleRegion:
                    PInvoke.RECT rcClient;
                    PInvoke.GetClientRect(hWnd, out rcClient);
                    if (rcClient.Left == r.Left && rcClient.Top == r.Top && rcClient.Right == r.Right && rcClient.Bottom == r.Bottom)
                    {
                        Console.WriteLine("completely uncovered");
                        flag = true;
                    }
                    else
                    {
                        Console.WriteLine("partially covered");
                        flag = false;
                    }
                    break;
                case PInvoke.ClipBoxComplexity.ComplexRegion:
                    Console.WriteLine("partially covered");
                    flag = false;
                    break;
                default:
                    Console.WriteLine("unknown return code {0}", cb);
                    flag = false;
                    break;
            }
            PInvoke.ReleaseDC(hWnd, hdc);
            return flag;
        }

        public static void SuppressSIP()
        {
            var hWnd = PInvoke.FindWindow("SIPWndClass", null);
            if (hWnd == IntPtr.Zero)
                return;
            var flags = (PInvoke.WindowStyles)PInvoke.GetWindowLong(hWnd, -16/*GWL_STYLE*/);
            if ((flags & PInvoke.WindowStyles.WS_VISIBLE) == 0)
                return; // not visible

            var hWndButton = PInvoke.FindWindow("MS_SIPBUTTON", "MS_SIPBUTTON");
            if (hWndButton == IntPtr.Zero)
                return; // no button!

            PInvoke.SipShowIM(PInvoke.SIPF.SIPF_OFF);
            //Click(hWndButton, null);

            flags = (PInvoke.WindowStyles)PInvoke.GetWindowLong(hWnd, -16/*GWL_STYLE*/);
            if ((flags & PInvoke.WindowStyles.WS_VISIBLE) != 0)
            {
                Console.WriteLine("oops");
            }
        }
    }
}