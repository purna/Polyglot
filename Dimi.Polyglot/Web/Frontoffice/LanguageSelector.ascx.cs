using System;
using System.Text;
using Zeta.EnterpriseLibrary.Web;

namespace Dimi.Polyglot.Web.Frontoffice
{
    /// <summary>
    /// Drop down list box which allows the selection of languages on the front-end of the site
    /// </summary>
    public partial class LanguageSelector : LanguageSelectorUserControl
    {
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

            foreach (var language in AvailableLanguages)
            {
                var selected = string.Empty;

                if (language.ISOCode.ToLower() == SelectedLanguage.ToLower())
                    selected = " selected=\"selected\" ";

                sb.Append(string.Format("<option value=\"{0}\"" + selected + ">{1}</option>", language.ISOCode,
                                        language.Description));
            }
            sb.Append("</select>");
            sb.Append("<noscript><input type=\"submit\" value=\"&gt;\" /></noscript>");
            sb.Append("</form>");
            LanguageDropDownListLiteral.Text = sb.ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Initialise();
            CreateLanguageDropDownList();
        }
    }
}