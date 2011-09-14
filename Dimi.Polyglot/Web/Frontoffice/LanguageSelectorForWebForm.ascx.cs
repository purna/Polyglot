using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Dimi.Polyglot.Web.Frontoffice
{
    public partial class LanguageSelectorForWebForm : LanguageSelectorUserControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            Initialise();
            LanguageDropDownList.DataSource = AvailableLanguages;
            LanguageDropDownList.DataValueField = "ISOCode";
            LanguageDropDownList.DataTextField = "Description";
            LanguageDropDownList.DataBind();
            var selected = LanguageDropDownList.Items.FindByValue(SelectedLanguage);
            if (selected != null) selected.Selected = true;
        }

        protected void LanguageDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Page.Response.Redirect(PageQueryStringEmptyLang + LanguageDropDownList.SelectedValue);
        }

        protected void LanguageSubmitNoScript_Click(object sender, EventArgs e)
        {
            Page.Response.Redirect(PageQueryStringEmptyLang + LanguageDropDownList.SelectedValue);
        }
    }
}