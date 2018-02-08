using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace SimpleWinceGuiAutomation.Wince
{
    internal class PInvoke
    {
        private static int _getControlNameMessage = 0;
        public static int GetControlNameMessage
        {
            get { return _getControlNameMessage; }
        }

        static PInvoke()
        {
            _getControlNameMessage = (int)RegisterWindowMessage("WM_GETCONTROLNAME");
        }


        [Flags]
        public enum GetWindowFlags
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
        }

        public enum MemUsageFlags
        {
            MEM_COMMIT = 0x1000,
            MEM_RESERVE = 0x2000,
            MEM_DECOMMIT = 0x4000,
            MEM_RELEASE = 0x8000
        }

        [Flags]
        public enum WindowStyles : uint
        {
            WS_OVERLAPPED = 0x00000000,
            WS_POPUP = 0x80000000,
            WS_CHILD = 0x40000000,
            WS_MINIMIZE = 0x20000000,
            WS_VISIBLE = 0x10000000,
            WS_DISABLED = 0x08000000,
            WS_CLIPSIBLINGS = 0x04000000,
            WS_CLIPCHILDREN = 0x02000000,
            WS_MAXIMIZE = 0x01000000,
            WS_BORDER = 0x00800000,
            WS_DLGFRAME = 0x00400000,
            WS_VSCROLL = 0x00200000,
            WS_HSCROLL = 0x00100000,
            WS_SYSMENU = 0x00080000,
            WS_THICKFRAME = 0x00040000,
            WS_GROUP = 0x00020000,
            WS_TABSTOP = 0x00010000,

            WS_MINIMIZEBOX = 0x00020000,
            WS_MAXIMIZEBOX = 0x00010000,

            WS_CAPTION = WS_BORDER | WS_DLGFRAME,
            WS_TILED = WS_OVERLAPPED,
            WS_ICONIC = WS_MINIMIZE,
            WS_SIZEBOX = WS_THICKFRAME,
            WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,

            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
            WS_CHILDWINDOW = WS_CHILD,

            //Extended Window Styles

            WS_EX_DLGMODALFRAME = 0x00000001,
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            WS_EX_TOPMOST = 0x00000008,
            WS_EX_ACCEPTFILES = 0x00000010,
            WS_EX_TRANSPARENT = 0x00000020,

            //#if(WINVER >= 0x0400)

            WS_EX_MDICHILD = 0x00000040,
            WS_EX_TOOLWINDOW = 0x00000080,
            WS_EX_WINDOWEDGE = 0x00000100,
            WS_EX_CLIENTEDGE = 0x00000200,
            WS_EX_CONTEXTHELP = 0x00000400,

            WS_EX_RIGHT = 0x00001000,
            WS_EX_LEFT = 0x00000000,
            WS_EX_RTLREADING = 0x00002000,
            WS_EX_LTRREADING = 0x00000000,
            WS_EX_LEFTSCROLLBAR = 0x00004000,
            WS_EX_RIGHTSCROLLBAR = 0x00000000,

            WS_EX_CONTROLPARENT = 0x00010000,
            WS_EX_STATICEDGE = 0x00020000,
            WS_EX_APPWINDOW = 0x00040000,

            WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
            WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST),

            //#endif /* WINVER >= 0x0400 */

            //#if(WIN32WINNT >= 0x0500)

            WS_EX_LAYERED = 0x00080000,

            //#endif /* WIN32WINNT >= 0x0500 */

            //#if(WINVER >= 0x0500)

            WS_EX_NOINHERITLAYOUT = 0x00100000, // Disable inheritence of mirroring by children
            WS_EX_LAYOUTRTL = 0x00400000, // Right to left mirroring

            //#endif /* WINVER >= 0x0500 */

            //#if(WIN32WINNT >= 0x0500)

            WS_EX_COMPOSITED = 0x02000000,
            WS_EX_NOACTIVATE = 0x08000000

            //#endif /* WIN32WINNT >= 0x0500 */

        }

        [FlagsAttribute]
        public enum PageAccessFlags
        {
            PAGE_READONLY = 0x02,
            PAGE_READWRITE = 0x04,
            PAGE_EXECUTE = 0x10,
            PAGE_EXECUTE_READ = 0x20,
            PAGE_EXECUTE_READWRITE = 0x40,
            PAGE_GUARD = 0x100,
            PAGE_NOACCESS = 0x01,
            PAGE_NOCACHE = 0x200,
            PAGE_PHYSICAL = 0x400
        }

        public const int GWL_STYLE = (-16);

        public static int WM_LBUTTONDOWN = 0x0201;
        public static int WM_LBUTTONUP = 0x0202;
        public static int BM_GETCHECK = 0x00F0;
        public static int BM_SETCHECK = 0x00F1;
        public static int WM_SETTEXT = 0x000C;
        public static int WM_KILLFOCUS = 0x0008;
        public static int WM_SETFOCUS = 0x0007;
        public static int WM_IME_SETCONTEXT = 0x0281;

        public static long WS_DISABLED = 0x08000000L;
        public static long WS_HSCROLL = 0x00100000L;
        public static long WS_VSCROLL = 0x00200000L;
        public static long WS_VISIBLE = 0x10000000L;

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern IntPtr GetFocus();
        [DllImport("coredll.dll", SetLastError= true)]
        public static extern IntPtr SetFocus(IntPtr hWnd);
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);
        [DllImport("coredll.dll", SetLastError = true)]
        public static extern IntPtr GetActiveWindow();
        [DllImport("coredll.dll", SetLastError=true)]
        public static extern ushort GetWindowThreadProcessId(IntPtr hWnd, ref IntPtr lpdwProcessId);

        [DllImport("coredll.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("coredll.dll")]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("coredll.dll", SetLastError=true)]
        public static extern int GetClipBox(IntPtr hdc, out RECT lprc);

        [DllImport("coredll.dll", SetLastError=true)]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("coredll.dll")]
        public static extern UInt32 RegisterWindowMessage(String lpString);

        [DllImport("coredll.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hwnd, uint relationship);

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("coredll.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetClassName(IntPtr hwnd, StringBuilder windowClass, int maxText);

        [DllImport("coredll.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("coredll.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("coredll.dll")]
        public static extern bool SetWindowText(IntPtr hwnd, string text);

        [DllImport("coredll.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("coredll.dll")]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("coredll.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowEnabled(IntPtr hWnd);

        [DllImport("coredll.dll")]
        public static extern bool IsWindowVisible(IntPtr hwnd);

        [DllImport("coredll.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        public static IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, StringBuilder lParam)
        {
            int size = lParam.Capacity;
            IntPtr lpv = VirtualAlloc(IntPtr.Zero, 2*1024*1024, MemUsageFlags.MEM_RESERVE, PageAccessFlags.PAGE_NOACCESS);
            IntPtr pbuffer = VirtualAlloc(lpv, 65536, MemUsageFlags.MEM_COMMIT, PageAccessFlags.PAGE_READWRITE);
            IntPtr res = SendMessage(hWnd, Msg, wParam, pbuffer);
            var content = new byte[size*2];
            uint nbBytesRead = 0;
            ReadProcessMemory((uint) Process.GetCurrentProcess().Id, pbuffer, content, (uint) content.Length,
                              ref nbBytesRead);
            VirtualFree(lpv, 65536, MemUsageFlags.MEM_RELEASE);
            lParam.Append(Encoding.Unicode.GetString(content, 0, content.Length));
            return res;
        }

        [DllImport("Coredll.dll")]
        public static extern IntPtr VirtualAlloc(IntPtr lpAddress, UInt32
                                                                       dwSize, MemUsageFlags flAllocationType,
                                                 PageAccessFlags
                                                     flProtect);

        [DllImport("Coredll.dll")]
        public static extern bool VirtualFree(IntPtr lpAddress, UInt32 dwSize,
                                              MemUsageFlags dwFreeType);


        //Flags

        [DllImport("Coredll.dll")]
        public static extern bool ReadProcessMemory(uint hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize,
                                                    ref uint lpNumberOfBytesRead);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left; // x position of upper-left corner
            public int Top; // y position of upper-left corner
            public int Right; // x position of lower-right corner
            public int Bottom; // y position of lower-right corner
        }

        public enum ClipBoxComplexity : int
        {
            Error = 0x0,
            NullRegion = 0x1,
            SimpleRegion = 0x2,
            ComplexRegion = 0x3
        }
    }
}