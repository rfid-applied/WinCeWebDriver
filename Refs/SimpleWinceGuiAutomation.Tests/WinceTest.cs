using NUnit.Framework;
using SimpleWinceGuiAutomation.AppTest;
using SimpleWinceGuiAutomation.Components;

namespace SimpleWinceGuiAutomation.Tests
{
    public class WinceTest
    {
        protected WinceApplication application;

        [SetUp]
        public void Init()
        {
            application = WinceApplicationFactory.StartFromTypeInApplication<Form1>();
        }

        [TearDown]
        public void KillApp()
        {
            application.Kill();
        }
    }
}