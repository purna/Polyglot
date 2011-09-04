using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Dimi.Polyglot.Model;

namespace Dimi.Polyglot.BLL
{
    /// <summary>
    /// Business logic for languages (static method)
    /// </summary>
    public static class Languages
    {
        /// <summary>
        /// Gets the default language of the site
        /// </summary>
        /// <returns>The ISO code of the default language</returns>
        public static string GetDefaultLanguage()
        {
            string defaultLanguage;
            try
            {
                defaultLanguage = ConfigurationManager.AppSettings["uPolyglotDefaultLanguage"];
            }
            catch
            {
                defaultLanguage = string.Empty;
            }

            if (string.IsNullOrEmpty(defaultLanguage))
            {
                defaultLanguage = GetLanguages(true)[0].ISOCode;
            }

            return defaultLanguage;
        }

        /// <summary>
        /// Gets the default culture of the site
        /// </summary>
        /// <returns>The code of the default culture, e.g. "en-GB"</returns>
        public static string GetDefaultCulture()
        {
            string defaultCulture;
            try
            {
                defaultCulture = ConfigurationManager.AppSettings["uPolyglotDefaultCulture"];
            }
            catch
            {
                defaultCulture = string.Empty;
            }

            if (string.IsNullOrEmpty(defaultCulture))
            {
                defaultCulture = GetLanguageCulture(GetDefaultLanguage());
            }

            return defaultCulture;
        }

        /// <summary>
        /// Gets the list of languages defined in Umbraco in the Settings section, under "Languages"
        /// </summary>
        /// <returns>The list of languages</returns>
        public static IList<Language> GetLanguages(bool forBackOffice)
        {
            string appendLanguageCodes;
            try
            {
                appendLanguageCodes = ConfigurationManager.AppSettings["uPolyglotAppendLanguageCodes"];
            }
            catch
            {
                appendLanguageCodes = string.Empty;
            }

            IList<Language> languages = new List<Language>();
            var umbracoLanguages =
                umbraco.cms.businesslogic.language.Language.GetAllAsList();
            var sequence = 0;
            foreach (var umbLanguage in umbracoLanguages)
            {
                var isoCode = umbLanguage.CultureAlias.Substring(0, 2);
                var description = forBackOffice ? umbLanguage.FriendlyName : CultureInfo.CreateSpecificCulture(umbLanguage.CultureAlias).NativeName;
                
                if (description.Contains('('))
                    description = description.Substring(0, description.IndexOf('(')).Trim();

                if ((from lang in languages
                     where lang.ISOCode == isoCode
                     select lang).Count() != 0) continue;
                var language = new Language
                                   {
                                       ISOCode = isoCode,
                                       Description = description + (appendLanguageCodes != "false" || forBackOffice ? " (" + isoCode + ")" : string.Empty),
                                       CultureAlias = umbLanguage.CultureAlias,
                                       Sequence = sequence
                                   };
                languages.Add(language);
                sequence++;
            }
            return languages;
        }

        /// <summary>
        /// Checks if the language with the given ISO code has been declared in the Settings section, under "Languages"
        /// </summary>
        /// <param name="isoCode"></param>
        /// <returns>True if the language has been declared; false otherwise</returns>
        public static bool ExistsLanguage(string isoCode)
        {
            var languages = GetLanguages(true);
            return ((from language in languages where language.ISOCode == isoCode select language).Count() > 0);
        }

        /// <summary>
        /// For a language declared in the Settings section, under "Languages", gets the corresponding culture
        /// </summary>
        /// <param name="isoCode">The ISO code of the language</param>
        /// <returns>The culture, e.g. "en-GB"</returns>
        public static string GetLanguageCulture(string isoCode)
        {
            var languages = GetLanguages(true);
            return (from language in languages where language.ISOCode == isoCode select language.CultureAlias).Single();
        }

        /// <summary>
        /// Sets the current culture according to the language passed in as a parameter
        /// </summary>
        /// <param name="languageISOCode">The language ISO code.</param>
        /// <remarks></remarks>
        public static void SetPageCulture(string languageISOCode)
        {
            var regex = new Regex("^[a-zA-Z][a-zA-Z]$");

            string culture;

            if (!string.IsNullOrEmpty(languageISOCode) && regex.IsMatch(languageISOCode) &&
                ExistsLanguage(languageISOCode.ToLower()))
            {
                culture = GetLanguageCulture(languageISOCode.ToLower());
            }
            else
            {
                culture = GetDefaultCulture();
            }

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture);
        }
    }
}