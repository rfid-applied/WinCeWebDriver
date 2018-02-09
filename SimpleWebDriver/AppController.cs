using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using CompactWebServer;
using System.Threading;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using PubSubApi;

namespace SimpleWebDriver
{
    public class AppController : BaseController
    {
        static Session _session = null;

        [Route(HttpMethod.GET, "/status")]
        public void Status(HttpRequest req, HttpResponse res)
        {
            req.Server.RaiseLogEvent("info", string.Format("ping success"));
            
            var message = (_session == null) ? "ready to go" : "session already exists";

            res.SendJson(new
            {
                ready = (_session == null),
                message = message
            });
        }

        [Route(HttpMethod.POST, "/session")]
        public void CreateSession(HttpRequest req, HttpResponse res)
        {
            var data = req.ParseJSON<SessionRequest>();
            if (data == null || data.sessionId == null || data.capabilities == null || string.IsNullOrEmpty(data.capabilities.app))
            {
                res.SendErrorJson(StatusCode.BadRequest, new ErrorResponse(new Exception("invalid argument")));
                return;
            }
            /*
            if (_session != null)
            {
                res.SendErrorJson(StatusCode.InternalServerError, new ErrorResponse(new Exception("session not created")));
                return;
            }*/
            var sess = new Session()
            {
                SessionId = data.sessionId,
                Capabilities = data.capabilities
            };
            try
            {
                if (_session != null)
                {
                    _sessionApp.Kill();
                    _sessionApp = null;
                    _session = null;
                }
                var killed = ProcessCE.ProcessCE.FindAndKill(sess.Capabilities.app);

                _sessionApp = SimpleWinceGuiAutomation.WinceApplicationFactory.StartFromPath(sess.Capabilities.app, sess.Capabilities.args);
            }
            catch (Exception e)
            {
                _sessionApp = null;
                res.SendErrorJson(StatusCode.BadRequest, new ErrorResponse(new Exception("invalid argument")));
                return;
            }
            _session = sess;

            _sessionApp.DoStuff();

            req.Server.RaiseLogEvent("info", string.Format("session created: {0}", sess.SessionId));

            res.SendJson(new { success = true });
        }

        [Route(HttpMethod.DELETE, "/session/{sessionId}")]
        public void DeleteSession(HttpRequest req, HttpResponse res)
        {
            if (!req.UrlParameters.ContainsKey("sessionId") ||
                _session == null ||
                _sessionApp == null ||
                req.UrlParameters["sessionId"] != _session.SessionId)
            {
                res.SendErrorJson(StatusCode.BadRequest, new ErrorResponse(new Exception("invalid argument")));
                return;
            }
            string sessionID = req.UrlParameters["sessionId"];
            // delete the session, try to close it
            try
            {
                _sessionApp.Kill();
            }
            catch (Exception e)
            {
                req.Server.RaiseLogEvent("error", string.Format("failed to kill the app: {0}", e.Message));
            }
            _session = null;

            res.SendJson(new JsonResponse<object>(sessionID, "", null));
        }

#if false
        [Route(HttpMethod.POST, "/session/{sessionId}/url")]
        public void SessionUrlCreate(HttpRequest req, HttpResponse res)
        {
            if (!req.UrlParameters.ContainsKey("sessionId") ||
                _session == null ||
                req.UrlParameters["sessionId"] != _session.SessionId)
            {
                res.SendErrorJson(StatusCode.BadRequest, new ErrorResponse(new Exception("invalid argument")));
                return;
            }

            var data = req.ParseJSON<SessionUrlRequest>();
            if (data == null || data.url == null)
            {
                res.SendErrorJson(StatusCode.BadRequest, new ErrorResponse(new Exception("invalid argument")));
                return;
            }
            if (_sessionApp != null)
            {
                // can't run anything again... only one app at a time
                res.SendErrorJson(StatusCode.InternalServerError, new ErrorResponse(new Exception("invalid argument")));
                return;
            }
            try {
                _session.ApplicationUrl = data.url;
                _sessionApp = SimpleWinceGuiAutomation.WinceApplicationFactory.StartFromUrl(data.url);
            }
            catch (Exception e)
            {
                res.SendErrorJson(StatusCode.InternalServerError, new ErrorResponse(e));
                return;
            }

            res.SendJson(new JsonResponse<object>(_session.SessionId, "success", null));
        }

