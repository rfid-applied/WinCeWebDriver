using System;
using SimpleWinceGuiAutomation.Wince;
using SimpleWinceGuiAutomation.Core;

namespace SimpleWinceGuiAutomation.Components
{
    public class WinceLabel : WinceComponent
    {
        public WinceLabel(IntPtr ptr) : base(ptr) { }

        public static bool Check(WinComponent c)
        {
            return c.Class.ToLower().Equals("static");
        }

        public override string TagName()
        {
            return "label";
        }
        public override string AttributeValue(string name)
        {
            return name == "text"? Text : null;
        }

        public String Text
        {
            get { return WindowHelper.GetText(Handle); }
        }

        public void Click()
        {
            WindowHelper.Click(Handle, null);
        }

        public void Click(SimpleWinceGuiAutomation.Wince.WindowHelper.Location loc)
        {
            WindowHelper.Click(Handle, loc);
        }
    }
}