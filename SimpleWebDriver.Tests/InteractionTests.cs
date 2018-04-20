using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SimpleWebDriver.Tests
{
    public class InteractionTests : BaseTest
    {
        static string SUT = "\\Program Files\\SimpleWinceGuiAutomation.AppTest\\SimpleWinceGuiAutomation.AppTest.exe";

        public static void TestClick(string endpoint)
        {
            var wd = new WebDriver(endpoint);
            wd.Session(SUT, null);

            var Bouton1 = wd.GetElement("link text", "Bouton1");
            var OtherButton = wd.GetElement("link text", "OtherButton");

            wd.Click(OtherButton);
            var x = wd.GetElement("link text", "Clicked");
            Assertions.Equal(OtherButton, x);
        }
        
        public static void TestClickModalDlg(string endpoint)
        {
            var wd = new WebDriver(endpoint);
            wd.Session(SUT, null);

            var btn = wd.GetElement("link text", "dlg");

            wd.Click(btn);

            // check that scroll into view works
            var lab = wd.GetElement("link text", "label1");
            wd.Click(lab);

            var close = wd.GetElement("link text", "Close");
            wd.Click(close);
        }

        public static void DisabledButtonClick(string endpoint)
        {
            var wd = new WebDriver(endpoint);
            wd.Session(SUT, null);

            var Bouton1 = wd.GetElement("link text", "Bouton1");

            // should fail with an error here!
            var res = wd.PostFaultySessionCommand("element/" + Bouton1 + "/click", new object());
            Assertions.Equal("element not interactable", res["value"].Value<string>("error"));
        }

        public static void TestClickComboBox(string endpoint)
        {
            var wd = new WebDriver(endpoint);
            wd.Session(SUT, null);

            var comboBox = wd.GetElement("css selector", "select");
            Assertions.Equal(true, comboBox != null);
            
            var text = wd.ElementState(comboBox, "text");
            Assertions.Equal("", text.Value<string>());

            var items = wd.GetElements("css selector", "select > option").ToList();
            Assertions.Equal(3, items.Count);

            var texts = items.Select(handle => {
                var txt = wd.ElementState(handle, "text");
                return txt.Value<string>();
            }).ToList();

            Assertions.Equal("First", texts[0]);
            Assertions.Equal("Second", texts[1]);
            Assertions.Equal("Third", texts[2]);

            wd.Click(items[0]);

            var selected = wd.ElementState(items[0], "selected");
            Assertions.Equal(true, selected.Value<bool>());
        }

        public static void TestSelectItem(string endpoint)
        {
            // FIXME: how to select items webdriver-style?

            throw new NotImplementedException();
        }

        public static void TestCheckBox(string endpoint)
        {
            var wd = new WebDriver(endpoint);
            wd.Session(SUT, null);

            var cb1 = wd.GetElement("link text", "My checkbox checked");
            Assertions.Equal(true, cb1 != null);

            wd.Click(cb1);

            var cbs = wd.GetElements("css selector", "input[type=\"checkbox\"]").ToList();
            Assertions.Equal(2, cbs.Count);
        }

        public static void TestRadioBox(string endpoint)
        {
            var wd = new WebDriver(endpoint);
            wd.Session(SUT, null);

            var rb1 = wd.GetElement("link text", "First Radio");
            Assertions.Equal(true, rb1 != null);

            var rb2 = wd.GetElement("link text", "Second Radio");
            Assertions.Equal(true, rb2 != null);

            wd.Click(rb2);

            var r1 = wd.ElementState(rb1, "selected");
            Assertions.Equal(false, r1.Value<bool>());

            var r2 = wd.ElementState(rb2, "selected");
            Assertions.Equal(true, r2.Value<bool>());
        }

        public static void TestTextEntry(string endpoint)
        {
            var wd = new WebDriver(endpoint);
            wd.Session(SUT, null);

            var tb1 = wd.GetElement("css selector", "input[type=\"text\"]");
            Assertions.Equal(true, tb1 != null);

            wd.Clear(tb1);
            wd.SendKeys(tb1, "HELLO");

            var tb = wd.GetElement("link text", "HELLO");
            Assertions.Equal(tb1, tb);
        }

        public static void TestSIP(string endpoint)
        {
            var wd = new WebDriver(endpoint);
            wd.Session(SUT, null);

            var btn = wd.GetElement("link text", "dlg");

            wd.Click(btn);

            var tb1 = wd.GetElement("css selector", "input[type=\"text\"]");
            Assertions.Equal(true, tb1 != null);

            // when we do this, the SIP is opened, which interferes
            // with input
            wd.Clear(tb1);
            wd.SendKeys(tb1, "HELLO");

            var tb = wd.GetElement("link text", "HELLO");
            Assertions.Equal(tb1, tb);
        }
    }
}
