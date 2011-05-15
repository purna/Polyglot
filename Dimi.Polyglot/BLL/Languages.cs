using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
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
                defaultLanguage = GetLanguages()[0].ISOCode;
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
        public static IList<Language> GetLanguages()
        {
            IList<Language> languages = new List<Language>();
              IEnumerable<umbraco.cms.businesslogic.language.Language> umbracoLanguages = 
                  umbraco.cms.businesslogic.language.Language.GetAllAsList();
              int sequence = 0;
                foreach (umbraco.cms.businesslogic.language.Language umbLanguage in umbracoLanguages)
                {
                    string isoCode = umbLanguage.CultureAlias.Substring(0, 2);
                    string description = umbLanguage.FriendlyName;

                    if (description.Contains('('))
                        description = description.Substring(0, description.IndexOf('(')).Trim();

                    if ((from lang in languages
                         where lang.ISOCode == isoCode
                         select lang).Count() == 0)
                    {
                        Language language = new Language
                        {
                            ISOCode = isoCode,
                            Description = description + " (" + isoCode + ")",
                            CultureAlias = umbLanguage.CultureAlias,
                            Sequence = sequence
                        };
                        languages.Add(language);
                        sequence++;
                    }
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
            IList<Language> languages = GetLanguages();
            return ((from language in languages where language.ISOCode == isoCode select language).Count() > 0);
        }

        /// <summary>
        /// For a language declared in the Settings section, under "Languages", gets the corresponding culture
        /// </summary>
        /// <param name="isoCode">The ISO code of the language</param>
        /// <returns>The culture, e.g. "en-GB"</returns>
        public static string GetLanguageCulture(string isoCode)
        {
            IList<Language> languages = GetLanguages();
            return (from language in languages where language.ISOCode == isoCode select language.CultureAlias).Single();
        }
    }
}