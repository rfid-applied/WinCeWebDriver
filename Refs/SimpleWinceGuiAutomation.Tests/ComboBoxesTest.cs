using System;
using NUnit.Framework;

namespace SimpleWinceGuiAutomation.Tests
{
    [TestFixture]
    public class ComboBoxesTest : WinceTest
    {
        [Test]
        public void TestClick()
        {
            var comboBox = application.MainWindow.ComboBoxes.All[0];
            Assert.AreEqual("", comboBox.Text);
            var items = comboBox.Items;
            Assert.AreEqual(3, items.Count);
            Assert.AreEqual("First", items[0]);
            Assert.AreEqual("Second", items[1]);
            Assert.AreEqual("Third", items[2]);
            comboBox.Select("First");
            Assert.AreEqual("First", comboBox.Text);
            try
            {
                comboBox.Select("Non existing");
                Assert.Fail("Must fail");
            }
            catch (Exception)
            {
            }
            Assert.AreEqual("First", comboBox.Text);
        }

        [Test]
        public void TestReadAllComboBoxes()
        {
            var comboBoxes = application.MainWindow.ComboBoxes.All;
            Assert.AreEqual(2, comboBoxes.Count);
        }
    }
}