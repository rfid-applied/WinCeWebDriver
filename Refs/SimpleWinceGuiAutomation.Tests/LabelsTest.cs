using NUnit.Framework;

namespace SimpleWinceGuiAutomation.Tests
{
    [TestFixture]
    public class LabelsTest : WinceTest
    {

        [Test]
        public void TestReadAllLabels()
        {
            var labels = application.MainWindow.Labels.All;
            Assert.AreEqual(1, labels.Count);
            Assert.AreEqual("A label", labels[0].Text);
        }
    }
}