        [Route(HttpMethod.GET, "/session/{sessionId}/url")]
        public void SessionUrlCurrent(HttpRequest req, HttpResponse res)
        {
            if (!req.UrlParameters.ContainsKey("sessionId") ||
                _session == null ||
                req.UrlParameters["sessionId"] != _session.SessionId)
            {
                res.SendErrorJson(StatusCode.BadRequest, new ErrorResponse(new Exception("invalid argument")));
                return;
            }

            if (_sessionApp == null)
            {
                res.SendErrorJson(StatusCode.NotFound, new ErrorResponse(new Exception("no such window")));
                return;
            }

            res.SendJson(new JsonResponse<object>(_session.SessionId, "success", _session.ApplicationUrl));
        }
#endif

        [Route(HttpMethod.GET, "/session/{sessionId}/title")]
        public void SessionUrlTitle(HttpRequest req, HttpResponse res)
        {
            if (!req.UrlParameters.ContainsKey("sessionId") ||
                _session == null ||
                req.UrlParameters["sessionId"] != _session.SessionId)
            {
                res.SendErrorJson(StatusCode.BadRequest, new ErrorResponse(new Exception("invalid argument")));
                return;
            }

            if (_sessionApp == null)
            {
                res.SendErrorJson(StatusCode.NotFound, new ErrorResponse(new Exception("no such window")));
                return;
            }

            var title = _sessionApp.CurrentWindow.GetTitle();

            res.SendJson(new JsonResponse<object>(_session.SessionId, "success", title));
        }

        static SimpleWinceGuiAutomation.Components.WinceApplication _sessionApp = null;

        [Route(HttpMethod.GET, "/session/{sessionId}/source")]
        public void ViewSource(HttpRequest req, HttpResponse res)
        {
            if (!req.UrlParameters.ContainsKey("sessionId") ||
                _session == null ||
                req.UrlParameters["sessionId"] != _session.SessionId)
            {
                res.SendErrorJson(StatusCode.BadRequest, new ErrorResponse(new Exception("invalid argument")));
                return;
            }

            // TODO: if window is not open, return error 404, "no such window"

            try
            {
                var elements = _sessionApp.CurrentWindow.Elements();

                res.SendJson(new JsonResponse<SimpleWinceGuiAutomation.Components.WinceComponent>(_session.SessionId, "success", elements));
            }
            catch (Exception e)
            {
                res.SendErrorJson(StatusCode.InternalServerError, new ErrorResponse(e));
            }
        }

        [Route(HttpMethod.GET, "/session/{sessionId}/screenshot")]
        public void TakeScreenshot(HttpRequest req, HttpResponse res)
        {
            if (!req.UrlParameters.ContainsKey("sessionId") ||
                _session == null ||
                req.UrlParameters["sessionId"] != _session.SessionId)
            {
                res.SendErrorJson(StatusCode.BadRequest, new ErrorResponse(new Exception("invalid argument")));
                return;
            }

            // if window is not open, return error 404, "no such window"

            try
            {
                var base64 = Screenshot.TakeScreenshot(IntPtr.Zero, System.Drawing.Rectangle.Empty);
                res.SendJson(new JsonResponse<string>(_session.SessionId, "success", base64));
            }
            catch (Exception e)
            {
                res.SendErrorJson(StatusCode.InternalServerError, new ErrorResponse(e));
            }
        }

