using System;
using System.Collections.Generic;
using System.Text;
using SimpleWinceGuiAutomation.Core;
using SimpleWinceGuiAutomation.Wince;

namespace SimpleWinceGuiAutomation.Components
{
    public class WinceComboBox : WinceComponent
    {
        private const int CB_GETCOUNT = 0x0146;
        private const int CB_GETLBTEXT = 0x0148;
        private const int CB_GETLBTEXTLEN = 0x149;
        private const int CB_SETCURSEL = 0x014E;

        public WinceComboBox(IntPtr ptr) : base(ptr)
        {
            var items = Items;

            for (var i = 0; i < items.Count; i++)
            {
                var c = new WinceComboBoxItem(this, ptr);
                Children.Add(c);
            }
        }

        public override string TagName()
        {
            return "select";
        }
        public override string AttributeValue(string name)
        {
            return name == "text" ? Text : null;
        }

        public String Text
        {
            get { return WindowHelper.GetText(Handle); }
        }

        public List<String> Items
        {
            get
            {
                var items = new List<string>();
                IntPtr ptr = PInvoke.SendMessage(Handle, CB_GETCOUNT, (IntPtr) 0, (IntPtr) 0);
                for (int i = 0; i < ptr.ToInt32(); i++)
                {
                    items.Add(GetComboItem(i));
                }
                return items;
            }
        }

        private string GetComboItem(int index)
        {
            int size = PInvoke.SendMessage(Handle, CB_GETLBTEXTLEN, new IntPtr(index), new IntPtr(0)).ToInt32();
            var ssb = new StringBuilder(size);
            PInvoke.SendMessage(Handle, CB_GETLBTEXT, new IntPtr(index), ssb).ToInt32();
            return ssb.ToString();
        }


        public void Select(string value)
        {
            List<string> items = Items;
            for (int i = 0; i < items.Count; i++)
            {
                if (value == items[i])
                {
                    PInvoke.SendMessage(Handle, CB_SETCURSEL, (IntPtr) i, (IntPtr) 0);
                    return;
                }
            }
            throw new Exception("No item named : " + value);
        }

        public static bool Check(WinComponent c)
        {
            return c.Class.ToLower().Contains("combobox");
        }
    }

    public class WinceComboBoxItem : WinceComponent
    {
        private const int CB_GETCOUNT = 0x0146;
        private const int CB_GETLBTEXT = 0x0148;
        private const int CB_GETLBTEXTLEN = 0x149;
        private const int CB_SETCURSEL = 0x014E;

        public WinceComboBoxItem(WinceComboBox cb, IntPtr handle)
            : base(handle)
        {
            Parent = cb;
        }

        public override string ID
        {
            get {
                var i = Parent.Children.IndexOf(this);
                return ((long)Handle).ToString("X") + "-" + i.ToString("X");
            }
        }

        public override string TagName()
        {
            return "option";
        }

        public String Text
        {
            get
            {
                var index = this.Parent.Children.IndexOf(this);

                int size = PInvoke.SendMessage(Handle, CB_GETLBTEXTLEN, new IntPtr(index), new IntPtr(0)).ToInt32();
                var ssb = new StringBuilder(size);
                PInvoke.SendMessage(Handle, CB_GETLBTEXT, new IntPtr(index), ssb).ToInt32();
                return ssb.ToString();
            }
        }

        public bool Selected
        {
            get
            {
                var selectedText = WindowHelper.GetText(Handle);

                return selectedText == Text;
            }
        }

        // FIXME: this class will report parent's rectangle!

        public void Click()
        {
            var i = this.Parent.Children.IndexOf(this);
            PInvoke.SendMessage(Handle, CB_SETCURSEL, (IntPtr)i, (IntPtr)0);
        }

        public override string AttributeValue(string name)
        {
            return
                name == "selected" ? Selected.ToString().ToLower() :
                name == "text" ? Text :
                null;
        }
    }
}