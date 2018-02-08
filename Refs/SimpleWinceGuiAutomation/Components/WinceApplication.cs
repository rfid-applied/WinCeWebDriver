using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace SimpleWinceGuiAutomation.Components
{
    public class WinceApplication
    {
        private readonly Process process;

        public WinceApplication(IntPtr handle, Process process)
        {
            this.process = process;
            MainWindow = new WinceWindow(handle);
            Windows = new OpenNETCF.Windows.Forms.ThreadWindows(handle);
        }

        public WinceWindow MainWindow { get; private set; }
        OpenNETCF.Windows.Forms.ThreadWindows Windows { get; set; }

        public WinceWindow CurrentWindow
        {
            get
            {
                var foreground = Windows.GetForegroundForm();
                if (foreground == IntPtr.Zero)
                    return MainWindow; // FIXME: it means that app is not focused!

                return new WinceWindow(foreground);
            }
        }

        public void Kill()
        {
            process.Kill();
        }

        public void DoStuff()
        {
            var ptrs = Windows.GetWindows();
            foreach (var pt in ptrs)
            {
                Console.WriteLine("{0:X}", pt);
            }
        }
    }
}