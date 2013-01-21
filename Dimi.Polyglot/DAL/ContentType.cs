using System;
using System.Collections.Generic;
using System.Configuration;
using Dimi.Polyglot.Model;
using umbraco.DataLayer;

namespace Dimi.Polyglot.DAL
{
    /// <summary>
    /// Content type data access layer methods (static)
    /// </summary>
    public static class ContentType
    {
        /// <summary>
        /// Gets the list of property types for a given content (document) type, recursively if there are parent types
        /// </summary>
        /// <param name="contentTypeId">The id of the content (document) type</param>
        /// <param name="rowList">If recursion is triggered, the properties gathered from the previous execution </param>
        /// <returns>The list of property types</returns>
        public static List<ContentTypePropertyInfo> GetPropertyList(int contentTypeId, List<ContentTypePropertyInfo> rowList = null)
        {
            var onUmbraco6 = false;

            if (rowList == null)
            {
                rowList = new List<ContentTypePropertyInfo>();
            }

            ISqlHelper h;
            try
            {
                h = DataLayerHelper.CreateSqlHelper(ConfigurationManager.AppSettings["umbracoDbDSN"]);
            }
            catch (Exception)
            {
                // Using umbraco 6 maybe?
                h = DataLayerHelper.CreateSqlHelper(ConfigurationManager.ConnectionStrings["umbracoDbDSN"].ConnectionString);
                onUmbraco6 = true;
            }


            using (
                var reader =
                    h.ExecuteReader("SELECT * from [cmsPropertyType] WHERE contentTypeId = @contentTypeId",
                                    h.CreateParameter("contentTypeId", contentTypeId)))
            {
                while (reader.Read())
                {
                    var s = new ContentTypePropertyInfo
                                                    {
                                                        Id = reader.GetInt("Id"),
                                                        DataTypeId = reader.GetInt("DataTypeId"),
                                                        ContentTypeId = reader.GetInt("ContentTypeId"),
                                                        // TabId = reader.GetInt("TabId"),
                                                        Alias = reader.GetString("Alias"),
                                                        Name = reader.GetString("Name"),
                                                        HelpText = reader.GetString("HelpText"),
                                                        SortOrder = reader.GetInt("SortOrder"),
                                                        Mandatory = reader.GetBoolean("Mandatory"),
                                                        ValidationRegExp = reader.GetString("ValidationRegExp"),
                                                        Description = reader.GetString("Description")
                                                    };
                    rowList.Add(s);
                }
            }

            var masterContentTypeId = -1;

            if (!onUmbraco6)
            {
                using (
                    var reader =
                        h.ExecuteReader("SELECT masterContentType from [cmsContentType] WHERE nodeId = @contentTypeId",
                                        h.CreateParameter("contentTypeId", contentTypeId)))
                {
                    if (reader.Read())
                    {
                        masterContentTypeId = reader.GetInt("masterContentType");

                    }
                }
            }
            else
            {
                using (
                    var reader =
                        h.ExecuteReader("SELECT parentContentTypeId from [cmsContentType2ContentType] WHERE childContentTypeId = @childContentTypeId",
                                        h.CreateParameter("childContentTypeId", contentTypeId)))
                {
                    if (reader.Read())
                    {
                        masterContentTypeId = reader.GetInt("parentContentTypeId");

                    }
                }
            }

            return masterContentTypeId == -1 ? rowList : GetPropertyList(masterContentTypeId, rowList);

        }
    }
}