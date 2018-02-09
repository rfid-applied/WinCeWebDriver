using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SimpleWebDriver.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var address = args.Length > 0 ? args[0] : "192.168.0.3";
            var port = args.Length > 1 ? args[1] : "8080";

            Console.WriteLine("USAGE: tests.exe address port");

            var endpoint = string.Format("http://{0}:{1}/", address, port);
            Console.WriteLine("Executing tests against {0}", endpoint);

            Type[] allTypes = Assembly.GetExecutingAssembly().GetTypes();
            RunTests(endpoint, allTypes);
        }

        static void RunTests(string endpoint, Type[] allTypes)
        {
            var succ = 0;
            var fail = 0;
            var sw = new System.Diagnostics.Stopwatch();

            sw.Start();

            Type baseType = typeof(BaseTest);
            foreach (Type test in allTypes)
            {
                if (!test.IsSubclassOf(baseType))
                    continue;

                MethodInfo[] methods = test.GetMethods();

                foreach (MethodInfo method in methods)
                {
                    ParameterInfo[] parameters = method.GetParameters();

                    // only process public method has 2 parameters with one is HttpRequest and another is HttpResponse
                    if (!method.IsPublic || !method.IsStatic || !(parameters.Length == 1) || !(parameters[0].ParameterType == typeof(string)))
                        continue;

                    try
                    {
                        method.Invoke(null, new[] { endpoint });
                        
                        succ++;
                        Console.WriteLine("SUCCESS: test {0}", method.Name);
                    }
                    catch (TargetInvocationException e1)
                    {
                        var e = e1.InnerException;
                        fail++;
                        Console.WriteLine("ERROR: test {0} failed with exception: {1} failed at {2}", method.Name, e.Message, e.StackTrace);
                    }
                }
            }

            sw.Stop();
            Console.WriteLine("Run {0} tests in {1}, {2} succeded, {3} failed", succ + fail, sw.Elapsed, succ, fail);
        }
    }

    public class BaseTest
    {
    }
}
