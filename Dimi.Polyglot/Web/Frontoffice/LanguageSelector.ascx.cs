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
    /// <summary>
    /// Drop down list box which allows the selection of languages on the front-end of the site
    /// </summary>
    public partial class LanguageSelector : UserControl
    {
        private IList<Language> _languages;
        private QueryString _queryString;
        private string _selectedCulture;
        private string _selectedLanguage;

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
            else
            {
                _selectedLanguage = Languages.GetDefaultLanguage();
                _selectedCulture = Languages.GetDefaultCulture();
            }

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(_selectedCulture);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(_selectedCulture);
        }

        private void LoadLanguages()
        {
            _languages = Languages.GetLanguages(false);
        }

        private void CreateLanguageDropDownList()
        {
            var redirectQs = new QueryString(Page);
            redirectQs.RemoveParameter("lang");

            var jsRedirect = redirectQs.AllUrl;
            if (redirectQs.Parameters.Count == 0) jsRedirect += "?lang=";
            else jsRedirect += "&lang=";


            var sb = new StringBuilder();
            sb.Append("<form method=\"get\" action=\"" + redirectQs.AllUrl + "\">");
            foreach (string key in redirectQs.Parameters.Keys)
            {
                sb.Append("<input type=\"hidden\" name=\"" + key + "\" value=\"" + redirectQs[key] + "\" />");
            }

            sb.Append("<select id=\"LanguageDropDownList\" name=\"lang\"  onchange=\"javascript:location='" + jsRedirect +
                      "' + this.options[this.selectedIndex].value\" >");

            foreach (var language in _languages)
            {
                var selected = string.Empty;

                if (language.ISOCode == _selectedLanguage.ToLower())
                    selected = " selected=\"selected\" ";

                sb.Append(string.Format("<option value=\"{0}\"" + selected + ">{1}</option>", language.ISOCode,
                                        language.Description));
            }
            sb.Append("</select>");
            sb.Append("<noscript><input type=\"submit\" value=\">\"></noscript>");
            sb.Append("</form>");
            LanguageDropDownListLiteral.Text = sb.ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadQueryString();
            LoadLanguages();
            CreateLanguageDropDownList();
        }
    }
}