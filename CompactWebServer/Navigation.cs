using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;

namespace MonoCross.Navigation
{
    /// <summary>
    /// Represents a mapping of a URL pattern to a particular <see cref="IMXController"/> instance
    /// along with any default parameters needed for initialization of the controller.
    /// </summary>
    /// <remarks>Adapted from MonoCross: https://github.com/Zebra/iFactr-NETCF/blob/master/iFactr.UI/MonoCross/Navigation/MXNavigation.cs</remarks>
    public class MXNavigation
    {
        /// <summary>
        /// Gets or sets the navigation URL pattern for the controller.
        /// </summary>
        /// <value>The URL pattern as a <see cref="String"/> value.</value>
        public string Pattern { get; private set; }

        /// <summary>
        /// Gets or sets any parameters to add to the controller.
        /// </summary>
        /// <value>
        /// The parameters as a <see cref="Dictionary&lt;TKey,TValue&gt;"/> instance.
        /// </value>
        public Dictionary<string, string> Parameters { get; private set; }

        MethodInfo _methodInfo;
        public MethodInfo MethodInfo
        {
            get { return _methodInfo; }
        }
        public CompactWebServer.HttpMethod Method { get; private set; }

        public MXNavigation(string pattern, CompactWebServer.HttpMethod method, MethodInfo methodinfo, Dictionary<string, string> parameters)
        {
            Method = method;
            _methodInfo = methodinfo;
            Pattern = pattern;
            Parameters = parameters;
            Parts = Segment.Split(Pattern);
        }

        /// <summary>
        /// <summary>
        /// Converts the <see cref="Pattern"/> into a regex string.
        /// </summary>
        /// <returns>A regex string that represents the pattern.</returns>
        public string RegexPattern()
        {
            return Pattern.Replace("{", "(?<").Replace("}", @">[-&\w\. ]+)");
        }

        /// <summary>
        /// Parses the specified URL for parameters and adds them to the specified dictionary.
        /// </summary>
        /// <param name="url">The URL to parse for parameters.</param>
        /// <param name="parameters">The <see cref="Dictionary&lt;TKey,TValue&gt;"/> to add the parsed parameters to.</param>
        /// <exception cref="ArgumentException">Thrown if the segment count of the <paramref name="url"/> is not equal to the segment count of this instance.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameters"/> is <c>null</c>.</exception>
        public void ExtractParameters(string url, Dictionary<string, string> parameters)
        {
            string[] urlParts = url.Split(new[] { '/' });
            if (urlParts.Length != Parts.Length)
                throw new ArgumentException("URL is not valid for this match!", "url");
            if (parameters == null)
                throw new ArgumentNullException("parameters", "Parameters must have already been allocated!");

            for (int partNumber = 0; partNumber < urlParts.Length; partNumber++)
            {
                if (Parts[partNumber].IsParameter)
                    parameters[Parts[partNumber].SegmentValue] = urlParts[partNumber];
            }
        }

        /// <summary>
        /// Represents a part of a URL.
        /// </summary>
        public class Segment
        {
            /// <summary>
            /// Splits the specified URL into segments and returns the result.
            /// </summary>
            /// <param name="url">The URL to split into segments.</param>
            public static Segment[] Split(string url)
            {
                string[] parts = url.Split(new[] { '/' });
                var segments = new Segment[parts.Length];
                for (int partNumber = 0; partNumber < parts.Length; partNumber++)
                    segments[partNumber] = new Segment(parts[partNumber]);
                return segments;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Segment"/> class.
            /// </summary>
            /// <param name="segment">The URL part to initialize with.</param>
            public Segment(string segment)
            {
                segment = segment.Trim();
                if (segment.Length > 1 && segment[0] == '{' && segment[segment.Length - 1] == '}')
                {
                    // should be a part field, extract the part name
                    SegmentValue = segment.Substring(1, segment.Length - 2);
                    IsParameter = true;
                }
                else
                {
                    SegmentValue = segment;
                    IsParameter = false;
                }
            }

            /// <summary>
            /// Gets whether this instance represents a parameter in the URL.
            /// </summary>
            public bool IsParameter { get; private set; }

            /// <summary>
            /// Gets the value of this instance.
            /// </summary>
            public string SegmentValue { get; private set; }
            //RegEx TypeValidator; // could be added in the future to allow paths such as Customer/{Number:[0-9]* or Customer/{Name:[A-Za-z ]*}
        }

        /// <summary>
        /// Gets an array of <see cref="Segment"/>s that make up the URL pattern.
        /// </summary>
        public Segment[] Parts { get; set; }

