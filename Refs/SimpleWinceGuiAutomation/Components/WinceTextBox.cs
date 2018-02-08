using System;
using SimpleWinceGuiAutomation.Wince;

namespace SimpleWinceGuiAutomation.Components
{
    public class WinceTextBox : WinceComponent
    {

        public WinceTextBox(IntPtr ptr) : base(ptr) { }

        public static bool Check(SimpleWinceGuiAutomation.Core.WinComponent e)
        {
            return e.Class.ToLower().Contains("edit");
        }

        public override string TagName()
        {
            return "input";
        }
        public override string AttributeValue(string name)
        {
            return name == "type"? "text" :
                name == "text"? Text : null;
        }

        public String Text
        {
            get { return WindowHelper.GetText(Handle); }
            set {WindowHelper.SetText(Handle, value);}
        }

        public void Click()
        {
            WindowHelper.Click(Handle, null);
        }

        public void Click(SimpleWinceGuiAutomation.Wince.WindowHelper.Location loc)
        {
            WindowHelper.Click(Handle, loc);
        }

        public void Focus()
        {
            /*
            var loc = new SimpleWinceGuiAutomation.Wince.WindowHelper.Location()
            {
                X = Location.X + Size.Width / 2,
                Y = Location.Y + Size.Height / 2
            };
            WindowHelper.Click(Handle, null);*/
            WindowHelper.Focus(Handle);
        }
    }
}