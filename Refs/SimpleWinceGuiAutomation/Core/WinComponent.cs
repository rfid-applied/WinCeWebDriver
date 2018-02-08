using System;

namespace SimpleWinceGuiAutomation.Core
{
    public class WinComponent
    {
        public WinComponent(string @class, string text, IntPtr handle, int left, int top, int style)
        {
            Class = @class;
            Text = text;
            Handle = handle;
            Left = left;
            Top = top;
            Style = style;
        }

        public String Class { get; private set; }
        public String Text { get; private set; }
        public IntPtr Handle { get; private set; }
        public int Left { get; private set; }
        public int Top { get; private set; }
        public int Style { get; private set; }
    }
}