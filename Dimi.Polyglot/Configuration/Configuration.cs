using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using Dimi.Polyglot.Model;

namespace Dimi.Polyglot.Configuration
{
    /// <summary>
    /// Configuration for Polyglot
    /// </summary>
    public static class Configuration
    {
        private static readonly object ThreadSafetyLock = new object();
        private static XDocument _document;
        private static IList<Language> _nonStandardCultures;

        /// <summary>
        /// Loads the XML configuration document.
        /// </summary>
        private static void LoadDocument()
        {
            if (_document == null)
            {
                lock (ThreadSafetyLock)
                {
                    // Load document with mappath if on the web. Otherwise use direct
                    // path, since we're probably unit testing.
                    _document = XDocument.Load(System.Web.Hosting.HostingEnvironment.IsHosted ?
                         System.Web.Hosting.HostingEnvironment.MapPath("/config/Polyglot.config") :
                        "..\\..\\..\\Dimi.Polyglot\\config\\PolyglotTest.config");
                }
                LoadNonStandardCultures();
            }
        }

        /// <summary>
        /// Loads the non-standard cultures from the configuration file.
        /// </summary>
        private static void LoadNonStandardCultures()
        {
            _nonStandardCultures = new List<Language>();
            var elements = _document.XPathSelectElements("/PolyglotConfiguration/nonStandardCultures/culture");
            foreach (var element in elements)
            {
                var isoCode = element.Attribute("ISOCode");
                var description = element.Attribute("Description");
                var cultureAlias = element.Attribute("CultureAlias");
                _nonStandardCultures.Add(new Language()
                    {
                        ISOCode = isoCode.Value,
                        Description = description.Value,
                        CultureAlias = cultureAlias.Value
                    });

            }
        }

        public static IList<Language> NonStandardCultures
        {
            get
            {
                LoadDocument();
                return _nonStandardCultures;
            }
        }


    }
}