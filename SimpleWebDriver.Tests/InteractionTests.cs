using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleWebDriver.Tests
{
    public class InteractionTests : BaseTest
    {
        static string SUT = "\\Program Files\\SimpleWinceGuiAutomation.AppTest\\SimpleWinceGuiAutomation.AppTest.exe";

        public static void TestClick(string endpoint)
        {
            var wd = new WebDriver(endpoint);

            var Bouton1 = wd.GetElement("link text", "Bouton1");
            var OtherButton = wd.GetElement("link text", "OtherButton");

            wd.Click(OtherButton);
            var x = wd.GetElement("link text", "Clicked");
            Assertions.Equal(OtherButton, x);
        }
        
        public static void TestClickModalDlg(string endpoint)
        {
            var wd = new WebDriver(endpoint);

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

            var Bouton1 = wd.GetElement("link text", "Bouton1");

            wd.Click(Bouton1);
            // not interactable!
            throw new NotImplementedException();
        }

        public static void TestSelectItem(string endpoint)
        {
            // FIXME: how to select items webdriver-style?
            throw new NotImplementedException();
        }

        public static void TestCheckBox(string endpoint)
        {
            var wd = new WebDriver(endpoint);

            var cb1 = wd.GetElement("link text", "My checkbox checked");
            Assertions.Equal(true, cb1 != null);

            wd.Click(cb1);

            var cbs = wd.GetElements("css selector", "input[type=\"checkbox\"]").ToList();
            Assertions.Equal(2, cbs.Count);
        }

        public static void TestRadioBox(string endpoint)
        {
            var wd = new WebDriver(endpoint);
            var cb1 = wd.GetElement("link text", "Second Radio");
            Assertions.Equal(true, cb1 != null);

            wd.Click(cb1);
            // FIXME: how to check that second radio got activated?
            // need to support query like input[type="checkbox"][checked] (i.e. two assertions about the element)
            throw new NotImplementedException();
        }

        public static void TestTextEntry(string endpoint)
        {
            var wd = new WebDriver(endpoint);

            var tb1 = wd.GetElement("css selector", "input[type=\"text\"]");
            Assertions.Equal(true, tb1 != null);

            wd.Clear(tb1);
            wd.SendKeys(tb1, "HELLO");

            var tb = wd.GetElement("link text", "HELLO");
            Assertions.Equal(tb1, tb);
        }
    }
}