        [Route(HttpMethod.GET, "/session/{sessionId}/element/{elementId}/screenshot")]
        public void TakeElementScreenshot(HttpRequest req, HttpResponse res)
        {
            var data = req.ParseJSON<ElementScreenshotRequest>();

            if (!req.UrlParameters.ContainsKey("sessionId") ||
                _session == null ||
                req.UrlParameters["sessionId"] != _session.SessionId ||
                !req.UrlParameters.ContainsKey("elementId") ||
                string.IsNullOrEmpty(req.UrlParameters["elementId"]))
            {
                res.SendErrorJson(StatusCode.BadRequest, new ErrorResponse(new Exception("invalid argument")));
                return;
            }

            var elementId = req.UrlParameters["elementId"];

            try
            {
                var elem = _sessionApp.CurrentWindow.ElementByHandle(elementId);
                if (elem == null)
                {
                    res.SendErrorJson(StatusCode.NotFound, new ErrorResponse(new Exception("stale element reference")));
                    return;
                }
                var rect = new System.Drawing.Rectangle(elem.Location.X, elem.Location.Y, elem.Size.Width, elem.Size.Height);
                // FIXME: wonky
                var base64 = Screenshot.TakeScreenshot((IntPtr)int.Parse(elem.ID, System.Globalization.NumberStyles.HexNumber), rect);
                res.SendJson(new JsonResponse<string>(_session.SessionId, "success", base64));
            }
            catch (Exception e)
            {
                res.SendErrorJson(StatusCode.InternalServerError, new ErrorResponse(e));
            }
        }

        [Route(HttpMethod.POST, "/session/{sessionId}/element")]
        public void FindElement(HttpRequest req, HttpResponse res)
        {
            var data = req.ParseJSON<FindElementRequest>();
            if (!req.UrlParameters.ContainsKey("sessionId") ||
                _session == null ||
                req.UrlParameters["sessionId"] != _session.SessionId)
            {
                res.SendErrorJson(StatusCode.BadRequest, new ErrorResponse(new Exception("invalid argument")));
                return;
            }

            try
            {
                var elems = SessionUtility.FindElements(_sessionApp, data.@using, data.value, null);
                var elem = elems == null? null : elems.FirstOrDefault();
                if (elem == null)
                {
                    res.SendErrorJson(StatusCode.NotFound, new ErrorResponse(new Exception("no such element")));
                }
                else
                {
                    res.SendJson(new JsonResponse<string>(_session.SessionId, "success", elem.ID));
                }
            }
            catch (ArgumentException e)
            {
                res.SendErrorJson(StatusCode.BadRequest, new ErrorResponse(new Exception("invalid argument", e)));
            }
        }

        [Route(HttpMethod.POST, "/session/{sessionId}/elements")]
        public void FindElements(HttpRequest req, HttpResponse res)
        {
            var data = req.ParseJSON<FindElementRequest>();
            if (!req.UrlParameters.ContainsKey("sessionId") ||
                _session == null ||
                req.UrlParameters["sessionId"] != _session.SessionId)
            {
                res.SendErrorJson(StatusCode.BadRequest, new ErrorResponse(new Exception("invalid argument")));
                return;
            }

            try
            {
                var elems = SessionUtility.FindElements(_sessionApp, data.@using, data.value, null);
                var elem = elems == null ? new string[]{ } : elems.Select(e => e.ID).ToArray();
                res.SendJson(new JsonResponse<string[]>(_session.SessionId, "success", elem));
            }
            catch (ArgumentException e)
            {
                res.SendErrorJson(StatusCode.BadRequest, new ErrorResponse(new Exception("invalid argument", e)));
            }
        }

        [Route(HttpMethod.GET, "/session/{sessionId}/element/{elementId}/rect")]
        public void GetElementRect(HttpRequest req, HttpResponse res)
        {
            if (!req.UrlParameters.ContainsKey("sessionId") ||
                _session == null ||
                req.UrlParameters["sessionId"] != _session.SessionId ||
                !req.UrlParameters.ContainsKey("elementId") ||
                string.IsNullOrEmpty(req.UrlParameters["elementId"]))
            {
                res.SendErrorJson(StatusCode.BadRequest, new ErrorResponse(new Exception("invalid argument")));
                return;
            }

            var elementId = req.UrlParameters["elementId"];

            try
            {
                var elem = _sessionApp.CurrentWindow.ElementByHandle(elementId);
                if (elem == null)
                {
                    res.SendErrorJson(StatusCode.NotFound, new ErrorResponse(new Exception("stale element reference")));
                    return;
                }
                var loc = elem.Location;
                var size = elem.Size;
                var rect = new Rect
                {
                    x = loc.X,
                    y = loc.Y,
                    width = size.Width,
                    height = size.Height
                };

                res.SendJson(new JsonResponse<Rect>(_session.SessionId, "success", rect));
            }
            catch (Exception e)
            {
                res.SendErrorJson(StatusCode.InternalServerError, new ErrorResponse(e));
            }
        }

