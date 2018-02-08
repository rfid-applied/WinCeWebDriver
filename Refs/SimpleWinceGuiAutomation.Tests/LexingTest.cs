using NUnit.Framework;
using SimpleWinceGuiAutomation.Query;
namespace SimpleWinceGuiAutomation.Tests
{
    [TestFixture]
    public class LexingTest
    {
        Lexer _lexer;
        
        [SetUp]
        public void Init()
        {
            _lexer = new Lexer();
        }

        [TearDown]
        public void KillApp()
        {
            _lexer = null;
        }

        static void Ensure(Token token, string lexeme, TOK tp, Position beg, Position end)
        {
            Assert.IsNotNull(token);
            Assert.AreEqual(lexeme, token.Lexeme());
            Assert.AreEqual(tp, token.Type());
            Assert.AreEqual(beg, token.Beg);
            Assert.AreEqual(end, token.End);
        }

        [Test]
        public void TestWhiteSpaceParenIdent()
        {
            _lexer.SetBuffer("button\r\n\t, input");
            Token token;

            _lexer.Lex(out token);
            Ensure(token, "button", TOK.IDENT, new Position(0, 1, 1), new Position(6, 1, 7));

            _lexer.Lex(out token);
            Ensure(token, ",", TOK.PUNCTUATION, new Position(9, 2, 2), new Position(10, 2, 3));

            _lexer.Lex(out token);
            Ensure(token, "input", TOK.IDENT, new Position(11, 2, 4), new Position(16, 2, 9));
        }

        [Test]
        public void Ident()
        {
            Token token;
            _lexer.SetBuffer("button[text|=\"HELLO\\r\\nworld!\"]");

            _lexer.Lex(out token);
            Ensure(token, "button", TOK.IDENT, new Position(0, 1, 1), new Position(6, 1, 7));

            _lexer.Lex(out token);
            Ensure(token, "[", TOK.LBRACKET, new Position(6, 1, 7), new Position(7, 1, 8));

            _lexer.Lex(out token);
            Ensure(token, "text", TOK.IDENT, new Position(7, 1, 8), new Position(11, 1, 12));

            _lexer.Lex(out token);
            Ensure(token, "|=", TOK.PUNCTUATION, new Position(11, 1, 12), new Position(13, 1, 14));

            _lexer.Lex(out token);
            Ensure(token, "HELLO\r\nworld!", TOK.STRING, new Position(13, 1, 14), new Position(30, 1, 30));

            _lexer.Lex(out token);
            Ensure(token, "]", TOK.RBRACKET, new Position(30, 1, 30), new Position(31, 1, 31));

            _lexer.Lex(out token);
            Ensure(token, "", TOK.EOF, new Position(31, 1, 31), new Position(31, 1, 31));
        }
    }
}
