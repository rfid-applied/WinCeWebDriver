using System;
using System.Collections.Generic;
using System.Text;
using SimpleWinceGuiAutomation.Core;
using SimpleWinceGuiAutomation.Wince;

namespace SimpleWinceGuiAutomation.Components
{
    public class WinceListBox : WinceComponent
    {
        private const int LB_GETCOUNT = 0x018B;
        private const int LB_GETCURSEL = 0x0188;
        private const int LB_GETTEXT = 0x0189;
        private const int LB_GETTEXTLEN = 0x018A;
        private const int LB_SETCURSEL = 0x0186;

        public WinceListBox(IntPtr ptr) : base(ptr)
        {
            var items = Items;

            for (var i = 0; i < items.Count; i++)
            {
                var c = new WinceListBoxItem(this, ptr);
                Children.Add(c);
            }
        }

        public override string TagName()
        {
            return "ul";
        }
        public override string AttributeValue(string name)
        {
            return null;
        }

        public int SelectedItem
        {
            get { return PInvoke.SendMessage(Handle, LB_GETCURSEL, (IntPtr) 0, (IntPtr) 0).ToInt32(); }
        }

        public List<String> Items
        {
            get
            {
                var items = new List<string>();
                IntPtr ptr = PInvoke.SendMessage(Handle, LB_GETCOUNT, (IntPtr)0, (IntPtr)0);
                for (int i = 0; i < ptr.ToInt32(); i++)
                {
                    items.Add(GetListItem(i));
                }
                return items;
            }
        }

        private string GetListItem(int index)
        {
            int size = PInvoke.SendMessage(Handle, LB_GETTEXTLEN, new IntPtr(index), new IntPtr(0)).ToInt32();
            var sb = new StringBuilder(size);
            PInvoke.SendMessage(Handle, LB_GETTEXT, new IntPtr(index), sb);
            return sb.ToString();
        }

        public void Select(string value)
        {
            List<string> items = Items;
            for (int i = 0; i < items.Count; i++)
            {
                if (value == items[i])
                {
                    PInvoke.SendMessage(Handle, LB_SETCURSEL, (IntPtr)i, (IntPtr)0);
                }
            }
        }

        public static bool Check(WinComponent c)
        {
            return c.Class.ToLower().Contains("listbox");
        }
    }

    public class WinceListBoxItem : WinceComponent
    {
        private const int LB_GETCOUNT = 0x018B;
        private const int LB_GETCURSEL = 0x0188;
        private const int LB_GETTEXT = 0x0189;
        private const int LB_GETTEXTLEN = 0x018A;
        private const int LB_SETCURSEL = 0x0186;

        public WinceListBoxItem(WinceListBox lb, IntPtr handle)
            : base(handle)
        {
            Parent = lb;
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
            return "li";
        }

        public String Text
        {
            get {
                var index = this.Parent.Children.IndexOf(this);
                int size = PInvoke.SendMessage(Handle, LB_GETTEXTLEN, new IntPtr(index), new IntPtr(0)).ToInt32();
                var sb = new StringBuilder(size);
                PInvoke.SendMessage(Handle, LB_GETTEXT, new IntPtr(index), sb);
                return sb.ToString();                
            }
        }

        public bool Selected
        {
            get {
                var index = this.Parent.Children.IndexOf(this);
                var selectedIdx = PInvoke.SendMessage(Handle, LB_GETCURSEL, (IntPtr) 0, (IntPtr) 0).ToInt32();

                return index == selectedIdx;
            }
        }

        // FIXME: this class will report parent's rectangle!

        public void Click()
        {
            var i = this.Parent.Children.IndexOf(this);
            PInvoke.SendMessage(Handle, LB_SETCURSEL, (IntPtr)i, (IntPtr)0);
        }

        public override string AttributeValue(string name)
        {
            return
                name == "selected" ? Selected.ToString().ToLower() :
                null;
        }
    }
}