        [Route(HttpMethod.GET, "/session/{sessionId}/element/{elementId}/enabled")]
        public void GetElementEnabled(HttpRequest req, HttpResponse res)
        {
            if (!req.UrlParameters.ContainsKey("sessionId") ||
                _session == null ||
                req.UrlParameters["sessionId"] != _session.SessionId ||
                !req.UrlParameters.ContainsKey("elementId") ||
                string.IsNullOrEmpty(req.UrlParameters["elementId"]))
            {
                res.SendErrorJson(StatusCode.BadRequest, new ErrorResponse(new Exception("invalid argument")));
                return;
            }

            var elementId = req.UrlParameters["elementId"];

            try
            {
                var elem = _sessionApp.CurrentWindow.ElementByHandle(elementId);
                if (elem == null)
                {
                    res.SendErrorJson(StatusCode.NotFound, new ErrorResponse(new Exception("stale element reference")));
                    return;
                }

                res.SendJson(new JsonResponse<bool>(_session.SessionId, "success", elem.Enabled));
            }
            catch (Exception e)
            {
                res.SendErrorJson(StatusCode.InternalServerError, new ErrorResponse(e));
            }
        }

        [Route(HttpMethod.GET, "/session/{sessionId}/element/{elementId}/selected")]
        public void GetElementSelected(HttpRequest req, HttpResponse res)
        {
            if (!req.UrlParameters.ContainsKey("sessionId") ||
                _session == null ||
                req.UrlParameters["sessionId"] != _session.SessionId ||
                !req.UrlParameters.ContainsKey("elementId") ||
                string.IsNullOrEmpty(req.UrlParameters["elementId"]))
            {
                res.SendErrorJson(StatusCode.BadRequest, new ErrorResponse(new Exception("invalid argument")));
                return;
            }

            var elementId = req.UrlParameters["elementId"];

            try
            {
                var elem = _sessionApp.CurrentWindow.ElementByHandle(elementId);
                if (elem == null)
                {
                    res.SendErrorJson(StatusCode.NotFound, new ErrorResponse(new Exception("stale element reference")));
                    return;
                }
                var val = false;
                if (elem is SimpleWinceGuiAutomation.Components.WinceCheckBox)
                {
                    val = ((SimpleWinceGuiAutomation.Components.WinceCheckBox)elem).Checked;
                }
                else if (elem is SimpleWinceGuiAutomation.Components.WinceRadio)
                {
                    val = ((SimpleWinceGuiAutomation.Components.WinceRadio)elem).Checked;
                }
                else if (elem is SimpleWinceGuiAutomation.Components.WinceComboBoxItem)
                {
                    val = ((SimpleWinceGuiAutomation.Components.WinceComboBoxItem)elem).Selected;
                }
                else if (elem is SimpleWinceGuiAutomation.Components.WinceListBoxItem)
                {
                    val = ((SimpleWinceGuiAutomation.Components.WinceListBoxItem)elem).Selected;
                }

                res.SendJson(new JsonResponse<bool>(_session.SessionId, "success", val));
            }
            catch (Exception e)
            {
                res.SendErrorJson(StatusCode.InternalServerError, new ErrorResponse(e));
            }
        }

        [Route(HttpMethod.GET, "/session/{sessionId}/element/{elementId}/text")]
        public void GetElementText(HttpRequest req, HttpResponse res)
        {
            if (!req.UrlParameters.ContainsKey("sessionId") ||
                _session == null ||
                req.UrlParameters["sessionId"] != _session.SessionId ||
                !req.UrlParameters.ContainsKey("elementId") ||
                string.IsNullOrEmpty(req.UrlParameters["elementId"]))
            {
                res.SendErrorJson(StatusCode.BadRequest, new ErrorResponse(new Exception("invalid argument")));
                return;
            }

            var elementId = req.UrlParameters["elementId"];

            try
            {
                var elem = _sessionApp.CurrentWindow.ElementByHandle(elementId);
                if (elem == null)
                {
                    res.SendErrorJson(StatusCode.NotFound, new ErrorResponse(new Exception("stale element reference")));
                    return;
                }
                var val = SessionUtility.GetElementText(elem);

                res.SendJson(new JsonResponse<string>(_session.SessionId, "success", val));
            }
            catch (Exception e)
            {
                res.SendErrorJson(StatusCode.InternalServerError, new ErrorResponse(e));
            }
        }

