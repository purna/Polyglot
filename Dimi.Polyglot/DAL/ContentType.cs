using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dimi.Polyglot.Model;
using System.Configuration;

namespace Dimi.Polyglot.DAL
{
    /// <summary>
    /// Content type data access layer methods (static)
    /// </summary>
    public static class ContentType
    {
        /// <summary>
        /// Gets the list of property types for a given content (document) type
        /// </summary>
        /// <param name="contentTypeId">The id of the content (document) type</param>
        /// <returns>The list of property types</returns>
        public static List<ContentTypePropertyInfo> GetPropertyList(int contentTypeId)
        {
            List<ContentTypePropertyInfo> rowList = new List<ContentTypePropertyInfo>();

            umbraco.DataLayer.ISqlHelper h = umbraco.DataLayer.DataLayerHelper.CreateSqlHelper(ConfigurationManager.AppSettings["umbracoDbDSN"]);

            ContentTypePropertyInfo s;

            using (umbraco.DataLayer.IRecordsReader reader = h.ExecuteReader("SELECT * from [cmsPropertyType] WHERE contentTypeId = @contentTypeId", h.CreateParameter("contentTypeId", contentTypeId)))
            {
                while (reader.Read())
                {
                    s = new ContentTypePropertyInfo();
                    s.Id = reader.GetInt("Id");
                    s.DataTypeId = reader.GetInt("DataTypeId");
                    s.ContentTypeId = reader.GetInt("ContentTypeId");
                    s.TabId = reader.GetInt("TabId");
                    s.Alias = reader.GetString("Alias");
                    s.Name = reader.GetString("Name");
                    s.HelpText = reader.GetString("HelpText");
                    s.SortOrder = reader.GetInt("SortOrder");
                    s.Mandatory = reader.GetBoolean("Mandatory");
                    s.ValidationRegExp = reader.GetString("ValidationRegExp");
                    s.Description = reader.GetString("Description");
                    rowList.Add(s);
                }
            }
            return rowList;
        }
    }
}