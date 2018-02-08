using System;
using SimpleWinceGuiAutomation.Wince;
using SimpleWinceGuiAutomation.Core;

namespace SimpleWinceGuiAutomation.Components
{
    public class WinceButton : WinceComponent
    {        
        public WinceButton(IntPtr handle) : base(handle) { }

        public static bool Check(WinComponent e)
        {
            return e.Class.ToLower().Contains("button") && !WinceCheckBox.Check(e) && !WinceRadio.Check(e);
        }

        public override string TagName()
        {
            return "button";
        }
        public override string AttributeValue(string name)
        {
            return null;
        }

        public void Click()
        {
            WindowHelper.Click(Handle, null);
        }
        public void Click(SimpleWinceGuiAutomation.Wince.WindowHelper.Location loc)
        {
            WindowHelper.Click(Handle, loc);
        }
        public String Text
        {
            get { return WindowHelper.GetText(Handle); }
        }
    }
}