using System;
using System.Collections.Generic;
using System.Text;
using SimpleWinceGuiAutomation.Core;

namespace SimpleWinceGuiAutomation.Wince
{
    internal class WinceComponentsFinder
    {
        public List<WinComponent> ListChilds(IntPtr handle)
        {
            var childs = new List<WinComponent>();
            IntPtr hwndCur = PInvoke.GetWindow(handle, (uint)PInvoke.GetWindowFlags.GW_CHILD);

            RecurseFindWindow(hwndCur, (c) => childs.Add(c));

            childs.Sort((x,y) => {
                var r = IntCompare(x.Top, y.Top);
                return r == 0 ? IntCompare(x.Left, y.Left) : r;
            });
            
            return childs;
        }

        public static int IntCompare(int x, int y)
        {
            return
                x < y ? -1 :
                x > y ? 1 :
                0;
        }

        private static WinComponent FromHandle(IntPtr hwndCur)
        {
            var chArWindowClass = new StringBuilder(257);
            PInvoke.GetClassName(hwndCur, chArWindowClass, 256);
            var strWndClass = chArWindowClass.ToString();

            var length = PInvoke.GetWindowTextLength(hwndCur);
            var sb = new StringBuilder(length + 1);
            PInvoke.GetWindowText(hwndCur, sb, sb.Capacity);

            PInvoke.RECT rct;
            PInvoke.GetWindowRect(hwndCur, out rct);

            var style = PInvoke.GetWindowLong(hwndCur, PInvoke.GWL_STYLE);
            var res = new WinComponent(strWndClass, sb.ToString(), hwndCur, rct.Left, rct.Top, style);

            return res;
        }

        private static void RecurseFindWindow(IntPtr hWndParent, Action<WinComponent> child)
        {
            if (hWndParent == IntPtr.Zero)
                return;
            var pointers = new List<IntPtr>();

            IntPtr hwndCur = PInvoke.GetWindow(hWndParent, (uint)PInvoke.GetWindowFlags.GW_HWNDFIRST);
            do
            {
                pointers.Add(hwndCur);
                var res = FromHandle(hwndCur);

                child(res);

                hwndCur = PInvoke.GetWindow(hwndCur, (uint)PInvoke.GetWindowFlags.GW_HWNDNEXT);
            } while (hwndCur != IntPtr.Zero);

            foreach (var pointer in pointers)
            {
                RecurseFindWindow(PInvoke.GetWindow(pointer, (uint)PInvoke.GetWindowFlags.GW_CHILD), child);
            }
        }

        public static T RecursiveFindWindow<T>(IntPtr x, Func<WinComponent,T> f, Func<T,T,T> g)
            where T : class
        {
            var list = new List<T>();

            if (x == IntPtr.Zero)
                return null;
            
            var r = x;
            var stk = new Stack<T>();
            var node = r;

            while (true)
            {
                var n = f(FromHandle(node));

                stk.Push(n);
                var node_child = PInvoke.GetWindow(node, (uint)PInvoke.GetWindowFlags.GW_CHILD);
                if (node_child != IntPtr.Zero)
                {
                    node = node_child; // walk down
                }
                else
                {
                    IntPtr node_next;
                    while (true)
                    {
                        if (node == r)
                        {
                            var res = stk.Pop();
                            return res;
                        }
                        node_next = PInvoke.GetWindow(node, (uint)PInvoke.GetWindowFlags.GW_HWNDNEXT);
                        if (node_next != IntPtr.Zero)
                        {
                            break;
                        }
                        var xn = stk.Pop();

                        var e = stk.Pop();
                        e = g(e, xn);
                        stk.Push(e);
                        node = PInvoke.GetParent(node); // walk up
                    }
                    var xn0 = stk.Pop();
                    var e1 = stk.Pop();
                    e1 = g(e1, xn0);
                    stk.Push(e1);

                    node = node_next; // PInvoke.GetWindow(node, (uint)PInvoke.GetWindowFlags.GW_HWNDNEXT); // ... and right
                }
            }
        }
    }
}