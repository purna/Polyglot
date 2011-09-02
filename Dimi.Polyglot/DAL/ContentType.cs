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
        /// Gets the list of property types for a given content (document) type
        /// </summary>
        /// <param name="contentTypeId">The id of the content (document) type</param>
        /// <returns>The list of property types</returns>
        public static List<ContentTypePropertyInfo> GetPropertyList(int contentTypeId)
        {
            var rowList = new List<ContentTypePropertyInfo>();

            var h = DataLayerHelper.CreateSqlHelper(ConfigurationManager.AppSettings["umbracoDbDSN"]);

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
                                                        TabId = reader.GetInt("TabId"),
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
            return rowList;
        }
    }
}