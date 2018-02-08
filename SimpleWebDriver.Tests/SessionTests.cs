using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SimpleWebDriver.Tests
{
    public class SessionTests : BaseTest
    {
        static string SUT = "\\Program Files\\SimpleWinceGuiAutomation.AppTest\\SimpleWinceGuiAutomation.AppTest.exe";

        public static void TestCreate(string endpoint)
        {
            var wd = new WebDriver(endpoint);
            wd.Session(SUT, null);
        }

        public static void TestDelete(string endpoint)
        {
            var wd = new WebDriver(endpoint);
            wd.Session(SUT, null);
            wd.SessionDrop();
        }

        public static void TestFullScreenshot(string endpoint)
        {
            var wd = new WebDriver(endpoint);
            wd.Session(SUT, null);
            var res = wd.GetSessionCommand("screenshot");

            Assertions.Equal(wd.SessionID, res.Value<string>("sessionId"));
            Assertions.Equal("success", res.Value<string>("status"));
            byte[] bytes = Convert.FromBase64String(res.Value<string>("value"));
        }

        public static void TestSource(string endpoint)
        {
            var wd = new WebDriver(endpoint);
            wd.Session(SUT, null);
            var res = wd.GetSessionCommand("source");

            Assertions.Equal("success", res.Value<string>("status"));
            Assertions.Equal(true, res["value"] != null);
        }

        public static void TestTitle(string endpoint)
        {
            var wd = new WebDriver(endpoint);
            wd.Session(SUT, null);
            var res = wd.GetSessionCommand("title");

            Assertions.Equal("success", res.Value<string>("status"));
            Assertions.Equal("Form1", res.Value<string>("value"));
        }
    }
}