        /// <summary>
        /// Determines if the specified object is equal to this instance.
        /// </summary>
        /// <param name="mapping">The object to test for equality.</param>
        /// <returns><c>true</c> if the object is equal to this instance; otherwise <c>false</c>.</returns>
        public bool Equals(MXNavigation mapping)
        {
            if (ReferenceEquals(mapping, null))
                return false;
            if (ReferenceEquals(this, mapping))
                return true;
            if (Method != mapping.Method)
                return false;
            if (Parts.Length != mapping.Parts.Length)
                return false;
            for (int part = 0; part < Parts.Length; part++)
            {
                if (Parts[part].IsParameter != mapping.Parts[part].IsParameter)
                    return false;
                if (!Parts[part].IsParameter && Parts[part].SegmentValue != mapping.Parts[part].SegmentValue)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determines if the specified object is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to test for equality.</param>
        /// <returns><c>true</c> if the object is equal to this instance; otherwise <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is MXNavigation))
            {
                return false;
            }
            return Equals((MXNavigation)obj);
        }

        /// <summary>
        /// Serves as a hash function for a MXNavigation. 
        /// </summary>
        /// <returns>
        /// A hash code for the current MXNavigation.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Pattern != null ? Pattern.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Method.GetHashCode());
                hashCode = (hashCode * 397) ^ (_methodInfo != null ? _methodInfo.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Parameters != null ? Parameters.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Parts != null ? Parts.GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <summary>
        /// Tests for equality between two <see cref="MXNavigation"/> instances.
        /// </summary>
        /// <param name="a">The first MXNavigation to test.</param>
        /// <param name="b">The second MXNavigation to test.</param>
        /// <returns><c>true</c> if the MXNavigations are equal; otherwise <c>false</c>.</returns>
        public static bool operator ==(MXNavigation a, MXNavigation b)
        {
            return ReferenceEquals(a, null) ? ReferenceEquals(b, null) : a.Equals(b);
        }

        /// <summary>
        /// Tests for inequality between two <see cref="MXNavigation"/> instances.
        /// </summary>
        /// <param name="a">The first MXNavigation to test.</param>
        /// <param name="b">The second MXNavigation to test.</param>
        /// <returns><c>true</c> if the MXNavigations are not equal; otherwise <c>false</c>.</returns>
        public static bool operator !=(MXNavigation a, MXNavigation b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="MXNavigation"/>.
        /// </summary>
        public override string ToString()
        {
            return string.Format("[MXNavigation: Pattern={0}, Layer={1}, Method={2}]", Pattern, _methodInfo, Method);
        }
    }

    /// <summary>
    /// Represents a collection of <see cref="MXNavigation"/>s for controllers in an application.
    /// </summary>
    public class NavigationList : List<MXNavigation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationList"/> class.
        /// </summary>
        public NavigationList() { }

        /// <summary>
        /// Gets the controller for the specified URI pattern.
        /// </summary>
        /// <param name="pattern">The URI pattern of the controller.</param>
        /// <returns>The <see cref="IMXController"/> in this instance that matches the pattern.</returns>
        public MethodInfo GetMethodForPattern(string pattern)
        {
            return Contains(pattern) ? this.Where(m => m.Pattern == pattern).First().MethodInfo : null;
        }

        /// <summary>
        /// Determines whether this instance contains the specified pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns><c>true</c> if the pattern is already defined in this instance; otherwise <c>false</c>.</returns>
        public bool Contains(string pattern)
        {
            return this.Any(m => m.Pattern == pattern);
        }

        /// <summary>
        /// Adds the specified controller to the navigation list with the specified string pattern.
        /// </summary>
        /// <param name="pattern">The navigation pattern to associate with the controller.</param>
        /// <param name="controller">The controller to add to the navigation list.</param>
        public void Add(string pattern, MethodInfo controller)
        {
            Add(CompactWebServer.HttpMethod.ANY, pattern, controller, new Dictionary<string, string>());
        }

        /// <summary>
        /// Adds the specified controller to the navigation list with the specified string pattern.
        /// </summary>
        /// <param name="pattern">The navigation pattern to associate with the controller.</param>
        /// <param name="controller">The controller to add to the navigation list.</param>
        /// <param name="parameters">Any default parameters to include when the controller is loaded.</param>
        public void Add(CompactWebServer.HttpMethod method, string pattern, MethodInfo methodinfo, Dictionary<string, string> parameters)
        {
            var mxNavItem = new MXNavigation(pattern, method, methodinfo, parameters);
            InternalAdd(mxNavItem);
        }

        /// <summary>
        /// Adds the specified navigation to the list.
        /// </summary>
        /// <param name="item">The navigation entry to add.</param>
        private void InternalAdd(MXNavigation item)
        {
            Add(item);

            // Enforce uniqueness
            var currentMatches = this.ToList()
                .Where(m => m.Method == item.Method && m.Pattern == item.Pattern).ToList();
            if (currentMatches.Count > 1)
            {
                Remove(item);
#if DEBUG
                string text = string.Format("MapUri \"{0}\" is already matched to Controller type {1}", item.Pattern,
                    currentMatches.FirstOrDefault(i => i != item).MethodInfo);
                throw new Exception(text);
#else
                return;
#endif
            }
        }

        /// <summary>
        /// Returns a <see cref="MXNavigation"/> from the Navigation List that matches the specified URL.
        /// </summary>
        /// <param name="url">A <see cref="String"/> representing the URL to match.</param>
        /// <returns>A <see cref="MXNavigation"/> that matches the URL.</returns>
        public virtual MXNavigation MatchUrl(CompactWebServer.HttpMethod method, string url)
        {
            MXNavigation match = null;

            if (url == string.Empty)
            {
                // figure out what empty matches
                throw new NotImplementedException();
            }
            else
            {
                string[] urlPieces = url.Split('/');

                // first get the mappings with the same number of pieces, then march down one at a time, skipping those with parameters mappings (e.g., '{')
                foreach (var navEntry in this)
                {
                    if (navEntry.Method != CompactWebServer.HttpMethod.ANY && navEntry.Method != method)
                        continue;
                    if (navEntry.Parts.Length != urlPieces.Length)
                        continue;
                    int pieceNumber = 0;
                    for (; pieceNumber < urlPieces.Length; pieceNumber++)
                    {
                        if (!navEntry.Parts[pieceNumber].IsParameter && urlPieces[pieceNumber] != navEntry.Parts[pieceNumber].SegmentValue)
                            // skip parameter match fields
                            break;
                    }
                    if (pieceNumber == urlPieces.Length)
                    {
                        // all NON-parameters matched, found!
                        match = navEntry;
                        break;
                    }
                }
            }

            return match;
        }
    }
}