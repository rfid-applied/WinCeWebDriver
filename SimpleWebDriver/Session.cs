using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SimpleWebDriver
{
    public class Session
    {
        public string SessionId { get; set; }

        public string ApplicationUrl { get; set; }
        public Capability Capabilities { get; set; }
    }

    public class SessionRequest
    {
        public string sessionId { get; set; }
        public Capability capabilities { get; set; }
    }
    public class Capability
    {
        // path to app (URL)
        public string app { get; set; }
        // additional command-line arguments
        public string args { get; set; }
    }

    public class SessionUrlRequest
    {
        public string url { get; set; }
    }

    public class FindElementRequest
    {
        public string @using { get; set; }
        public string value { get; set; }
    }

    public class ElementScreenshotRequest
    {
        public bool scroll { get; set; }
    }

    public class SendKeysRequest
    {
        public string text { get; set; }
    }

    public class Rect
    {
        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
}