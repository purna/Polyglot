using System.Collections.Generic;
using Dimi.Polyglot.Model;

namespace Dimi.Polyglot.BLL
{
    /// <summary>
    /// Business logic of content type (static methods)
    /// </summary>
    public static class ContentType
    {
        /// <summary>
        /// Get the list of property types for a given content (document) type
        /// </summary>
        /// <param name="contentTypeId">The id of the content (document) type</param>
        /// <returns>The list of property types</returns>
        public static List<ContentTypePropertyInfo> GetPropertyList(int contentTypeId)
        {
            return DAL.ContentType.GetPropertyList(contentTypeId);
        }
    }
}