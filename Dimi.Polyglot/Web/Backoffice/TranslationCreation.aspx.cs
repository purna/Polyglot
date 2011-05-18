using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Zeta.EnterpriseLibrary.Web;
using Dimi.Polyglot.Extensions;

namespace Dimi.Polyglot.Web.Backoffice
{
    /// <summary>
    /// Page that manages the creation of translation nodes in the back office of the site
    /// </summary>
    public partial class TranslationCreation : umbraco.BasePages.BasePage
    {
        QueryString queryString;
        int NodeID;

        protected void Page_Load(object sender, EventArgs e)
        {
            queryString = new QueryString(Page);
            NodeID = queryString["NodeID"].ToInt();
            umbraco.cms.businesslogic.web.Document currentDocument = new umbraco.cms.businesslogic.web.Document(NodeID);
            DocumentDescription.Text = currentDocument.Text;

            //if (BLL.DocumentTranslation.GetTranslationFolderContentType(NodeID) != null)

            string status = BLL.DocumentTranslation.CheckTranslationInfrastructure(NodeID);
            if (status == "ok")
            {
                if (!IsPostBack)
                {
                    CheckBoxList1.DataSource = BLL.Languages.GetLanguages();
                    CheckBoxList1.DataValueField = "ISOCode";
                    CheckBoxList1.DataTextField = "Description";
                    CheckBoxList1.DataBind();

                    foreach (ListItem li in CheckBoxList1.Items)
                    {
                        if (BLL.DocumentTranslation.TranslationNodeExists(NodeID, li.Value))
                        {
                            li.Selected = false;
                            li.Enabled = false;
                        }
                        else
                        {
                            if (li.Value.ToLower() != BLL.Languages.GetDefaultLanguage()) li.Selected = true;
                            li.Enabled = true;
                        }
                    }
                }

                string savedParam = queryString["saved"].ToStr();
                if (!string.IsNullOrEmpty(savedParam))
                {
                    switch (savedParam)
                    {
                        case "ok":
                            {
                                ((umbraco.BasePages.BasePage)HttpContext.Current.Handler).ClientTools.ShowSpeechBubble(speechBubbleIcon.save, "Successful creation", "Translation documents created. Please reload nodes.");
                                break;
                            }
                        case "NoLangProp":
                            {
                                ((umbraco.BasePages.BasePage)HttpContext.Current.Handler).ClientTools.ShowSpeechBubble(speechBubbleIcon.error, "Error", "Creation failed because there is no Label field with the alias 'language' in the translation document type");
                                break;
                            }
                        default:
                            {
                                ((umbraco.BasePages.BasePage)HttpContext.Current.Handler).ClientTools.ShowSpeechBubble(speechBubbleIcon.error, "Error", "There was an error creating the translation documents");
                                break;
                            }
                    }
                }
            }
            else
            {
                PropertyPanel1.Visible = false;
                PropertyPanel2.Visible = true;
                ((umbraco.BasePages.BasePage)HttpContext.Current.Handler).ClientTools.ShowSpeechBubble(speechBubbleIcon.warning, "Infrastructure issue", status);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            /*** SAVE ***/
            ImageButton save = Panel1.Menu.NewImageButton();
            save.ImageUrl = "/umbraco/images/editor/save.gif";
            save.AlternateText = "Save";

            save.Click += new ImageClickEventHandler(save_Click);

            /*** MULTI_LANGUAGE_SELECT ***/
            CheckBoxMultiLanguageSelect.AutoPostBack = true;
            CheckBoxMultiLanguageSelect.CheckedChanged += new EventHandler(CheckBoxMultiLanguageSelect_CheckedChanged);
        }

        void CheckBoxMultiLanguageSelect_CheckedChanged(object sender, EventArgs e)
        {
            if (BLL.DocumentTranslation.GetTranslationFolderContentType(NodeID) != null)
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

        void save_Click(object sender, ImageClickEventArgs e)
        {
            if (BLL.DocumentTranslation.GetTranslationFolderContentType(NodeID) != null)
            {
                string saved = "ok";
                try
                {
                    if (!BLL.DocumentTranslation.TranslationFolderExists(NodeID))
                        if (!BLL.DocumentTranslation.TranslationFolderCreate(NodeID)) throw new Exception();

                    foreach (ListItem li in CheckBoxList1.Items)
                    {
                        if (li.Selected)
                        {
                            if (!BLL.DocumentTranslation.TranslationNodeCreate(NodeID, li.Value))
                                throw new Exception();
                        }
                    }

                    BLL.DocumentTranslation.SortTranslationNodes(NodeID);
                }
                catch(Exception ex)
                {
                    if (ex.Message == "NoLangProp") saved = "NoLangProp";
                    else saved = "failed";
                }

                Response.Redirect(queryString.BeforeUrl + "?NodeID=" + NodeID.ToStr() + "&saved=" + saved);
            }
        }
    }
}