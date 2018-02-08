using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleWebDriver.Tests
{
    public static class Assertions
    {
        public static void Equal<T>(T expect, T actual)
            where T : struct, IEquatable<T>
        {
            if (!expect.Equals(actual))
            {
                throw new Exception(string.Format("Expected {0} but got {1}", expect, actual));
            }
        }

        public static void Equal(string expect, string actual)
        {
            if (expect != actual)
            {
                throw new Exception(string.Format("Expected {0} but got {1}", expect, actual));
            }
        }
    }
}
