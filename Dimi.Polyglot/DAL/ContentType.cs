using System.Collections.Generic;
using System.Configuration;
using Dimi.Polyglot.Model;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using System.Linq;
using System.Linq.Expressions;

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
            if (rowList == null)
            {
                rowList = new List<ContentTypePropertyInfo>();
            }

            var db = UmbracoDatabase.Get();

            rowList.InsertRange(0, db.Query<ContentTypePropertyInfo>(new Sql().Select("*").From("cmsPropertyType").Where("contentTypeId = @0", contentTypeId)).ToList());

            var masterContentTypeId = db.SingleOrDefault<int>(new Sql().Select("parentContentTypeId").From("cmsContentType2ContentType").Where("childContentTypeId = @0", contentTypeId));

            return masterContentTypeId == 0 ? rowList : GetPropertyList(masterContentTypeId, rowList);
        }
    }
}