using System;
using umbraco.BusinessLogic;
using umbraco.BusinessLogic.Actions;
using umbraco.cms.presentation.Trees;

namespace Dimi.Polyglot.Web.Backoffice
{
    /// <summary>
    /// Customises Umbraco content tree context menu, adding the Translation creation action
    /// </summary>
    public class MenuCustomisation : ApplicationBase
    {
        public MenuCustomisation()
        {
            BaseTree.BeforeNodeRender += BaseTree_BeforeNodeRender;
        }

        private void BaseTree_BeforeNodeRender(ref XmlTree sender, ref XmlTreeNode node, EventArgs e)
        {
            if (node.Menu == null || node.NodeType.ToLower() != "content") return;
            node.Menu.Insert(3, new TranslationCreationAction());
            node.Menu.Insert(3, ContextMenuSeperator.Instance);
            node.Menu.Insert(5, ContextMenuSeperator.Instance);
        }
    }
}