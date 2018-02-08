using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SimpleWebDriver
{
    public class ErrorResponse
    {
        public ErrorResponse(Exception ex)
        {
            value = new ErrorResponseValue();
            value.error = ex.Message;
            value.stacktrace = ex.StackTrace;
            value.message = "";
        }
        public ErrorResponseValue value { get;set; }
    }
    public class ErrorResponseValue
    {
        public string error {get;set;}
        public string message {get;set;}
        public string stacktrace {get;set;}
    }
}
