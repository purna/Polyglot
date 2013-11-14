using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;
using Umbraco.Core.Logging;

namespace Dimi.Polyglot.Installation
{
    /// <summary>
    /// The custom installation actions of the Polyglot package
    /// </summary>
    public class InstallAction : IPackageAction
    {
        private const string XPathQueryXmlTranslationActionArea = "/language/area[@alias='actions']";
        private const string XPathQueryXmlCreateTranslationsKey = "key[@alias='polyglotCreateTranslations']";

        private string GetTranslationFilePathPath()
        {
            return System.Web.HttpContext.Current.Server.MapPath("~/umbraco/config/lang/en.xml");
        }

        private bool AddCreateTranslationsTranslation()
        {
            var translationsFilePath = GetTranslationFilePathPath();
            var translationsFile = XDocument.Load(translationsFilePath, LoadOptions.PreserveWhitespace);
            var success = true;
            try
            {
                var actions = translationsFile.XPathSelectElements(XPathQueryXmlTranslationActionArea).Single();
                if (!actions.XPathSelectElements(XPathQueryXmlCreateTranslationsKey).Any())
                {
                    var translation = new XElement("key", new XAttribute("alias", "polyglotCreateTranslations"));
                    translation.SetValue("[Polyglot] Create translations");
                    actions.Add(translation);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(InstallAction), ex.Message, ex);
                success = false;
            }

            if (success)
            {
                try
                {
                    translationsFile.Save(translationsFilePath, SaveOptions.DisableFormatting);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(typeof(InstallAction), ex.Message, ex);
                    success = false;
                }
            }

            return success;
        }

        private bool RemoveCreateTranslationsTranslation()
        {
            var translationsFilePath = GetTranslationFilePathPath();
            var translationsFile = XDocument.Load(translationsFilePath, LoadOptions.PreserveWhitespace);
            var success = true;
            try
            {
                var actions = translationsFile.XPathSelectElements(XPathQueryXmlTranslationActionArea).Single();
                if (actions.XPathSelectElements(XPathQueryXmlCreateTranslationsKey).Any())
                {
                    actions.XPathSelectElements(XPathQueryXmlCreateTranslationsKey).Single().Remove();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(InstallAction), ex.Message, ex);
                success = false;
            }

            if (success)
            {
                try
                {
                    translationsFile.Save(translationsFilePath, SaveOptions.DisableFormatting);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(typeof(InstallAction), ex.Message, ex);
                    success = false;
                }
            }
            return success;
        }

        public bool Execute(string packageName, XmlNode xmlData)
        {
            return AddCreateTranslationsTranslation();
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {
            return RemoveCreateTranslationsTranslation();
        }

        public string Alias()
        {
            return "PolyglotCustomInstallAction";
        }

        public XmlNode SampleXml()
        {
            const string sample = "<Action runat=\"install\" undo=\"true\" alias=\"PolyglotCustomInstallAction\"></Action>";
            return helper.parseStringToXmlNode(sample);
        }
    }
}