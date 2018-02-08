using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using SimpleWinceGuiAutomation.Query;
using NUnit.Framework;

namespace SimpleWinceGuiAutomation.Tests
{
    [TestFixture]
    public class EvalTest
    {
        [Test]
        public void SuccessorTests()
        {
            var A = new MyNode("A", true);
            var B = new MyNode("B", true);
            var C = new MyNode("C", true);
            var D = new MyNode("D", true);
            var E = new MyNode("E", true);
            var F = new MyNode("F", true);

            A.AddChild(B);
            A.AddChild(C);
            B.AddChild(D);
            B.AddChild(E);
            E.AddChild(F);

            var ops = new MyNodeOps();

            Assert.AreEqual(F, ops.Successor(D));
            Assert.AreEqual(C, ops.Successor(F));
            Assert.AreEqual(null, ops.Successor(C));
        }

        [Test]
        public void SimpleTagName()
        {
            // form(label, button, panel(label, button))

            var form = new MyNode("form", true);
            var label1 = new MyNode("label", true);
            var button1 = new MyNode("button", true);
            var panel = new MyNode("panel", true);
            var label2 = new MyNode("label", true);
            var button2 = new MyNode("button", false);

            form.AddChild(label1);
            form.AddChild(button1);
            form.AddChild(panel);
            panel.AddChild(label2);
            panel.AddChild(button2);

            var ops = new MyNodeOps();

            var eval = new Evaluator<MyNode>(ops);
            {
                var list = eval.Evaluate("button", form).ToList();
                Assert.AreEqual(2, list.Count);
                Assert.IsTrue(list.Any(e => e == button1));
                Assert.IsTrue(list.Any(e => e == button2));
            }
            {
                var list = eval.Evaluate("button[enabled=\"false\"]", form).ToList();
                Assert.AreEqual(1, list.Count);
                Assert.IsTrue(button2 == list[0]);
            }
        }

        [Test]
        public void SuccessorTagName()
        {
            // form(label, button, panel(label, button))

            var form = new MyNode("form", true);
            var label1 = new MyNode("label", false);
            var button1 = new MyNode("button", true);
            var panel = new MyNode("panel", true);
            var label2 = new MyNode("label", true);
            var button2 = new MyNode("button", true);

            form.AddChild(label1);
            form.AddChild(button1);
            form.AddChild(panel);
            panel.AddChild(label2);
            panel.AddChild(button2);

            var ops = new MyNodeOps();

            var eval = new Evaluator<MyNode>(ops);
            {
                var list = eval.Evaluate("label[enabled=\"true\"] + button", form).ToList();
                Assert.AreEqual(1, list.Count);
                Assert.IsTrue(button2 == list[0]);
            }
            {
                var list = eval.Evaluate("label[enabled=\"false\"] + button", form).ToList();
                Assert.AreEqual(1, list.Count);
                Assert.IsTrue(button1 == list[0]);
            }
            {
                var list = eval.Evaluate("label[enabled=\"false\"] ~ label[enabled=\"true\"]", form).ToList();
                Assert.AreEqual(1, list.Count);
                Assert.IsTrue(label2 == list[0]);
            }
            {
                var list = eval.Evaluate("form label", form).ToList();
                Assert.AreEqual(2, list.Count);
                Assert.IsTrue(list.Any(e => e == label1));
                Assert.IsTrue(list.Any(e => e == label2));
            }
        }
    }

    public class MyNode
    {
        public MyNode(string tagname, bool enabled) { _tagname = tagname; _enabled = enabled; Children = new MyNode[] { }; }
        
        string _tagname;
        bool _enabled;

        public void AddChild(MyNode n)
        {
            var c = Children;
            Array.Resize(ref c, c.Length + 1);
            c[c.Length - 1] = n;
            n.Parent = this;
            Children = c;
        }

        public string TagName { get { return _tagname; } }
        public bool Enabled { get { return _enabled; } }
        public MyNode[] Children { get; set; }
        public MyNode Parent { get; set; }
    }

    public class MyNodeOps : NodeOps<MyNode>
    {
        public IEnumerable<MyNode> DepthFirstSearch(MyNode root, Func<MyNode, bool> pred)
        {
            var s = new Stack<MyNode>();

            s.Push(root);
            while ( s.Count > 0 ) {
                var n = s.Pop();
                if (pred(n))
                {
                    yield return n;
                }

                if (n.Children == null)
                    continue;

                for (var i = 0; i < n.Children.Length; i++)
                    s.Push(n.Children[i]);
            }
        }

        public IEnumerable<MyNode> Navigate(MyNode root, Op op)
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
        IEnumerable<MyNode> Successors(MyNode root)
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

        MyNode RightSiblingNode(MyNode node)
        {
            if (node.Parent == null)
                return null;

            var ix = Array.IndexOf(node.Parent.Children, node);
            if (ix + 1 < node.Parent.Children.Length)
                return node.Parent.Children[ix + 1];
            return null;
        }

        // function to find left most node in a tree
        MyNode LeftMostNode(MyNode node)
        {
            while (node != null)
            {
                var ln = node.Children.Length > 0? node.Children[0] : null;
                if (ln == null)
                    break;
                node = ln;
            }
            return node;
        }

        // function to find right most node in a tree
        MyNode RightMostNode(MyNode node)
        {
            while (node != null)
            {
                var rn = node.Children.Length > 0? node.Children[node.Children.Length-1] : null;
                if (rn == null)
                    break;
                node = rn;
            }
            return node;
        }

        // function to find inorder successor of 
        // a node
        public MyNode Successor(MyNode x)
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
        public string TagName(MyNode root)
        {
            return root.TagName;
        }

        public bool SatisfiesAttrib(MyNode node, string op, string property, string value)
        {
            if (property == "enabled")
            {
                var bv = bool.Parse(value);
                return node.Enabled == bv;
            }
            return false;
        }
    }
}
