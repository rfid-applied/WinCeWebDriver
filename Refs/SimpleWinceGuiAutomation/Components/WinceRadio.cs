using System;
using SimpleWinceGuiAutomation.Core;
using SimpleWinceGuiAutomation.Wince;

namespace SimpleWinceGuiAutomation.Components
{
    public class WinceRadio : WinceComponent
    {
        public WinceRadio(IntPtr ptr) : base(ptr) { }

        public override string TagName()
        {
            return "input";
        }
        public override string AttributeValue(string name)
        {
            return name == "type"? "radio" :
                name == "checked"? Checked.ToString().ToLower() :
                null;
        }

        public static bool Check(WinComponent component)
        {
            if (!component.Class.ToLower().Contains("button"))
            {
                return false;
            }
            int style = component.Style;
            int BS_TYPEMASK = 0x0000000F;
            int BS_RADIOBUTTON = 0x0004;
            int BS_AUTORADIOBUTTON = 0x0009;
            style = style & BS_TYPEMASK;
            return (style == BS_RADIOBUTTON || style == BS_AUTORADIOBUTTON);
        }

        public String Text
        {
            get { return WindowHelper.GetText(Handle); }
        }

        public bool Checked
        {
            get { return (int) PInvoke.SendMessage(Handle, PInvoke.BM_GETCHECK, (IntPtr) 0x0, (IntPtr) 0) == 1; }
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