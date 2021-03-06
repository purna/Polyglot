﻿using System;
using System.Configuration;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;

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
        /// <param name="nodeID">The id of the node</param>
        /// <returns>The document type of the translation folder, if it exists; otherwise null</returns>
        public static Umbraco.Core.Models.ContentType GetTranslationFolderContentType(int nodeID)
        {
            try
            {
                Umbraco.Core.Models.ContentType folderContentType = null;
                var cs = ApplicationContext.Current.Services.ContentService;
                var nodeDoc = cs.GetById(nodeID);

                var cts = ApplicationContext.Current.Services.ContentTypeService;

                foreach (
                    var contentType in
                        nodeDoc.ContentType.AllowedContentTypes.Where(
                                childContentType => childContentType.Alias == nodeDoc.ContentType.Alias + TranslationFolderAliasSuffix)
                    )
                {
                    folderContentType = (Umbraco.Core.Models.ContentType)ApplicationContext.Current.Services.ContentTypeService.GetContentType(contentType.Alias);
                }
                return folderContentType;
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(DocumentTranslation), ex.Message, ex);
                throw ex;
            }
        }

        /// <summary>
        /// For a given node in Umbraco return the corresponding translation folder, if it exists
        /// </summary>
        /// <param name="nodeID">The id of the node</param>
        /// <returns>The translation folder, if it exists; otherwise null</returns>
        public static IContent TranslationFolderGet(int nodeID)
        {
            try
            {
                var nodeDoc = ApplicationContext.Current.Services.ContentService.GetById(nodeID);
                return ApplicationContext.Current.Services.ContentService.GetChildren(nodeID).FirstOrDefault(doc => doc.ContentType.Alias == nodeDoc.ContentType.Alias + TranslationFolderAliasSuffix);
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(DocumentTranslation), ex.Message, ex);
                throw ex;
            }
        }

        /// <summary>
        /// For a given node in umbraco, check if a translation node exists for a given language
        /// </summary>
        /// <param name="nodeID">The id of the node</param>
        /// <param name="languageISOCode">The ISO code of the language</param>
        /// <returns>True if the translation node exists; otherwise false</returns>
        public static bool TranslationNodeExists(int nodeID, string languageISOCode)
        {
            try
            {
                var languagesFolder = TranslationFolderGet(nodeID);
                return languagesFolder != null && languagesFolder.Children().Any(langDoc => langDoc.Name.ToLower() == languageISOCode.ToLower());
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(DocumentTranslation), ex.Message, ex);
                throw ex;
            }
        }

        /// <summary>
        /// For a given node in Umbraco, check if the translation folder exists
        /// </summary>
        /// <param name="nodeID">The id of the node</param>
        /// <returns>True if the translation folder exists; otherwise false</returns>
        public static bool TranslationFolderExists(int nodeID)
        {
            try
            {
                return (TranslationFolderGet(nodeID) != null);
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(DocumentTranslation), ex.Message, ex);
                throw ex;
            }
        }

        /// <summary>
        /// For a given node in Umbraco, create the corresponding translation folder
        /// </summary>
        /// <param name="nodeID">The id of the node</param>
        /// <returns>True if the creation of the translation folder succeeds; otherwise false</returns>
        public static bool TranslationFolderCreate(int nodeID)
        {
            try
            {
                var nodeDoc = ApplicationContext.Current.Services.ContentService.GetById(nodeID);
                var contentType = GetTranslationFolderContentType(nodeID);
                if (contentType == null)
                    return false;
                else
                {

                    var folder = ApplicationContext.Current.Services.ContentService.CreateContent(TranslationFolderName, nodeDoc.Id, contentType.Alias);

                    var folderProperties = ContentType.GetPropertyList(contentType.Id);

                    if (
                        (from prop in folderProperties where prop.Alias == GetHideFromNavigationPropertyAlias() select prop).Any())
                    {
                        folder.Properties.Single(x => x.Alias == GetHideFromNavigationPropertyAlias()).Value = true;
                    }
                    
                    ApplicationContext.Current.Services.ContentService.Save(folder);

                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(DocumentTranslation), ex.Message, ex);
                throw ex;
            }

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
            try
            {
                var alias = "umbracoNaviHide";

                var aliasFromAppSettings = ConfigurationManager.AppSettings["PolyglotHideFromNavigationPropertyAlias"];

                if (!string.IsNullOrEmpty(aliasFromAppSettings)) alias = aliasFromAppSettings;

                return alias;
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(DocumentTranslation), ex.Message, ex);
                throw ex;
            }
        }

        /// <summary>
        /// For a given node in Umbraco, create a corresponding translation node in a given language
        /// </summary>
        /// <param name="nodeID">The id of the node</param>
        /// <param name="languageISOCode">The ISO code of the language</param>
        /// <returns>True if the creation succeeds; otherwise false</returns>
        public static bool TranslationNodeCreate(int nodeID, string languageISOCode)
        {
            try
            {
                var translationFolder = TranslationFolderGet(nodeID);
                if (translationFolder != null)
                {
                    var translationFolderContentType = ApplicationContext.Current.Services.ContentTypeService.GetContentType(translationFolder.ContentType.Id);
                    var translationNodeContentType = translationFolderContentType.AllowedContentTypes.First();
                    var langNode = ApplicationContext.Current.Services.ContentService.CreateContent(languageISOCode, translationFolder, translationNodeContentType.Alias);
                    var node = ApplicationContext.Current.Services.ContentService.GetById(nodeID);
                    try
                    {
                        langNode.Properties.Single(x => x.Alias == LanguagePropertyAlias).Value = languageISOCode;
                        
                    }
                    catch(Exception ex)
                    {
                        LogHelper.Error(typeof(DocumentTranslation), ex.Message, ex);
                        ApplicationContext.Current.Services.ContentService.Delete(langNode);
                        var notFoundMessage = "Could not find alias " + LanguagePropertyAlias + " amongst aliases ";
                        foreach (var prop in langNode.Properties)
                        {
                            notFoundMessage += prop.Alias + "/";
                        }
                        LogHelper.Warn(typeof(DocumentTranslation), notFoundMessage);
                        throw new Exception("NoLangProp");
                    }
                    
                    ApplicationContext.Current.Services.ContentService.Save(langNode);

                    var langNodeProperties =
                        ContentType.GetPropertyList(translationNodeContentType.Id.Value);
                    foreach (var property in langNodeProperties)
                    {
                        if (property.Alias != LanguagePropertyAlias &&
                            property.Alias != GetHideFromNavigationPropertyAlias())
                            langNode.Properties.Single(x => x.Alias == property.Alias).Value = node.Properties.Single(x => x.Alias == property.Alias).Value;
                        if (property.Alias == GetHideFromNavigationPropertyAlias())
                            langNode.Properties.Single(x => x.Alias == property.Alias).Value = true;
                        ApplicationContext.Current.Services.ContentService.Save(langNode);
                    }

                    return true;
                }
                LogHelper.Warn(typeof(DocumentTranslation), "Could not find translation folder for node id " + nodeID);
                return false;
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(DocumentTranslation), ex.Message, ex);
                throw ex;
            }
        }

        /// <summary>
        /// Sort the translation nodes of a given node in Umbraco by the sequence in which the corresponding
        /// languages appear in the Umbraco Settings section, under Languages.
        /// </summary>
        /// <param name="nodeID">The id of the node</param>
        public static void SortTranslationNodes(int nodeID)
        {
            try
            {
                var translationFolder = TranslationFolderGet(nodeID);
                if (translationFolder == null) return;
                foreach (var translationNode in translationFolder.Children())
                {
                    foreach (var lang in Languages.GetLanguages(true))
                    {
                        if (lang.ISOCode.ToLower() ==
                            translationNode.Properties.Single(x => x.Alias == LanguagePropertyAlias).Value.ToString().ToLower())
                            translationNode.SortOrder = lang.Sequence;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(DocumentTranslation), ex.Message, ex);
                throw ex;
            }
        }

        /// <summary>
        /// Checks if the appropriate document types exist and if they are properly structured so that 
        /// translations for a given node can be created.
        /// </summary>
        /// <param name="nodeID">The id of the node for which the translations are going to be created</param>
        /// <returns>"ok" if everything is in order; otherwise a description of the problem which has been discovered </returns>
        public static string CheckTranslationInfrastructure(int nodeID)
        {
            try
            {
                var status = "ok";
                var nodeDoc = ApplicationContext.Current.Services.ContentService.GetById(nodeID);
                var cts = ApplicationContext.Current.Services.ContentTypeService;

                try
                {
                    var translationFolderContentType = GetTranslationFolderContentType(nodeID);
                    if (translationFolderContentType != null)
                    {
                        if (!translationFolderContentType.AllowedContentTypes.Any())
                            throw new Exception(
                                "Translation document type does not exist, or it is not an allowed child nodetype of the translation folder document type.");

                        if (translationFolderContentType.AllowedContentTypes.Count() > 1)
                            throw new Exception(
                                "Translation folder document type has more than one allowed child nodetypes. It should only have one.");

                        var translationContentType = translationFolderContentType.AllowedContentTypes.First();

                        var translationContentTypeInstance = cts.GetContentType(translationContentType.Id.Value);

                        if (!(from prop in translationContentTypeInstance.PropertyTypes
                              where prop.Alias == LanguagePropertyAlias
                              select prop).Any())
                            throw new Exception("Translation document type does not contain the '" + LanguagePropertyAlias +
                                                "' (alias) property");

                        if ((from p in translationContentTypeInstance.PropertyTypes
                             where
                                 !(from pr in ContentType.GetPropertyList(nodeDoc.ContentType.Id) select pr.Alias).Contains(
                                     p.Alias)
                                 && p.Alias != GetHideFromNavigationPropertyAlias() && p.Alias != LanguagePropertyAlias
                             select p).Any())
                            throw new Exception(
                                "Translation document type contains properties that do not exist in the document type (apart from language and navigation hiding)");
                    }
                    else
                        throw new Exception("TranslationFolder document type " + ApplicationContext.Current.Services.ContentService.GetById(nodeID).ContentType.Alias +
                                            TranslationFolderAliasSuffix +
                                            " does not exist, or it does not have the right alias or is not an allowed child nodetype");
                }
                catch (Exception ex)
                {
                    LogHelper.Error(typeof(DocumentTranslation), ex.Message, ex);
                    status = ex.Message;
                }

                return status;
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(DocumentTranslation), ex.Message, ex);
                throw ex;
            }
        }
    }
}
