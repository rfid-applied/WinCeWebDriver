using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using SimpleWinceGuiAutomation.Components;

namespace SimpleWinceGuiAutomation
{
    public class WinceApplicationFactory
    {
        public static WinceApplication StartFromTypeInApplication<T>()
        {
            Assembly assemblyToTest = typeof (T).Assembly;
            string theDirectory = Path.GetDirectoryName(assemblyToTest.GetName().CodeBase.Replace("file:///", ""));
            string applicationName = Path.GetFileName(assemblyToTest.GetName().CodeBase.Replace("file:///", ""));
            Process p = Process.Start(theDirectory + @"\" + applicationName, "");

            for (int ix = 0; ix < 500; ++ix)
            {
                Thread.Sleep(100);
                p.Refresh();
                if (p.MainWindowHandle != IntPtr.Zero) break;
            }
            return new WinceApplication(p.MainWindowHandle, p);
        }

        public static WinceApplication StartFromPath(string url, string args)
        {
            var path = url;
            Process p = Process.Start(path, args == null? "" : args);

            for (int ix = 0; ix < 500; ++ix)
            {
                Thread.Sleep(100);
                p.Refresh();
                if (p.MainWindowHandle != IntPtr.Zero) break;
            }
            return new WinceApplication(p.MainWindowHandle, p);
        }
    }
}