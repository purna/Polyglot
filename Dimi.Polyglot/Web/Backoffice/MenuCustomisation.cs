using Umbraco.Core;

namespace Dimi.Polyglot.Web.Backoffice
{
    /// <summary>
    /// Customises Umbraco content tree context menu, adding the Translation creation action
    /// </summary>
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
                var i = new Umbraco.Web.Models.Trees.MenuItem("polyglotCreateTranslations", "[Polyglot] Create translations");
                i.AdditionalData.Add("actionUrl", "/Umbraco/Plugins/Dimi.Polyglot/TranslationCreation.aspx?NodeId=" + e.NodeId);
                i.Icon = "chat";
                e.Menu.Items.Insert(12, i);
            }
        }
    }
}