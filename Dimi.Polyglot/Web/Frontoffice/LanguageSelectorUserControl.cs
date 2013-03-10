using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.UI;
using Dimi.Polyglot.BLL;
using Dimi.Polyglot.Model;
using Zeta.EnterpriseLibrary.Web;

namespace Dimi.Polyglot.Web.Frontoffice
{
    public class LanguageSelectorUserControl : UserControl
    {
        private IList<Language> _languages;
        private QueryString _queryString;
        private string _selectedCulture;
        private string _selectedLanguage;

        /// <summary>
        /// Loads the query string.
        /// </summary>
        /// <remarks></remarks>
        private void LoadQueryString()
        {
            _queryString = new QueryString(Page);

            var regex = new Regex("^[a-zA-Z][a-zA-Z]$");

            _selectedLanguage = _queryString["lang"];

            if (!string.IsNullOrEmpty(_selectedLanguage) && regex.IsMatch(_selectedLanguage) &&
                Languages.ExistsLanguage(_selectedLanguage.ToLower()))
            {
                _selectedCulture = Languages.GetLanguageCulture(_selectedLanguage.ToLower());
            }
            else if (Regex.Matches(Request.RawUrl, "/([A-z]{2})/").Count > 0)
            {
                _selectedLanguage = Regex.Matches(Request.RawUrl, "/([A-z]{2})/")[0].Value.ToLower().Replace("/",
                                                                                                             string
                                                                                                                 .Empty);
                if (Languages.ExistsLanguage(_selectedLanguage))
                {
                    _selectedCulture = Languages.GetLanguageCulture(_selectedLanguage.ToLower());
                }
                else
                {
                    Response.Redirect("/");
                }
            }
            else
            {
                _selectedLanguage = Languages.GetDefaultLanguage();
                _selectedCulture = Languages.GetDefaultCulture();
            }

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(_selectedCulture);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(_selectedCulture);
        }

        /// <summary>
        /// Loads the languages.
        /// </summary>
        /// <remarks></remarks>
        private void LoadLanguages()
        {
            _languages = Languages.GetLanguages(false);
        }

        /// <summary>
        /// Initialises this instance.
        /// </summary>
        /// <remarks></remarks>
        protected void Initialise()
        {
            LoadQueryString();
            LoadLanguages();
        }

        /// <summary>
        /// Gets the available languages.
        /// </summary>
        /// <remarks></remarks>
        protected IList<Language> AvailableLanguages
        {
            get
            {
                return _languages;
            }
        }

        /// <summary>
        /// Gets the selected culture.
        /// </summary>
        /// <remarks></remarks>
        protected string SelectedCulture
        {
            get
            {
                return _selectedCulture;
            }
        }

        /// <summary>
        /// Gets the selected language.
        /// </summary>
        /// <remarks></remarks>
        protected string SelectedLanguage
        {
            get
            {
                return _selectedLanguage;
            }
        }

        /// <summary>
        /// Gets the page query string.
        /// </summary>
        /// <remarks></remarks>
        protected QueryString PageQueryString
        {
            get { return _queryString; }
        }

        /// <summary>
        /// Gets the page query string with the lang parameter empty at the end.
        /// </summary>
        /// <remarks></remarks>
        protected string PageQueryStringEmptyLang
        {
            get
            {
                LoadQueryString();
                var redirectQs = PageQueryString;
                redirectQs.RemoveParameter("lang");

                var queryString = redirectQs.AllUrl;
                if (redirectQs.Parameters.Count == 0) queryString += "?lang=";
                else queryString += "&lang=";
                return queryString;
            }
        }

    }
}