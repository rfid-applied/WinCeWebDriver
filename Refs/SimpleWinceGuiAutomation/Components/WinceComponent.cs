using System;
using SimpleWinceGuiAutomation.Wince;
using System.Collections.Generic;

namespace SimpleWinceGuiAutomation.Components
{
    public abstract class WinceComponent
    {
        protected IntPtr Handle { get; private set; }

        public string ID { get { return ((long)Handle).ToString("X"); } }

        protected WinceComponent(IntPtr handle)
        {
            Handle = handle;
            Children = new List<WinceComponent>();
        }

        public List<WinceComponent> Children { get; set; }
        public WinceComponent Parent { get; set; }

        public abstract string TagName();
        public abstract string AttributeValue(string name);

        public WindowHelper.Size Size
        {
            get
            {
                return WindowHelper.GetSize(Handle);
            }
        }

        public WindowHelper.Location Location
        {
            get
            {
                return WindowHelper.GetLocation(Handle);
            }
        }

        public bool Enabled
        {
            get { return PInvoke.IsWindowEnabled(Handle); }
        }

        public bool VisibleOnScreen()
        {
            return WindowHelper.ElementVisibleOnScreen(Handle);
        }
        public void ScrollIntoView()
        {
            WindowHelper.ScrollIntoView(Handle);
        }
    }
}
