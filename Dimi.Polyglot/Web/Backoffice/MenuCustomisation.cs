using System;
using umbraco.BusinessLogic;
using umbraco.BusinessLogic.Actions;
using umbraco.cms.presentation.Trees;
using Umbraco.Core;

namespace Dimi.Polyglot.Web.Backoffice
{
    /// <summary>
    /// Customises Umbraco content tree context menu, adding the Translation creation action
    /// </summary>
    //public class MenuCustomisation : ApplicationBase
    //{
    //    public MenuCustomisation()
    //    {
    //        BaseTree.BeforeNodeRender += BaseTree_BeforeNodeRender;
    //    }

    //    private void BaseTree_BeforeNodeRender(ref XmlTree sender, ref XmlTreeNode node, EventArgs e)
    //    {
    //        if (node.Menu == null || node.NodeType.ToLower() != "content") return;
    //        node.Menu.Insert(3, new TranslationCreationAction());
    //        node.Menu.Insert(3, ContextMenuSeperator.Instance);
    //        node.Menu.Insert(5, ContextMenuSeperator.Instance);
    //    }
    //}

    public class MenuCustomisation : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            Umbraco.Web.Trees.ContentTreeController.MenuRendering += ContentTreeController_MenuRendering;
        }

        void ContentTreeController_MenuRendering(Umbraco.Web.Trees.TreeControllerBase sender, Umbraco.Web.Trees.MenuRenderingEventArgs e)
        {

            if (!string.IsNullOrEmpty(sender.TreeAlias) && sender.TreeAlias.ToLower() == "content")
            {
                //creates a menu action that will open /umbraco/currentSection/itemAlias.html
                var i = new Umbraco.Web.Models.Trees.MenuItem("polyglotCreateTranslations", "Create translations");

                //optional, if you want to load a legacy page, otherwise it will just follow convention
                i.AdditionalData.Add("actionUrl", "/Umbraco/Plugins/Dimi.Polyglot/TranslationCreation.aspx?NodeId=" + e.NodeId);

                //sets the icon to icon-wine-glass 
                i.Icon = "polyglot";

                //insert at index 5
                e.Menu.Items.Insert(5, i);
            }
        }
    }
}