using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SimpleWinceGuiAutomation.Query
{
    public struct Location
    {
        public Location(Position start, Position end)
        {
            _start = start;
            _end = end;
        }
        public static Location FromLocations(IEnumerable<Location> locs)
        {
            var s = locs.First();
            var e = locs.Last();
            return new Location(s, e);
        }
        public Location(Location start, Location end)
        {
            _start = start.Start;
            _end = end.End;
        }

        Position _start, _end;
        
        public Position Start { get { return _start; } }
        public Position End { get { return _end; } }
    }

    public abstract class Node
    {
        public Node(Location loc)
        {
            _loc = loc;
        }

        Location _loc;

        public Location Location { get { return _loc; } }
    }

    // attribute selector [prop op parm]
    public class Attrib : Node
    {
        string _prop, _op, _parm;
        Node _elem;
        public Attrib(Node elem, string prop, string op, string parm, Location loc) : base(loc)
        {
            _elem = elem;
            _prop = prop;
            _op = op;
            _parm = parm;
        }
        public Node Element { get { return _elem; } }
        public string Prop { get { return _prop; } }
        public string Op { get { return _op; } }
        public string Parm { get { return _parm; } }
    }

    public class TagName : Node
    {
        string _tagname;
        public TagName(string tagname, Location loc)
            : base(loc)
        {
            _tagname = tagname;
        }
        public string Tag { get { return _tagname; } }
    }

    public enum Op
    {
        Descendant, // E1 E2
        Child, // E1 > E2
        Next, // E1 + E2, aka next sibling
        Successor // E1 ~ E2, aka successor
    }

    public class App : Node
    {
        public App(Op op, Node[] operands) : base(Location.FromLocations(operands.Select(o => o.Location)))
        {
            _op = op;
            _operands = operands;
        }
        Op _op;
        Node[] _operands;
        public Op Operation { get { return _op; } }
        public Node[] Operands { get { return _operands; } }
    }

    public class Parser
    {
        public Parser(Lexer lex)
        {
            _lex = lex;
        }
        Lexer _lex;

        private Attrib ParseAttributeQuery(Node elem, out Token tok)
        {
            var beg = _lex.Pos();

            _lex.Lex(out tok);
            if (tok.Type() != TOK.LBRACKET)
            {
                _lex.Putback(ref tok);
                return null;
            }

            _lex.Lex(out tok);
            tok.AssertType(TOK.IDENT);
            var propertyLoc = new Location(tok.Beg, tok.End);
            var property = tok.Lexeme();

            _lex.Lex(out tok);
            tok.AssertType(TOK.PUNCTUATION);
            var operLoc = new Location(tok.Beg, tok.End);
            var oper = tok.Lexeme();

            _lex.Lex(out tok);
            tok.AssertType(TOK.STRING);
            var valueLoc = new Location(tok.Beg, tok.End);
            var value = tok.Lexeme();

            _lex.Lex(out tok);
            tok.AssertType(TOK.RBRACKET);

            var end = tok.End;

            return new Attrib(elem, property, oper, value, new Location(beg, end));
        }

        Node ParseTerm(out Token tok)
        {
            _lex.Lex(out tok);
            tok.AssertType(TOK.IDENT);

            var beg = tok.Beg;
            var ident = tok.Lexeme();
            var end = tok.End;
            var tagname = new TagName(ident, new Location(beg, end));

            var attrib = ParseAttributeQuery(tagname, out tok);
            if (attrib == null)
            {
                return tagname;
            }
            else
            {
                return attrib;
            }
        }

        // expr := term expr' EOF
        // expr' := punctuation term expr' | term expr' | epsilon
        // term := IDENT attrib
        // attrib := LBRACK IDENT PUNCTUATION STRING RBRACK | epsilon

        public Node ParseExpr1(Node t, out Token tok)
        {
            _lex.Lex(out tok);
            if (tok.Type() == TOK.PUNCTUATION)
            {
                var l = tok.Lexeme();

                Op op;
                if (l == ">") op = Op.Child;
                else if (l == "+") op = Op.Next;
                else if (l == "~") op = Op.Successor;
                else
                    throw new NotSupportedException("wrong operand: " + l);

                var t1 = ParseTerm(out tok);

                var node = new App(op, new[] { t, t1 });
                
                return ParseExpr1(node, out tok);                
            }
            else if (tok.Type() != TOK.EOF)
            {
                var op = Op.Descendant;

                _lex.Putback(ref tok);
                var t1 = ParseTerm(out tok);

                var node = new App(op, new[] { t, t1 });
                
                return ParseExpr1(node, out tok);
            }
            else
            {
                _lex.Putback(ref tok);
                return t;
            }
        }

        public Node ParseExpr(out Token tok)
        {
            var t = ParseTerm(out tok);
            var t1 = ParseExpr1(t, out tok);

            // disallow unconsumed input
            tok.AssertType(TOK.EOF);
            return t1;
        }

        public Node Parse(string buffer)
        {
            _lex.SetBuffer(buffer);
            Token tok;

            var res = ParseExpr(out tok);

            return res;
        }

#if false
        public Node Exp()
        {
            /*
            simple grammar for Expr:
            - identifier ( | attribute)
            - pseudoident lparen Expr rparen
            - Expr comma Expr

            - Expr Expr
            - Expr > Expr
            - Expr + Expr
            - Expr ~ Expr
            */
            Token tok;

            // expr

            _lex.Lex(out tok);
            if (tok.Type() == TOK.IDENT)
            {
                // check next token
                // attribute if [
                // otherwise can be empty
            }
            else if (tok.Type() == TOK.PSEUDOCLASS)
            {
                // check next token
                // can be (
                // or empty
            }
            else if (tok.Type() == TOK.PUNCTUATION && tok.Lexeme() == ",")
            {
                // 
            }

            throw new NotImplementedException();
        }
#endif
    }
}
