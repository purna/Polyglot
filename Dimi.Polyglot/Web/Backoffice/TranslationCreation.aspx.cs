using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dimi.Polyglot.BLL;
using Dimi.Polyglot.Extensions;
using Zeta.EnterpriseLibrary.Web;
using umbraco.BasePages;
using umbraco.cms.businesslogic.web;

namespace Dimi.Polyglot.Web.Backoffice
{
    /// <summary>
    /// Page that manages the creation of translation nodes in the back office of the site
    /// </summary>
    public partial class TranslationCreation : BasePage
    {
        private int _nodeId;
        private QueryString _queryString;

        protected void Page_Load(object sender, EventArgs e)
        {
            _queryString = new QueryString(Page);
            _nodeId = _queryString["NodeID"].ToInt();
            var currentDocument = new Document(_nodeId);
            DocumentDescription.Text = currentDocument.Text;

            var status = DocumentTranslation.CheckTranslationInfrastructure(_nodeId);
            if (status == "ok")
            {
                if (!IsPostBack)
                {
                    var languagesToDisplay = Languages.GetLanguages(true);

                    var hideDefaultLanguage = ConfigurationManager.AppSettings["PolyglotHideDefaultLanguageCheckbox"];

                    if (!string.IsNullOrEmpty(hideDefaultLanguage) && hideDefaultLanguage.ToLower() == "true")
                    {
                        languagesToDisplay.Remove(
                            languagesToDisplay.Single(x => x.CultureAlias == Languages.GetDefaultCulture()));
                    }

                    CheckBoxList1.DataSource = languagesToDisplay;
                    CheckBoxList1.DataValueField = "ISOCode";
                    CheckBoxList1.DataTextField = "Description";
                    CheckBoxList1.DataBind();

                    foreach (ListItem li in CheckBoxList1.Items)
                    {
                        if (DocumentTranslation.TranslationNodeExists(_nodeId, li.Value))
                        {
                            li.Selected = false;
                            li.Enabled = false;
                        }
                        else
                        {
                            if (li.Value.ToLower() != Languages.GetDefaultLanguage().ToLower()) li.Selected = true;
                            li.Enabled = true;
                        }
                    }
                }

                var savedParam = _queryString["saved"].ToStr();
                if (!string.IsNullOrEmpty(savedParam))
                {
                    switch (savedParam)
                    {
                        case "ok":
                            {
                                ((BasePage) HttpContext.Current.Handler).ClientTools.ShowSpeechBubble(
                                    speechBubbleIcon.save, "Successful creation",
                                    "Translation documents created. Please reload nodes.");
                                break;
                            }
                        case "NoLangProp":
                            {
                                ((BasePage) HttpContext.Current.Handler).ClientTools.ShowSpeechBubble(
                                    speechBubbleIcon.error, "Error",
                                    "Creation failed because there is no Label field with the alias 'language' in the translation document type");
                                break;
                            }
                        default:
                            {
                                ((BasePage) HttpContext.Current.Handler).ClientTools.ShowSpeechBubble(
                                    speechBubbleIcon.error, "Error",
                                    "There was an error creating the translation documents");
                                break;
                            }
                    }
                }
            }
            else
            {
                PropertyPanel1.Visible = false;
                PropertyPanel2.Visible = true;
                ((BasePage) HttpContext.Current.Handler).ClientTools.ShowSpeechBubble(speechBubbleIcon.warning,
                                                                                      "Infrastructure issue", status);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            /*** SAVE ***/
            ImageButton save = Panel1.Menu.NewImageButton();
            save.ImageUrl = "/umbraco/images/editor/save.gif";
            save.AlternateText = "Save";

            save.Click += save_Click;

            /*** MULTI_LANGUAGE_SELECT ***/
            CheckBoxMultiLanguageSelect.AutoPostBack = true;
            CheckBoxMultiLanguageSelect.CheckedChanged += CheckBoxMultiLanguageSelect_CheckedChanged;
        }

        private void CheckBoxMultiLanguageSelect_CheckedChanged(object sender, EventArgs e)
        {
            if (DocumentTranslation.GetTranslationFolderContentType(_nodeId) != null)
            {
                var s = sender as CheckBox;

                foreach (ListItem li in CheckBoxList1.Items)
                {
                    if (li.Enabled)
                    {
                        li.Selected = s.Checked ? true : false;
                    }
                }
            }
        }

        private void save_Click(object sender, ImageClickEventArgs e)
        {
            if (DocumentTranslation.GetTranslationFolderContentType(_nodeId) != null)
            {
                var saved = "ok";
                try
                {
                    if (!DocumentTranslation.TranslationFolderExists(_nodeId))
                        if (!DocumentTranslation.TranslationFolderCreate(_nodeId)) throw new Exception();

                    if (CheckBoxList1.Items.Cast<ListItem>().Where(li => li.Selected).Any(li => !DocumentTranslation.TranslationNodeCreate(_nodeId, li.Value)))
                    {
                        throw new Exception();
                    }

                    DocumentTranslation.SortTranslationNodes(_nodeId);
                }
                catch (Exception ex)
                {
                    saved = ex.Message == "NoLangProp" ? "NoLangProp" : "failed";
                }

                Response.Redirect(_queryString.BeforeUrl + "?NodeID=" + _nodeId.ToStr() + "&saved=" + saved);
            }
        }
    }
}