        [Route(HttpMethod.GET, "/session/{sessionId}/element/{elementId}/name")]
        public void GetElementTagName(HttpRequest req, HttpResponse res)
        {
            if (!req.UrlParameters.ContainsKey("sessionId") ||
                _session == null ||
                req.UrlParameters["sessionId"] != _session.SessionId ||
                !req.UrlParameters.ContainsKey("elementId") ||
                string.IsNullOrEmpty(req.UrlParameters["elementId"]))
            {
                res.SendErrorJson(StatusCode.BadRequest, new ErrorResponse(new Exception("invalid argument")));
                return;
            }

            var elementId = req.UrlParameters["elementId"];

            try
            {
                var elem = _sessionApp.CurrentWindow.ElementByHandle(elementId);
                if (elem == null)
                {
                    res.SendErrorJson(StatusCode.NotFound, new ErrorResponse(new Exception("stale element reference")));
                    return;
                }
                var val = elem.TagName();

                res.SendJson(new JsonResponse<string>(_session.SessionId, "success", val));
            }
            catch (Exception e)
            {
                res.SendErrorJson(StatusCode.InternalServerError, new ErrorResponse(e));
            }
        }

        [Route(HttpMethod.POST, "/reboot")]
        public void RebootDevice(HttpRequest req, HttpResponse res)
        {
            res.SendJson(new
            {
                status = "rebooting"
            });

            // nonstandard!
            Reboot.RebootDevice();
        }

        /*
         * click
         * - scroll into view the element's container
         * - then click into the midpoint of element's rect
         * - If element’s container is obscured by another element, return error with error code element click intercepted. 
         */
        [Route(HttpMethod.POST, "/session/{sessionId}/element/{elementId}/click")]
        public void ElementClick(HttpRequest req, HttpResponse res)
        {
            if (!req.UrlParameters.ContainsKey("sessionId") ||
                _session == null ||
                req.UrlParameters["sessionId"] != _session.SessionId ||
                !req.UrlParameters.ContainsKey("elementId") ||
                string.IsNullOrEmpty(req.UrlParameters["elementId"]))
            {
                res.SendErrorJson(StatusCode.BadRequest, new ErrorResponse(new Exception("invalid argument")));
                return;
            }

            var elementId = req.UrlParameters["elementId"];

            try
            {
                var elem = _sessionApp.CurrentWindow.ElementByHandle(elementId);
                if (elem == null)
                {
                    res.SendErrorJson(StatusCode.NotFound, new ErrorResponse(new Exception("stale element reference")));
                    return;
                }

                elem.ScrollIntoView();                
                if (!elem.VisibleOnScreen() || !elem.Enabled)
                {
                    res.SendErrorJson(StatusCode.BadRequest, new ErrorResponse(new Exception("element not interactable")));
                    return;
                }

                if (elem is SimpleWinceGuiAutomation.Components.WinceButton)
                {
                    ((SimpleWinceGuiAutomation.Components.WinceButton)elem).Click();
                }
                else if (elem is SimpleWinceGuiAutomation.Components.WinceCheckBox)
                {
                    ((SimpleWinceGuiAutomation.Components.WinceCheckBox)elem).Click();
                }
                else if (elem is SimpleWinceGuiAutomation.Components.WinceRadio)
                {
                    ((SimpleWinceGuiAutomation.Components.WinceRadio)elem).Click();
                }
                else if (elem is SimpleWinceGuiAutomation.Components.WinceTextBox)
                {
                    ((SimpleWinceGuiAutomation.Components.WinceTextBox)elem).Click();
                }
                else if (elem is SimpleWinceGuiAutomation.Components.WinceLabel)
                {
                    ((SimpleWinceGuiAutomation.Components.WinceLabel)elem).Click();
                }
                else if (elem is SimpleWinceGuiAutomation.Components.WinceComboBoxItem)
                {
                    ((SimpleWinceGuiAutomation.Components.WinceComboBoxItem)elem).Click();
                }
                else if (elem is SimpleWinceGuiAutomation.Components.WinceListBoxItem)
                {
                    ((SimpleWinceGuiAutomation.Components.WinceListBoxItem)elem).Click();
                }

                res.SendJson(new JsonResponse<object>(_session.SessionId, "success", null));
            }
            catch (Exception e)
            {
                res.SendErrorJson(StatusCode.InternalServerError, new ErrorResponse(e));
            }
        }

