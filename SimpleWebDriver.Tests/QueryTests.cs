using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SimpleWebDriver.Tests
{
    public class QueryTests : BaseTest
    {
        static string SUT = "\\Program Files\\SimpleWinceGuiAutomation.AppTest\\SimpleWinceGuiAutomation.AppTest.exe";

        public static void TestReadAllButtons(string endpoint)
        {
            var wd = new WebDriver(endpoint);
            wd.Session(SUT, null);

            var res = wd.PostSessionCommand("elements", new
            {
                @using = "tag name",
                value = "button"
            });

            Assertions.Equal("success", res.Value<string>("status"));
            var elements = (JArray)res["value"];
            Assertions.Equal(3, elements.Count);

            foreach (var elem in elements)
            {
                var handle = elem.Value<string>();
                // at least we got the handle
            }
        }

        public static void TestReadAllCheckBoxes(string endpoint)
        {
            var wd = new WebDriver(endpoint);
            wd.Session(SUT, null);

            var checkboxes = wd.GetElements("css selector", "input[type=\"checkbox\"]").ToList();
            Assertions.Equal(2, checkboxes.Count);

            // FIXME!
            var check = wd.GetElement("css selector", "input[checked=\"false\"]");
            Assertions.Equal(checkboxes[0], check);
        }

        public static void TestGetSecondTextBox(string endpoint)
        {
            var wd = new WebDriver(endpoint);
            wd.Session(SUT, null);

            var element = wd.GetElement("css selector", "input[type=\"text\"] + input[type=\"text\"]");
            Assertions.Equal(true, element != null);
        }

        public static void TestGetComboboxAfterCheckbox(string endpoint)
        {
            var wd = new WebDriver(endpoint);
            wd.Session(SUT, null);

            var element = wd.GetElement("css selector", "input[type=\"checkbox\"] ~ input[type=\"combobox\"]");
            Assertions.Equal(true, element != null);
        }

        public static void TestElement1(string endpoint)
        {
            var wd = new WebDriver(endpoint);
            wd.Session(SUT, null);

            var res = wd.PostSessionCommand("element", new
            {
                @using = "link text",
                value = "OtherButton"
            });

            Assertions.Equal("success", res.Value<string>("status"));
            // one element handle as a string
            Assertions.Equal(true, null != res.Value<string>("value"));
        }

        public static void TestLabelText(string endpoint)
        {
            var wd = new WebDriver(endpoint);
            wd.Session(SUT, null);

            var value = wd.GetElement("link text", "A label");
            Assertions.Equal(true, null != value);
        }
    }
}
