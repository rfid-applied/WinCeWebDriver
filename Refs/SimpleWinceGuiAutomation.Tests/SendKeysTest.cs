using NUnit.Framework;
using OpenNETCF.Windows.Forms;
using System;
using SimpleWinceGuiAutomation.Tests;

namespace OpenNETCF.Windows.Forms.Test
{
    [TestFixture]
    public class SendKeysTest : WinceTest
    {
        // NOTE: adapted from OpenNETCF
        [Test]
        public void SendKeysTestPositive()
        {
            var textBoxes = application.MainWindow.TextBoxes.All;

            textBoxes[0].Focus();
            SendKeys.Send("hello");

            Assert.AreEqual("helloPremier", textBoxes[0].Text);
        }
    }
}
