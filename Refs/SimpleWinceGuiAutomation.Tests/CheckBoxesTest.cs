using NUnit.Framework;

namespace SimpleWinceGuiAutomation.Tests
{
    [TestFixture]
    public class CheckBoxesTest : WinceTest
    {
        [Test]
        public void TestCheck()
        {
            var checkBox = application.MainWindow.CheckBoxes.WithText("My checkbox");
            Assert.AreEqual("My checkbox", checkBox.Text);
            Assert.IsFalse(checkBox.Checked);
            checkBox.Click();
            Assert.IsTrue(checkBox.Checked);
        }

        [Test]
        public void TestReadAllCheckBoxes()
        {
            var checkBoxes = application.MainWindow.CheckBoxes.All;
            Assert.AreEqual(2, checkBoxes.Count);
            Assert.IsFalse(checkBoxes[0].Checked);
            Assert.AreEqual("My checkbox", checkBoxes[0].Text);
            Assert.IsTrue(checkBoxes[1].Checked);
            Assert.AreEqual("My checkbox checked", checkBoxes[1].Text);
        }
    }
}