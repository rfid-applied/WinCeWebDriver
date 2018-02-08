using NUnit.Framework;

namespace SimpleWinceGuiAutomation.Tests
{
    [TestFixture]
    public class ButtonsTest : WinceTest
    {
        [Test]
        public void TestClick()
        {
            var button = application.MainWindow.Buttons.WithText("OtherButton");
            Assert.AreEqual("OtherButton", button.Text);
            button.Click();
            Assert.AreEqual("Clicked", button.Text);
        }

        [Test]
        public void TestReadAllButtons()
        {
            var buttons = application.MainWindow.Buttons.All;
            Assert.AreEqual(3, buttons.Count);
            Assert.AreEqual("Bouton1", buttons[0].Text);
            Assert.AreEqual("OtherButton", buttons[1].Text);
            Assert.AreEqual("dlg", buttons[2].Text);
            Assert.AreEqual(40, buttons[0].Size.Height);
            Assert.False(buttons[0].Enabled);
            Assert.True(buttons[1].Enabled);
            Assert.True(buttons[2].Enabled);
        }

        [Test]
        public void TestClickModalDlg()
        {
            var buttons = application.MainWindow.Buttons.WithText("dlg");
            buttons.Click();

        }
    }
}