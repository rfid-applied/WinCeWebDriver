using System;
using System.Collections.Generic;
using System.Linq;
using SimpleWinceGuiAutomation.Wince;

namespace SimpleWinceGuiAutomation.Core
{
    public class ComponentRequester<TComponent>
    {
        private Func<IntPtr, TComponent> componentFactory;
        private IntPtr handle;

        private Func<WinComponent, bool> isKind;

        public ComponentRequester(Func<IntPtr, TComponent> componentFactory, Func<WinComponent, bool> isKind,
                                  IntPtr handle)
        {
            this.componentFactory = componentFactory;
            this.isKind = isKind;
            this.handle = handle;
        }

        public List<TComponent> All
        {
            get
            {
                List<WinComponent> childs = new WinceComponentsFinder().ListChilds(handle);
                return (from e in childs
                        where isKind(e)
                        orderby e.Top , e.Left
                        select componentFactory(e.Handle)).ToList();
            }
        }

        public List<TComponent> WithTexts(String text)
        {
            List<WinComponent> childs = new WinceComponentsFinder().ListChilds(handle);
            return (from e in childs
                    where isKind(e) && text == e.Text
                    orderby e.Top , e.Left
                    select componentFactory(e.Handle)).ToList();
        }

        public TComponent WithText(String text)
        {
            return WithTexts(text).First();
        }
    }
}