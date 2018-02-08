using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using SimpleWinceGuiAutomation;
using SimpleWinceGuiAutomation.Components;
using SimpleWinceGuiAutomation.Query;

namespace SimpleWebDriver
{
    public static class SessionUtility
    {
        public static string GetElementText(WinceComponent e)
        {
            if (e is WinceTextBox)
            {
                return ((WinceTextBox)e).Text;
            }
            else if (e is WinceLabel)
            {
                return ((WinceLabel)e).Text;
            }
            else if (e is WinceButton)
            {
                return ((WinceButton)e).Text;
            }
            else if (e is WinceCheckBox)
            {
                return ((WinceCheckBox)e).Text;
            }
            else if (e is WinceRadio)
            {
                return ((WinceRadio)e).Text;
            }
            return null;
        }

        public static IEnumerable<WinceComponent> FindElements(WinceApplication app, string strategy, string value, string startElementID) {
            IEnumerable<WinceComponent> elements = null;
            var components = app.CurrentWindow.Elements();

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("value");
            }
            
            switch (strategy)
            {
                case "css selector":
                    var ops = new NodeOps();
                    var eval = new Evaluator<WinceComponent>(ops);
                    elements = eval.Evaluate(value, components);
                    break;
                case "tag name":
                    elements = app.CurrentWindow.ListElements(c => c.TagName() == value);
                    break;
                case "link text":
                    elements = app.CurrentWindow.ListElements(e => {
                        var txt = GetElementText(e);
                        return txt == null ? false : txt == value;
                    });
                    break;
                case "partial link text":
                    elements = app.CurrentWindow.ListElements(e =>
                    {
                        var txt = GetElementText(e);
                        return txt == null ? false : txt.Contains(value);
                    });
                    break;
                default:
                    throw new ArgumentException("strategy");
            }
            return elements;
        }
    }

    public class NodeOps : NodeOps<WinceComponent>
    {
        public IEnumerable<WinceComponent> DepthFirstSearch(WinceComponent root, Func<WinceComponent, bool> pred)
        {
            var s = new Stack<WinceComponent>();

            s.Push(root);
            while (s.Count > 0)
            {
                var n = s.Pop();
                if (pred(n))
                {
                    yield return n;
                }
                if (n.Children == null)
                    continue;

                for (var i = 0; i < n.Children.Count; i++)
                    s.Push(n.Children[i]);
            }
        }

        public IEnumerable<WinceComponent> Navigate(WinceComponent root, Op op)
        {
            switch (op)
            {
                case Op.Child:
                    return root.Children;
                case Op.Next:
                    // in-order successor of the node
                    var res = Successor(root);
                    return res == null ? null : new[] { res };
                case Op.Descendant:
                    return DepthFirstSearch(root, (e) => true);
                case Op.Successor:
                    return Successors(root);
                default:
                    throw new NotSupportedException("operation unhandled: " + op.ToString());
            }
        }
        
        IEnumerable<WinceComponent> Successors(WinceComponent root)
        {
            var node = root;
            while (true)
            {
                var succ = Successor(node);
                if (succ == null)
                    break;
                yield return succ;
                node = succ;
            }
        }

        WinceComponent RightSiblingNode(WinceComponent node)
        {
            if (node.Parent == null)
                return null;

            var ix = node.Parent.Children.IndexOf(node);
            if (ix + 1 < node.Parent.Children.Count)
                return node.Parent.Children[ix + 1];
            return null;
        }

        // function to find left most node in a tree
        WinceComponent LeftMostNode(WinceComponent node)
        {
            while (node != null)
            {
                var ln = node.Children.Count > 0 ? node.Children[0] : null;
                if (ln == null)
                    break;
                node = ln;
            }
            return node;
        }

        // function to find right most node in a tree
        WinceComponent RightMostNode(WinceComponent node)
        {
            while (node != null)
            {
                var rn = node.Children.Count > 0 ? node.Children[node.Children.Count - 1] : null;
                if (rn == null)
                    break;
                node = rn;
            }
            return node;
        }

        // function to find inorder successor of 
        // a node
        public WinceComponent Successor(WinceComponent x)
        {
            var xr = RightSiblingNode(x);
            if (xr != null)
            {
                var res = LeftMostNode(xr);
                return res;
            }
            else
            // Case2: If right child is NULL
            {
                var tmp = x.Parent;
                while (true)
                {
                    if (tmp == null)
                        return null;
                    var xr0 = RightSiblingNode(tmp);
                    if (xr0 != null)
                        return LeftMostNode(xr0);
                    tmp = tmp.Parent;
                }
            }
        }

        public string TagName(WinceComponent root)
        {
            return root.TagName();
        }

        public bool SatisfiesAttrib(WinceComponent node, string op, string property, string value)
        {
            var v = node.AttributeValue(property);
            switch (op)
            {
                case "=": return v == value;
                case "^=": return v.StartsWith(value);
                case "$=": return v.EndsWith(value);
                case "*": return v.Contains(value);
                default:
                    throw new NotSupportedException("attribute selector with operator " + op + " is not supported");
            }
        }
    }
}
