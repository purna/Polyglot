using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimi.Polyglot.DAL
{
    public static class UmbracoDatabase
    {
        public static global::Umbraco.Core.Persistence.UmbracoDatabase Get()
        {
            return global::Umbraco.Core.ApplicationContext.Current == null ? new global::Umbraco.Core.Persistence.UmbracoDatabase("umbracoDbDSN") :
                global::Umbraco.Core.ApplicationContext.Current.DatabaseContext.Database;
        }
    }
}