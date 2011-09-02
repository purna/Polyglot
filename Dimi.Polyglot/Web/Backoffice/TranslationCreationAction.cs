using umbraco.interfaces;

namespace Dimi.Polyglot.Web.Backoffice
{
    /// <summary>
    /// The translation creation action
    /// </summary>
    public class TranslationCreationAction : IAction
    {
        #region IAction Members

        public string Alias
        {
            get { return "Create translations"; }
        }

        public bool CanBePermissionAssigned
        {
            get { return true; }
        }

        public string Icon
        {
            get { return "/umbraco/images/umbraco/settingLanguage.gif"; }
        }

        public string JsFunctionName
        {
            get { return "openTranslationCreation()"; }
        }

        public string JsSource
        {
            get
            {
                return
                    @"
                    function openTranslationCreation() {
                        if (UmbClientMgr.mainTree().getActionNode().nodeId != '-1' && UmbClientMgr.mainTree().getActionNode().nodeType != '') {
                            parent.right.document.location.href = 'plugins/Dimi.Polyglot/TranslationCreation.aspx?NodeID=' + UmbClientMgr.mainTree().getActionNode().nodeId;
	                        return false;
                        }
                    }
                    ";
            }
        }

        public char Letter
        {
            get { return ' '; }
        }

        public bool ShowInNotifier
        {
            get { return false; }
        }

        #endregion
    }
}