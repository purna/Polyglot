using Dimi.Polyglot.BLL;

namespace Dimi.Polyglot
{
    /// <summary>
    /// XSLT extensions of the Polyglot package
    /// </summary>
    public class XSLTExtensions
    {
        /// <summary>
        /// Sets the current culture according to the language passed in as a parameter
        /// </summary>
        /// <param name="languageISOCode">The ISO code of the language, the corresponding culture of which will be set as the culture of the page</param>
        public static void SetPageCulture(string languageISOCode)
        {
            Languages.SetPageCulture(languageISOCode);
        }
    }
}