        [Route(HttpMethod.POST, "/session/{sessionId}/element/{elementId}/value")]
        public void ElementSendKeys(HttpRequest req, HttpResponse res)
        {
            var data = req.ParseJSON<SendKeysRequest>();

            if (!req.UrlParameters.ContainsKey("sessionId") ||
                _session == null ||
                req.UrlParameters["sessionId"] != _session.SessionId ||
                !req.UrlParameters.ContainsKey("elementId") ||
                string.IsNullOrEmpty(req.UrlParameters["elementId"])
            )
            {
                res.SendErrorJson(StatusCode.BadRequest, new ErrorResponse(new Exception("invalid argument")));
                return;
            }

            var elementId = req.UrlParameters["elementId"];

            try
            {
                var elem = _sessionApp.CurrentWindow.ElementByHandle(elementId);
                if (elem == null)
                {
                    res.SendErrorJson(StatusCode.NotFound, new ErrorResponse(new Exception("stale element reference")));
                    return;
                }

                elem.ScrollIntoView();                
                if (!elem.VisibleOnScreen())
                {
                    res.SendErrorJson(StatusCode.BadRequest, new ErrorResponse(new Exception("element not interactable")));
                    return;
                }

                // TODO: In case the element is not keyboard-interactable, an element not interactable error is returned. 
                // http://bansky.net/blog/simulate-keystrokes-in-windows-mobile/
                // https://github.com/ctacke/sdf/blob/master/Samples/CSharp/SendKeysSample/MainForm.cs
                SimpleWinceGuiAutomation.Wince.WindowHelper.Focus((IntPtr)int.Parse(elem.ID, System.Globalization.NumberStyles.HexNumber));
                OpenNETCF.Windows.Forms.SendKeys.Send(data.text);

                res.SendJson(new JsonResponse<object>(_session.SessionId, "success", null));
            }
            catch (Exception e)
            {
                res.SendErrorJson(StatusCode.InternalServerError, new ErrorResponse(e));
            }
        }

        [Route(HttpMethod.POST, "/session/{sessionId}/element/{elementId}/clear")]
        public void ElementClear(HttpRequest req, HttpResponse res)
        {
            var data = req.ParseJSON<SendKeysRequest>();

            if (!req.UrlParameters.ContainsKey("sessionId") ||
                _session == null ||
                req.UrlParameters["sessionId"] != _session.SessionId ||
                !req.UrlParameters.ContainsKey("elementId") ||
                string.IsNullOrEmpty(req.UrlParameters["elementId"])
            )
            {
                res.SendErrorJson(StatusCode.BadRequest, new ErrorResponse(new Exception("invalid argument")));
                return;
            }

            var elementId = req.UrlParameters["elementId"];

            try
            {
                var elem = _sessionApp.CurrentWindow.ElementByHandle(elementId);
                if (elem == null)
                {
                    res.SendErrorJson(StatusCode.NotFound, new ErrorResponse(new Exception("stale element reference")));
                    return;
                }

                // TODO: In case the element is not keyboard-interactable, an element not interactable error is returned. 
                elem.ScrollIntoView();                
                if (!elem.VisibleOnScreen())
                {
                    res.SendErrorJson(StatusCode.BadRequest, new ErrorResponse(new Exception("element not interactable")));
                    return;
                }
                if (elem is SimpleWinceGuiAutomation.Components.WinceTextBox)
                {
                    var e = ((SimpleWinceGuiAutomation.Components.WinceTextBox)elem);
                    e.Text = ""; // TODO: figure out how to use sendkeys instead???
                }

                res.SendJson(new JsonResponse<object>(_session.SessionId, "success", null));
            }
            catch (Exception e)
            {
                res.SendErrorJson(StatusCode.InternalServerError, new ErrorResponse(e));
            }
        }
    }
}
