using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SimpleWebDriver
{
    public class JsonResponse<T>
    {
        public JsonResponse(string sessionID, string status, T value) {
            this.sessionId = sessionID;
            this.status = status;
            this.value = value;
        }
        public string sessionId { get; set; }
        public string status { get; set; }
        public T value { get; set; }
    }
}
