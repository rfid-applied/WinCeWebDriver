using System;
using SimpleWinceGuiAutomation.Core;
using SimpleWinceGuiAutomation.Wince;
using System.Collections.Generic;
using System.Linq;

namespace SimpleWinceGuiAutomation.Components
{
    public class WinceWindow
    {
        private readonly IntPtr handle;

        public WinceWindow(IntPtr handle)
        {
            this.handle = handle;
        }

        public string GetTitle()
        {
            return Wince.WindowHelper.GetText(handle);
        }

        public ComponentRequester<WinceButton> Buttons
        {
            get
            {
                return new ComponentRequester<WinceButton>(ptr => new WinceButton(ptr), WinceButton.Check, handle);
            }
        }

        public ComponentRequester<WinceCheckBox> CheckBoxes
        {
            get { return new ComponentRequester<WinceCheckBox>(ptr => new WinceCheckBox(ptr), WinceCheckBox.Check, handle); }
        }

        public ComponentRequester<WinceTextBox> TextBoxes
        {
            get
            {
                return new ComponentRequester<WinceTextBox>(ptr => new WinceTextBox(ptr), WinceTextBox.Check, handle);
            }
        }

        public ComponentRequester<WinceComboBox> ComboBoxes
        {
            get
            {
                return new ComponentRequester<WinceComboBox>(ptr => new WinceComboBox(ptr), WinceComboBox.Check, handle);
            }
        }

        public ComponentRequester<WinceContainer> Containers
        {
            get { return new ComponentRequester<WinceContainer>(ptr => new WinceContainer(ptr), WinceContainer.Check, handle); }
        }

        public IEnumerable<WinceComponent> ListElements(Func<WinceComponent,bool> pred)
        {
            var root = Tree(handle);

            var stack = new Stack<WinceComponent>();

            stack.Push(root);
            while (stack.Count > 0)
            {
                var node = stack.Pop();

                if (pred(node))
                {
                    yield return node;
                }

                foreach (var child in node.Children)
                    stack.Push(child);
            }

            yield break;
        }

        static WinceComponent FromWinComponent(WinComponent c)
        {
            if (WinceComboBox.Check(c))
            {
                return new WinceComboBox(c.Handle);
            }
            else if (WinceButton.Check(c))
            {
                return new WinceButton(c.Handle);
            }
            else if (WinceCheckBox.Check(c))
            {
                return new WinceCheckBox(c.Handle);
            }
            else if (WinceTextBox.Check(c))
            {
                return new WinceTextBox(c.Handle);
            }
            else if (WinceLabel.Check(c))
            {
                return new WinceLabel(c.Handle);
            }
            else if (WinceRadio.Check(c))
            {
                return new WinceRadio(c.Handle);
            }
            else if (WinceListBox.Check(c))
            {
                return new WinceListBox(c.Handle);
            }
            else if (WinceContainer.Check(c))
            {
                return new WinceContainer(c.Handle);
            }
            else
            {
                throw new NotImplementedException(string.Format("handle {0:X} unsupported", c.Handle));
            }
        }

        WinceComponent Tree(IntPtr handle)
        {
            var cmp = new ComponentComparer();

            var elems = WinceComponentsFinder.RecursiveFindWindow<WinceComponent>(handle,
                FromWinComponent,
                (p, c) =>
                {
                    AddSorted(p.Children, c, cmp);
                    c.Parent = p;
                    return p;
                });

            return elems;
        }

        public WinceComponent ElementByHandle(string id)
        {
            var ix = id.IndexOf('-');
            if (ix >= 0)
            {
                var ident = id.Substring(0, ix);
                var childnum = id.Substring(ix+1, id.Length - ix - 1);

                var handle = (IntPtr)int.Parse(ident, System.Globalization.NumberStyles.HexNumber);
                var root = Tree(handle);

                return root.Children.First(c => c.ID == id);
            }
            else
            {
                var handle = (IntPtr)int.Parse(id, System.Globalization.NumberStyles.HexNumber);
                var root = Tree(handle);
                return root;
            }
        }

        public WinceComponent Elements()
        {
            var elems = Tree(handle);
            return elems;
        }
        
        // thanks @Noseratio! https://stackoverflow.com/questions/12172162/how-to-insert-item-into-list-in-order
        public static void AddSorted<T>(List<T> @this, T item, IComparer<T> comp)
        {
            if (@this.Count == 0)
            {
                @this.Add(item);
                return;
            }
            if (comp.Compare(@this[@this.Count - 1], item) <= 0)
            {
                @this.Add(item);
                return;
            }
            if (comp.Compare(@this[0], item) >= 0)
            {
                @this.Insert(0, item);
                return;
            }
            int index = @this.BinarySearch(item, comp);
            if (index < 0)
                index = ~index;
            @this.Insert(index, item);
        }

        public ComponentRequester<WinceLabel> Labels
        {
            get { return new ComponentRequester<WinceLabel>(ptr => new WinceLabel(ptr), WinceLabel.Check, handle); }
        }


        public ComponentRequester<WinceRadio> Radios
        {
            get { return new ComponentRequester<WinceRadio>(ptr => new WinceRadio(ptr), WinceRadio.Check, handle); }
        }

        public ComponentRequester<WinceListBox> ListBoxes
        {
            get
            {
                return new ComponentRequester<WinceListBox>(ptr => new WinceListBox(ptr), WinceListBox.Check, handle);
            }
        }
    }

    public class ComponentComparer : IComparer<WinceComponent>
    {
        public int Compare(WinceComponent x, WinceComponent y)
        {            
            var r = WinceComponentsFinder.IntCompare(x.Location.Y, y.Location.Y);
            if (r == 0)
                return WinceComponentsFinder.IntCompare(x.Location.X, y.Location.X);
            return r;
        }
    }
}