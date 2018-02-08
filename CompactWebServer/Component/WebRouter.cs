using System.Collections.Generic;
using System;
using System.Reflection;

namespace CompactWebServer
{
    public class WebRouter
    {
        MonoCross.Navigation.NavigationList _list = new MonoCross.Navigation.NavigationList();

        public void Add(HttpMethod method, string path, MethodInfo handler)
        {
            _list.Add(method, path, handler, null);
        }
        public void Add(HttpMethod method, string path, MethodInfo handler, Dictionary<string,string> parameters)
        {
            _list.Add(method, path, handler, parameters);
        }

        public void Add(string path, MethodInfo handler)
        {
            this.Add(HttpMethod.ANY, path, handler);
        }

        public bool Contain(HttpMethod method, string path)
        {
            var nx = _list.MatchUrl(method, path);
            return (nx != null);
        }

        public MethodInfo Get(HttpMethod method, string path, Dictionary<string,string> parameters)
        {
            var nx = _list.MatchUrl(method, path);
            if (nx == null)
                throw new ArgumentException("path");
            nx.ExtractParameters(path, parameters);
            return nx.MethodInfo;
        }
    }
}