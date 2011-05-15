using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Zeta.EnterpriseLibrary.Web;
using Dimi.Polyglot.BLL;
using Dimi.Polyglot.Model;
using System.Text;

namespace Dimi.Polyglot.Web.Frontoffice
{
    /// <summary>
    /// Drop down list box which allows the selection of languages on the front-end of the site
    /// </summary>
    public partial class LanguageSelector : System.Web.UI.UserControl
    {
        private QueryString _queryString;
        private string _selectedLanguage;
        private string _selectedCulture;

        IList<Language> _languages;

        private void LoadQueryString()
        {
            _queryString = new QueryString(Page);

            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[a-zA-Z][a-zA-Z]$");

            _selectedLanguage = _queryString["lang"];

            if (!string.IsNullOrEmpty(_selectedLanguage) && regex.IsMatch(_selectedLanguage) && Languages.ExistsLanguage(_selectedLanguage.ToLower()))
            {
                string culture = Languages.GetLanguageCulture(_selectedLanguage.ToLower());
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);
                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(culture);
            }
            else
            {
                _selectedLanguage = Languages.GetDefaultLanguage();
                _selectedCulture = Languages.GetDefaultCulture();
            }

           
        }

        private void LoadLanguages()
        {
             _languages = Languages.GetLanguages();
            
        }

        private void CreateLanguageDropDownList()
        {

            QueryString redirectQs = new QueryString(Page);
            redirectQs.RemoveParameter("lang");

            string jsRedirect = redirectQs.AllUrl;
            if (redirectQs.Parameters.Count == 0) jsRedirect += "?lang=";
            else jsRedirect += "&lang=";



            StringBuilder sb = new StringBuilder();
            sb.Append("<form method=\"get\" action=\""+redirectQs.AllUrl + "\">");
            foreach (string key in redirectQs.Parameters.Keys)
            {
                sb.Append("<input type=\"hidden\" name=\"" + key + "\" value=\"" + redirectQs[key] + "\" />");
            }

            sb.Append("<select id=\"LanguageDropDownList\" name=\"lang\"  onchange=\"javascript:location='" + jsRedirect + "' + this.options[this.selectedIndex].value\" + >");
          
            foreach (Language language in _languages)
            {
                string selected = string.Empty;

                if (language.ISOCode == _selectedLanguage.ToLower())
                    selected = " selected=\"selected\" ";

                sb.Append(string.Format("<option value=\"{0}\"" + selected + ">{1}</option>", language.ISOCode, language.Description));
            }
            sb.Append("</select>");
            sb.Append("<noscript><input type=\"submit\" value=\">\"></button></noscript>");
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