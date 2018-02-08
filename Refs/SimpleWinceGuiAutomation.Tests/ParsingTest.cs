using NUnit.Framework;
using SimpleWinceGuiAutomation.Query;

namespace SimpleWinceGuiAutomation.Tests
{
    [TestFixture]
    public class ParsingTest
    {
        Lexer _lexer;
        Parser _parser;

        [SetUp]
        public void Init()
        {
            _lexer = new Lexer();
            _parser = new Parser(_lexer);
        }

        [TearDown]
        public void KillApp()
        {
            _parser = null;
            _lexer = null;
        }

        [Test]
        public void ParseIdentAttrib()
        {
            var node = _parser.Parse("button[text|=\"foobar\"]");
            Assert.IsNotNull(node);
            Assert.IsTrue(node is Attrib);

            var attrib = (Attrib)node;

            Assert.AreEqual("button", ((TagName)attrib.Element).Tag);
            Assert.AreEqual("text", attrib.Prop);
            Assert.AreEqual("|=", attrib.Op);
            Assert.AreEqual("foobar", attrib.Parm);
        }

        [Test]
        public void ParseDescendant()
        {
            var node = _parser.Parse("button[text=\"foobar\"] input");
            Assert.IsNotNull(node);
            Assert.IsTrue(node is App);
            var app = (App)node;
            Assert.AreEqual(Op.Descendant, app.Operation);
            Assert.AreEqual(2, app.Operands.Length);

            Assert.IsTrue(app.Operands[0] is Attrib);
            Assert.IsTrue(app.Operands[1] is TagName);
        }

        [Test]
        public void ParseNext()
        {
            var node = _parser.Parse("button[text=\"foobar\"] + input");
            Assert.IsNotNull(node);
            Assert.IsTrue(node is App);
            var app = (App)node;

            Assert.AreEqual(Op.Next, app.Operation);
            Assert.AreEqual(2, app.Operands.Length);

            Assert.IsTrue(app.Operands[0] is Attrib);
            Assert.IsTrue(app.Operands[1] is TagName);
        }
    }
}
