using System;
using SimpleWinceGuiAutomation.Core;
using SimpleWinceGuiAutomation.Wince;

namespace SimpleWinceGuiAutomation.Components
{
    public class WinceCheckBox : WinceComponent
    {
        public WinceCheckBox(IntPtr handle) : base(handle) { }

        public static bool Check(WinComponent component)
        {
            if (!component.Class.ToLower().Contains("button"))
            {
                return false;
            }
            int style = component.Style;
            int BS_TYPEMASK = 0x0000000F;
            int BS_CHECKBOX = 0x2;
            int BS_AUTOCHECKBOX = 0x3;
            style = style & BS_TYPEMASK;
            return (style == BS_AUTOCHECKBOX || style == BS_CHECKBOX);
        }

        public override string TagName()
        {
            return "input";
        }
        public override string AttributeValue(string name)
        {
            return name == "type"? "checkbox" :
                name == "checked"? Checked.ToString() :
                null;
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