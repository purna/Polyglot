using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using umbraco.cms.businesslogic.web;
using umbraco.BusinessLogic;
using Dimi.Polyglot.Model;

namespace Dimi.Polyglot.BLL
{
    /// <summary>
    /// Business logic of document translations (static methods)
    /// </summary>
    public static class DocumentTranslation
    {
        private const string TranslationFolderAliasSuffix = "_TranslationFolder";
        private const string TranslationFolderName = "Translations";
        private const string LanguagePropertyAlias = "language";

        /// <summary>
        /// For a given node in Umbraco, find the corresponding document type of the 
        /// corresponding translations folder. This is the allowed child document type
        /// the type alias of which is made up of the type alias of the given node
        /// followed by the suffix contained in the TranslationFolderAliasSuffix constant
        /// (see above), if it exists
        /// </summary>
        /// <param name="NodeID">The id of thenode</param>
        /// <returns>The document type of the translation folder, if it exists; otherwise null</returns>
        public static DocumentType GetTranslationFolderContentType(int NodeID)
        {
            DocumentType folderContentType = null;

            Document nodeDoc = new Document(NodeID);

            DocumentType nodeDocContentType = new DocumentType(nodeDoc.ContentType.Id);
            foreach (int contentTypeId in nodeDocContentType.AllowedChildContentTypeIDs)
            {
                DocumentType contentType = new DocumentType(contentTypeId);
                if (contentType.Alias.EndsWith(TranslationFolderAliasSuffix))
                {
                    folderContentType = contentType;
                }
            }

            return folderContentType;
        }

        /// <summary>
        /// For a given node in Umbraco return the corresponding translation folder, if it exists
        /// </summary>
        /// <param name="NodeID">The id of the node</param>
        /// <returns>The translation folder, if it exists; otherwise null</returns>
        public static Document TranslationFolderGet(int NodeID)
        {
            Document nodeDoc = new Document(NodeID);

            foreach (Document doc in Document.GetChildrenForTree(NodeID))
            {

                if (doc.ContentType.Alias == nodeDoc.ContentType.Alias + TranslationFolderAliasSuffix)
                {
                    return doc;
                }
            }

            return null;
        }

        /// <summary>
        /// For a given node in umbraco, check if a translation node exists for a given language
        /// </summary>
        /// <param name="NodeID">The id of the node</param>
        /// <param name="languageISOCode">The ISO code of the language</param>
        /// <returns>True if the translation node exists; otherwise false</returns>
        public static bool TranslationNodeExists(int NodeID, string languageISOCode)
        {
            Document languagesFolder = TranslationFolderGet(NodeID);

            if (languagesFolder != null)
            {
                foreach (Document langDoc in languagesFolder.Children)
                {
                    if (langDoc.Text.ToLower() == languageISOCode.ToLower()) return true;
                }
            }

            return false;
        }

        /// <summary>
        /// For a given node in Umbraco, check if the translation folder exists
        /// </summary>
        /// <param name="NodeID">The id of the node</param>
        /// <returns>True if the translation folder exists; otherwise false</returns>
        public static bool TranslationFolderExists(int NodeID)
        {
            return (TranslationFolderGet(NodeID) != null);
        }

        /// <summary>
        /// For a given node in Umbraco, create the corresponding translation folder
        /// </summary>
        /// <param name="NodeID">The id of the node</param>
        /// <returns>True if the creation of the translation folder succeeds; otherwise false</returns>
        public static bool TranslationFolderCreate(int NodeID)
        {
            Document nodeDoc = new Document(NodeID);
            DocumentType contentType = GetTranslationFolderContentType(NodeID);
            if (contentType != null)
            {
                Document folder = Document.MakeNew(TranslationFolderName, contentType, User.GetCurrent(), nodeDoc.Id);

                List<ContentTypePropertyInfo> folderProperties = BLL.ContentType.GetPropertyList(contentType.Id);

                if ((from prop in folderProperties where prop.Alias == GetHideFromNavigationPropertyAlias() select prop).Count() > 0)
                    folder.getProperty(GetHideFromNavigationPropertyAlias()).Value = true;

                return true;
            }
            else return false;
        }

        /// <summary>
        /// Get the alias of the property of a translation node that should indicate whether or not
        /// that node should be hidden from navigation menus. By default the alias will be "umbracoNaviHide",
        /// unless another name has been given via the AppSettings key "PolyglotHideFromNavigationPropertyAlias"
        /// in web.config
        /// </summary>
        /// <returns></returns>
        private static string GetHideFromNavigationPropertyAlias()
        {
            string alias = "umbracoNaviHide";

            string aliasFromAppSettings = ConfigurationManager.AppSettings["PolyglotHideFromNavigationPropertyAlias"];

            if (!string.IsNullOrEmpty(aliasFromAppSettings)) alias = aliasFromAppSettings;

            return alias;
        }

        /// <summary>
        /// For a given node in Umbraco, create a corresponding translation node in a given language
        /// </summary>
        /// <param name="NodeID">The id of the node</param>
        /// <param name="languageISOCode">The ISO code of the language</param>
        /// <returns>True if the creation succeeds; otherwise false</returns>
        public static bool TranslationNodeCreate(int NodeID, string languageISOCode)
        {
            Document translationFolder = TranslationFolderGet(NodeID);
            if (translationFolder != null)
            {

                DocumentType translationFolderContentType = new DocumentType(translationFolder.ContentType.Id);
                int translationNodeContentTypeId = translationFolderContentType.AllowedChildContentTypeIDs[0];
                Document langNode = Document.MakeNew(languageISOCode, new DocumentType(translationNodeContentTypeId), User.GetCurrent(), translationFolder.Id);
                Document node = new Document(NodeID);
                try
                {
                    langNode.getProperty(LanguagePropertyAlias).Value = languageISOCode;
                }
                catch
                {
                    langNode.delete(true);
                    throw new Exception("NoLangProp");
                }
                List<ContentTypePropertyInfo> langNodeProperties = BLL.ContentType.GetPropertyList(translationNodeContentTypeId);
                foreach (ContentTypePropertyInfo property in langNodeProperties)
                {
                    if (property.Alias != LanguagePropertyAlias && property.Alias != GetHideFromNavigationPropertyAlias())
                        langNode.getProperty(property.Alias).Value = node.getProperty(property.Alias).Value;
                    if (property.Alias == GetHideFromNavigationPropertyAlias())
                        langNode.getProperty(property.Alias).Value = true;
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Sort the translation nodes of a given node in Umbraco by the sequence in which the corresponding
        /// languages appear in the Umbraco Settings section, under Languages.
        /// </summary>
        /// <param name="NodeID">The id of the node</param>
        public static void SortTranslationNodes(int NodeID)
        {
            Document translationFolder = TranslationFolderGet(NodeID);
            if (translationFolder != null)
            {

                foreach (Document translationNode in translationFolder.Children)
                {
                    foreach (Language lang in Languages.GetLanguages())
                    {
                        if (lang.ISOCode.ToLower() == translationNode.getProperty(LanguagePropertyAlias).Value.ToString().ToLower())
                            translationNode.sortOrder = lang.Sequence;
                    }
                }
            }
        }

    }
}