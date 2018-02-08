using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SimpleWinceGuiAutomation.Query
{
    public interface NodeOps<T>
    {
        IEnumerable<T> DepthFirstSearch(T root, Func<T,bool> pred);
        IEnumerable<T> Navigate(T root, Op o);
        string TagName(T root);
        bool SatisfiesAttrib(T node, string op, string property, string value);
    }

    public class Evaluator<T>
    {
        public Evaluator(NodeOps<T> ops)
        {
            _lexer = new Lexer();
            _parser = new Parser(_lexer);
            _ops = ops;
        }

        NodeOps<T> _ops;
        Lexer _lexer;
        Parser _parser;

        bool Satisfies(Node node, T root)
        {
            if (node is TagName)
            {
                return _ops.TagName(root) == ((TagName)node).Tag;
            }
            else if (node is Attrib)
            {
                var attrib = ((Attrib)node);
                return Satisfies(attrib.Element, root) &&
                    _ops.SatisfiesAttrib(root, attrib.Op, attrib.Prop, attrib.Parm);
            }
            return false;
        }

        IEnumerable<T> Eval(Node expr, T root)
        {
            if (expr is App)
            {
                var app = (App)expr;
                var op = app.Operation;
                var lhs = app.Operands[0];
                var rhs = app.Operands[1];

                var res = _ops
                    .DepthFirstSearch(root, r => Satisfies(lhs, r))
                    .SelectMany(n => _ops.Navigate(n, op))
                    .Where(n => Satisfies(rhs, n));

                foreach (var c in res)
                    yield return c;
            }
            else
            {
                var res = _ops.DepthFirstSearch(root, r => Satisfies(expr, r));
                foreach (var c in res)
                    yield return c;
            }
            yield break;
        }

        public IEnumerable<T> Evaluate(string exprSource, T root)
        {
            var expr = _parser.Parse(exprSource);

            return Eval(expr, root);
        }
    }
}
