using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SimpleWebDriver.Tests
{
    public class WebDriver
    {
        public WebDriver(string endpoint)
        {
            _endpoint = endpoint;
            _sessionId = "701dccf6-07b8-44a1-850b-433355130a6b";
        }
        string _endpoint;
        string _sessionId;

        public string SessionID
        {
            get { return _sessionId; }
        }

        T CallApi<T>(Func<T> call)
        {
            try
            {
                var res = call();
                return res;
            }
            catch (System.Net.WebException ex)
            {
                if (ex.Status == System.Net.WebExceptionStatus.Timeout) // ex.Response == null
                    throw new Exception("server timed out!");

                var resp = (System.Net.HttpWebResponse)ex.Response;
                if (resp == null)
                {
                    throw ex;
                }

                switch (resp.StatusCode)
                {
                    case System.Net.HttpStatusCode.BadRequest: // 400
                    case System.Net.HttpStatusCode.NotFound: // 404
                    case System.Net.HttpStatusCode.InternalServerError: // 500
                        string json;
                        using (System.IO.StreamReader sr =
                              new System.IO.StreamReader(resp.GetResponseStream()))
                        {
                            json = sr.ReadToEnd().Trim();
                        }
                        throw new Exception("Server returned: \r\n" + json);

                    default:
                        throw ex;
                }
            }
        }

        public void Session(string program, string _args)
        {
            CallApi(() =>
            {
                var obj = new
                {
                    sessionId = _sessionId,
                    capabilities = new
                    {
                        app = program,
                        args = _args
                    }
                };
                using (var res = SimpleWebClient.PostJson(_endpoint + "session", obj)) {
                    return 0;
                }
            });
        }

        public void SessionDrop()
        {
            CallApi(() =>
            {
                using (var res = SimpleWebClient.Delete(_endpoint + "session/" + _sessionId))
                {
                    return res;
                }
            });
        }

        public JObject GetSessionCommand(string command)
        {
            return CallApi(() =>
            {
                using (var resp = SimpleWebClient.GetJson(_endpoint + "session/" + _sessionId + "/" + command))
                {
                    string json;
                    using (System.IO.StreamReader sr =
                          new System.IO.StreamReader(resp.GetResponseStream()))
                    {
                        json = sr.ReadToEnd().Trim();
                    }
                    var obj = JObject.Parse(json);
                    return obj;
                }
            });
        }

        public JObject PostSessionCommand(string command, object payload)
        {
            return CallApi(() =>
            {
                using (var resp = SimpleWebClient.PostJson(_endpoint + "session/" + _sessionId + "/" + command, payload))
                {
                    string json;
                    using (System.IO.StreamReader sr =
                          new System.IO.StreamReader(resp.GetResponseStream()))
                    {
                        json = sr.ReadToEnd().Trim();
                    }
                    var obj = JObject.Parse(json);
                    return obj;
                }
            });
        }

        public string GetElement(string @using, string value)
        {
            var res = PostSessionCommand("element", new
            {
                @using = @using,
                value = value
            });

            Assertions.Equal("success", res.Value<string>("status"));
            var x = res.Value<string>("value");
            return x;
        }

        public IEnumerable<string> GetElements(string @using, string value)
        {
            var res = PostSessionCommand("elements", new
            {
                @using = @using,
                value = value
            });

            Assertions.Equal("success", res.Value<string>("status"));
            var elements = (JArray)res["value"];

            foreach (var elem in elements)
            {
                var handle = elem.Value<string>();
                yield return handle;
            }            
        }

        public void Click(string handle)
        {
            PostSessionCommand("element/" + handle + "/click", new object());
        }

        public void SendKeys(string handle, string p)
        {
            PostSessionCommand("element/" + handle + "/value", new
            {
                text = p
            });
        }

        public void Clear(string handle)
        {
            PostSessionCommand("element/" + handle + "/clear", new object());
        }
    }
}
