using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimi.Polyglot.Web.Backoffice
{
    /// <summary>
    /// Customises Umbraco content tree context menu, adding the Translation creation action
    /// </summary>
    public class MenuCustomisation : umbraco.BusinessLogic.ApplicationBase
    {
        public MenuCustomisation()
        {
            umbraco.cms.presentation.Trees.BaseTree.BeforeNodeRender += new umbraco.cms.presentation.Trees.BaseTree.BeforeNodeRenderEventHandler(BaseTree_BeforeNodeRender);
        }

        void BaseTree_BeforeNodeRender(ref umbraco.cms.presentation.Trees.XmlTree sender, ref umbraco.cms.presentation.Trees.XmlTreeNode node, EventArgs e)
        {
            if (node.Menu != null && node.NodeType.ToLower() == "content")
            {
                node.Menu.Insert(3, new TranslationCreationAction());
                node.Menu.Insert(3, umbraco.BusinessLogic.Actions.ContextMenuSeperator.Instance);
                node.Menu.Insert(5, umbraco.BusinessLogic.Actions.ContextMenuSeperator.Instance);
            }
        }
    }
}