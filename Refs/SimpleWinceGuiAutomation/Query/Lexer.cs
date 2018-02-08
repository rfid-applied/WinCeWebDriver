using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SimpleWinceGuiAutomation.Query
{
    public struct Position
    {
        public Position(int ofs, int line, int column)
        {
            _ofs = ofs;
            _line = line;
            _column = column;
        }
        int _ofs;
        int _line, _column;
        public int Offset { get { return _ofs; } }
        public int Line { get { return _line; } }
        public int Column { get { return _column; } }
    }

    public class LexingException : Exception
    {
        public LexingException(string buf, Position pos, string message)
            : base(message)
        {
            Buffer = buf;
            Position = pos;
        }
        public Position Position { get; set; }
        public string Buffer { get; set; }
    }

    public enum TOK
    {
        ERR = 0,
        IDENT, // identifier
        PUNCTUATION, // , > + ~ = | $ * (or a combination of any of these)
        STRING, // "text"
        FLOAT, // floating-point number
        INTEGER, // plain integer
        PSEUDOCLASS, // :identifier
        LBRACKET, // [
        RBRACKET, // ]
        LPAREN, // (
        RPAREN, // )

        EOF
    }

    public struct Token
    {
        public Token(TOK ty, string lexeme, Position beg, Position end)
        {
            _type = ty;
            _lexeme = lexeme;
            _beg = beg;
            _end = end;
        }

        TOK _type;
        string _lexeme;
        Position _beg;
        Position _end;

        public TOK Type() { return _type; }
        public string Lexeme() { return _lexeme; }
        public int LocationBeg() { return _beg.Offset; }
        public int LocationEnd() { return _end.Offset; }

        public Position Beg { get { return _beg; } }
        public Position End { get { return _end; } }

        internal bool TypeIs(TOK ty) { return ty == _type; }
        internal void AssertType(TOK ty)
        {
            if (_type != ty)
                throw new LexingException(String.Empty, _beg, "expected type " + ty.ToString());
        }
        public double ExtractFloat()
        {
            AssertType(TOK.FLOAT);
            return double.Parse(_lexeme, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
        }
        public int ExtractInteger()
        {
            AssertType(TOK.INTEGER);
            return int.Parse(_lexeme, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
        }
    }

    public class Lexer
    {
        public Lexer()
        {
            _buf = String.Empty;
            _pos = 0;
            _line = 1;
            _column = 1;
        }
        string _buf;
        int _pos;
        int _column;
        int _line;

        public void SetBuffer(string newbuf)
        {
            _buf = newbuf;
            _pos = 0;
            _column = 1;
            _line = 1;
            _tokenBuf = null;
        }

        private bool LexIsWhitespace(int pos)
        {
            if (pos >= _buf.Length)
                return false;
            var x0 = _buf[pos];
            return (x0 == ' ' || x0 == '\t' || x0 == '\r' || x0 == '\n');
        }
        private void SkipWhitespace()
        {
            var beg = Pos();
            var ofs = _pos;
            while (LexIsWhitespace(ofs))
            {
                if (_buf[ofs] == '\n')
                {
                    _column = 1;
                    _line++;
                    ofs++;
                }
                else
                {
                    _column++;
                    ofs++;
                }
            }
            _pos = ofs;
        }
        private bool LexIsIdentifier(int pos)
        {
            if (pos >= _buf.Length)
                return false;
            var x0 = _buf[pos];
            return ('a' <= x0 && x0 <= 'z' || 'A' <= x0 && x0 <= 'Z');
        }
        private Token PseudoClass()
        {
            var beg = Pos();
            _pos++;
            _column++;
            var tok = Identifier();
            return new Token(TOK.PSEUDOCLASS, tok.Lexeme(), tok.Beg, tok.End);
        }
        private Token StringLiteral()
        {
            var beg = Pos();
            _pos++; // skip first "
            var ofs = _pos;
            var sb = new StringBuilder();
            while (ofs < _buf.Length && _buf[ofs] != '"')
            {
                if (_buf[ofs] == '\\')
                {
                    // escaped char
                    ofs++;
                    if (ofs >= _buf.Length)
                    {
                        if (ofs == _pos)
                            throw new LexingException(_buf, Pos(), "Unexpected EOF, awaiting escape char");
                    }
                    var c = _buf[ofs];
                    char c0;
                    switch (c)
                    {
                        case 'r':
                            c0 = '\r';
                            break;
                        case 'n':
                            c0 = '\n';
                            break;
                        case 't':
                            c0 = '\t';
                            break;
                        case '"':
                            c0 = '"';
                            break;
                        default:
                            throw new LexingException(_buf, Pos(), "unsupported escape sequence: " + c.ToString());
                    }
                    sb.Append(c0);
                }
                else
                {
                    var c0 = _buf[ofs];
                    sb.Append(c0);
                }
                ofs++;
            }
            ofs++; // skip the trailing "

            _column = _column + (ofs - _pos);
            _pos = ofs;
            return new Token(TOK.STRING, sb.ToString(), beg, Pos());
        }
        private Token Identifier()
        {
            // ([a-zA-Z])([a-zA-Z0-9])+
            var beg = Pos();
            var ofs = _pos;
            bool ident = false;
            while (ofs < _buf.Length)
            {
                var x0 = _buf[ofs];
                if (ident)
                {
                    // check if the character is rest of identifier
                    // or something that can't be part of an identifier
                    if ('a' <= x0 && x0 <= 'z'
                        || 'A' <= x0 && x0 <= 'Z'
                        || '0' <= x0 && x0 <= '9')
                    {
                        ofs++;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    // check if the character begins an ident
                    if ('a' <= x0 && x0 <= 'z'
                        || 'A' <= x0 && x0 <= 'Z')
                    {
                        ofs++;
                        ident = true;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (ofs == _pos)
                throw new LexingException(_buf, Pos(), "Expected identifier");
            var kw = _buf.Substring(beg.Offset, ofs - beg.Offset);
            _column = _column + (ofs - beg.Offset);
            _pos = ofs;
            return new Token(TOK.IDENT, kw, beg, Pos());
        }
        public Position Pos()
        {
            return new Position(_pos, _line, _column);
        }
        private bool LexIsDigit(int pos)
        {
            if (pos >= _buf.Length)
                return false;
            var c = _buf[pos];
            if ('0' <= c && c <= '9')
                return true;
            return false;
        }
        private bool LexIsOneOf(int pos, char[] c)
        {
            if (pos >= _buf.Length)
                return false;
            return c.Contains(_buf[pos]);
        }
        private bool LexIsPunctuation(int pos)
        {
            return LexIsOneOf(pos, new[]{',', '>', '+', '~', '=', '|', '$', '*'});
        }
        private Token Punctuation()
        {
            // punctuation+
            var beg = Pos();
            var ofs = _pos;
            if (ofs >= _buf.Length)
                throw new LexingException(_buf, beg, "EOF");
            while (LexIsPunctuation(ofs))
                ofs++;
            var lexeme = _buf.Substring(beg.Offset, ofs - beg.Offset);
            _column = _column + (ofs - beg.Offset);
            _pos = ofs;
            return new Token(TOK.PUNCTUATION, lexeme, beg, Pos());
        }
        private bool LexIsChar(int pos, char c)
        {
            if (pos >= _buf.Length)
                return false;
            return _buf[pos] == c;
        }
        private bool LexIsInteger(int pos)
        {
            if (pos >= _buf.Length)
                return false;
            var x0 = _buf[pos];
            return (x0 == '-' || x0 == '+') || ('0' <= x0 && x0 <= '9');
        }
        private Token Integer()
        {
            // [+-]?[0-9]+
            var beg = Pos();
            var ofs = _pos;
            if (ofs >= _buf.Length)
                throw new LexingException(_buf, beg, "EOF");
            var x0 = _buf[ofs];
            if (x0 == '+' || x0 == '-')
                ofs++;
            if (!LexIsDigit(ofs))
                throw new LexingException(_buf, Pos(), "EOF");
            ofs++; // skip the mandatory digit
            while (LexIsDigit(ofs))
                ofs++;
            var lexeme = _buf.Substring(beg.Offset, ofs - beg.Offset);
            _column = _column + (ofs - beg.Offset);
            _pos = ofs;
            return new Token(TOK.INTEGER, lexeme, beg, Pos());
        }
        private bool LexIsFloat(int pos)
        {
            if (pos >= _buf.Length)
                return false;
            var x0 = _buf[pos];
            if (x0 == '.' && pos + 1 < _buf.Length)
            {
                // 1-character look ahead
                var x1 = _buf[pos + 1];
                return ('0' <= x1 && x1 <= '9');
            }
            return (x0 == '-' || x0 == '+') || ('0' <= x0 && x0 <= '9');
        }
        private Token Float()
        {
            // [-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?
            var beg = Pos();
            var ofs = _pos;
            if (ofs >= _buf.Length)
                throw new LexingException(_buf, beg, "EOF");

            var x0 = _buf[ofs];
            if (x0 == '-' || x0 == '+')
                ofs++;
            while (LexIsDigit(ofs))
                ofs++;
            if (ofs >= _buf.Length)
                throw new LexingException(_buf, beg, "EOF");
            var dot = _buf[ofs];
            if (dot == '.')
            {
                ofs++;
            }
            if (ofs >= _buf.Length)
                throw new LexingException(_buf, beg, "EOF");
            if (LexIsDigit(ofs))
                ofs++;
            else
                throw new LexingException(_buf, beg, "EOF");
            while (LexIsDigit(ofs))
                ofs++;
            if (ofs >= _buf.Length || !(_buf[ofs] == 'e' || _buf[ofs] == 'E'))
            {
                var kw = _buf.Substring(beg.Offset, ofs - beg.Offset);
                _column = _column + (ofs - beg.Offset);
                _pos = ofs;
                return new Token(TOK.FLOAT, kw, beg, Pos());
            }
            ofs++;
            if (ofs >= _buf.Length)
                throw new LexingException(_buf, Pos(), "EOF");
            var c = _buf[ofs];
            if (c == '+' || c == '-')
                ofs++;
            else if ('0' <= c && c <= '9')
                ofs++;
            else
                throw new LexingException(_buf, Pos(), "EOF");
            while (LexIsDigit(ofs))
                ofs++;
            var lexeme = _buf.Substring(beg.Offset, ofs - beg.Offset);
            _column = _column + (ofs - beg.Offset);
            _pos = ofs;
            return new Token(TOK.FLOAT, lexeme, beg, Pos());
        }
        public bool LexCompareKeyword(ref Token t, string shrt, string lng)
        {
            var lexeme = t.Lexeme().ToUpper();
            if (!String.IsNullOrEmpty(lng))
                return lexeme == shrt || lexeme == lng;
            return lexeme == shrt;
        }

        private Token? _tokenBuf;
        public void Putback(ref Token tok)
        {
            if (_tokenBuf.HasValue)
                throw new LexingException(String.Empty, Pos(), "Lexing: putback buffer already occupied");
            _tokenBuf = tok;
        }

        public void Lex(out Token res)
        {
            if (_tokenBuf.HasValue)
            {
                res = _tokenBuf.Value;
                _tokenBuf = null;
                return;
            }

            SkipWhitespace();
            var beg = Pos();
            if (_pos >= _buf.Length)
            {
                res = new Token(TOK.EOF, String.Empty, beg, Pos());
                return;
            }
            else if (LexIsPunctuation(_pos))
            {
                res = Punctuation();
                return;
            }
            else if (LexIsChar(_pos, ':'))
            {
                res = PseudoClass();
                return;
            }
            else if (LexIsChar(_pos, '('))
            {
                _pos++;
                _column++;
                res = new Token(TOK.LPAREN, "(", beg, Pos());
                return;
            }
            else if (LexIsChar(_pos, '['))
            {
                _pos++;
                _column++;
                res = new Token(TOK.LBRACKET, "[", beg, Pos());
                return;
            }
            else if (LexIsChar(_pos, ')'))
            {
                _pos++;
                _column++;
                res = new Token(TOK.RPAREN, ")", beg, Pos());
                return;
            }
            else if (LexIsChar(_pos, ']'))
            {
                _pos++;
                _column++;
                res = new Token(TOK.RBRACKET, "]", beg, Pos());
                return;
            }
            else if (LexIsChar(_pos, '"'))
            {
                res = StringLiteral();
                return;
            }
            else if (LexIsIdentifier(_pos))
            {
                res = Identifier();
                return;
            }
            else if (LexIsFloat(_pos))
            {
                res = Float();
                return;
            }
            else if (LexIsInteger(_pos))
            {
                res = Integer();
                return;
            }
            throw new LexingException(_buf, Pos(), "unrecognized lexeme");
        }
    }
}
