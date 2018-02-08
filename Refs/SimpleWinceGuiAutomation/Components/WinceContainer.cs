using System;

namespace SimpleWinceGuiAutomation.Components
{
    public class WinceContainer : WinceComponent
    {
        public WinceContainer(IntPtr ptr) : base(ptr) { }

        public override string TagName()
        {
            return "div";
        }
        public override string AttributeValue(string name)
        {
            return null;
        }

        public static bool Check(SimpleWinceGuiAutomation.Core.WinComponent c)
        {
            return c.Class.ToLower().Equals("#netcf_agl_base_");
        }
    }
}