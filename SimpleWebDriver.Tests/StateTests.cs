using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SimpleWebDriver.Tests
{
    public class StateTests : BaseTest
    {
        static string SUT = "\\Program Files\\SimpleWinceGuiAutomation.AppTest\\SimpleWinceGuiAutomation.AppTest.exe";

        public static void ElementRect(string endpoint)
        {
            var wd = new WebDriver(endpoint);
            wd.Session(SUT, null);

            var Bouton1 = wd.GetElement("link text", "Bouton1");

            var rect = wd.ElementState(Bouton1, "rect");

            var x = rect.Value<int>("x");
            var y = rect.Value<int>("y");
            var w = rect.Value<int>("width");
            var h = rect.Value<int>("height");
        }

        public static void ElementSelectedRadio(string endpoint)
        {
            var wd = new WebDriver(endpoint);
            wd.Session(SUT, null);

            var r1 = wd.GetElement("link text", "First Radio");
            var selected1 = wd.ElementState(r1, "selected");

            Assertions.Equal(true, selected1.Value<bool>());

            var r2 = wd.GetElement("link text", "Second Radio");
            var selected2 = wd.ElementState(r2, "selected");
            Assertions.Equal(false, selected2.Value<bool>());
        }

        public static void ElementSelectedCheckbox(string endpoint)
        {
            var wd = new WebDriver(endpoint);
            wd.Session(SUT, null);

            var c1 = wd.GetElement("link text", "My checkbox");
            var selected1 = wd.ElementState(c1, "selected");

            Assertions.Equal(false, selected1.Value<bool>());

            var c2 = wd.GetElement("link text", "My checkbox checked");
            var selected2 = wd.ElementState(c2, "selected");
            Assertions.Equal(true, selected2.Value<bool>());
        }

        public static void ElementText(string endpoint)
        {
            var wd = new WebDriver(endpoint);
            wd.Session(SUT, null);

            var c1 = wd.GetElement("link text", "My checkbox");
            var text = wd.ElementState(c1, "text");
            Assertions.Equal("My checkbox", text.Value<string>());

            var b1 = wd.GetElement("link text", "Premier");
            text = wd.ElementState(b1, "text");
            Assertions.Equal("Premier", text.Value<string>());
        }

        public static void ElementEnabled(string endpoint)
        {
            var wd = new WebDriver(endpoint);
            wd.Session(SUT, null);

            var b1 = wd.GetElement("link text", "Bouton1");
            var enabled = wd.ElementState(b1, "enabled");
            Assertions.Equal(false, enabled.Value<bool>());
        }
    }
}
