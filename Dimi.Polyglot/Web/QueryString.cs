using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Web.UI;

namespace Zeta.EnterpriseLibrary.Web
{
    #region Using directives.

    // ----------------------------------------------------------------------
    
    // ----------------------------------------------------------------------

    #endregion

    /////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Class for parsing URL parameters (parameters).
    /// </summary>
    /// <remarks>
    /// For details about this class, please see the CodeProject.com article at 
    /// http://zeta.li/codeproject-simplequerystring
    /// 
    /// For details about the author, contact mailto:uwe.keim@gmail.com 
    /// or see him at http://twitter.com/UweKeim
    /// </remarks>
    [DebuggerDisplay(@"{AllUrlTilde}")]
    public class QueryString :
        ICloneable
    {
        #region Construction.

        // ------------------------------------------------------------------

        /// <summary>
        /// Constructor.
        /// </summary>
        public QueryString()
        {
            if (HttpContext.Current != null &&
                HttpContext.Current.Handler != null &&
                HttpContext.Current.Handler is Page)
            {
                _currentPage =
                    HttpContext.Current.Handler as Page;
                FromUrl(_currentPage);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        public QueryString(
            Page currentPage)
        {
            FromUrl(currentPage);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="url">The URL.</param>
        public QueryString(
            string url)
        {
            FromUrl(url);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="uri">The URI.</param>
        public QueryString(
            Uri uri)
        {
            FromUrl(uri.AbsoluteUri);
        }

        // ------------------------------------------------------------------

        #endregion

        #region Public properties.

        // ------------------------------------------------------------------

        /// <summary>
        /// Access the parameters collection.
        /// </summary>
        /// <value>The parameters.</value>
        public QueryStringItemCollection Parameters
        {
            get { return _qs; }
        }

        /// <summary>
        /// Get an array of all parameter names.
        /// </summary>
        /// <value>The parameter names.</value>
        public string[] ParameterNames
        {
            get
            {
                if (_qs.HasKeys())
                {
                    return _qs.AllKeys;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get an array of all parameter values.
        /// </summary>
        /// <value>The parameter values.</value>
        public string[] ParameterValues
        {
            get
            {
                if (_qs.HasKeys())
                {
                    var result = new List<string>();

                    foreach (string key in _qs.Keys)
                    {
                        string value = _qs[key];

                        if (!string.IsNullOrEmpty(value))
                        {
                            result.Add(value);
                        }
                    }

                    if (result.Count <= 0)
                    {
                        return null;
                    }
                    else
                    {
                        return result.ToArray();
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get an array of all parameter name/value pairs.
        /// </summary>
        /// <value>The parameter pairs.</value>
        public Pair<string, string>[] ParameterPairs
        {
            get
            {
                if (_qs.HasKeys())
                {
                    var result = new List<Pair<string, string>>();

                    foreach (string key in _qs.Keys)
                    {
                        string value = _qs[key];

                        result.Add(
                            string.IsNullOrEmpty(value)
                                ? new Pair<string, string>(key)
                                : new Pair<string, string>(key, value));
                    }

                    if (result.Count <= 0)
                    {
                        return null;
                    }
                    else
                    {
                        return result.ToArray();
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Access an parameter value by the parameter name.
        /// </summary>
        /// <value></value>
        public string this[string index]
        {
            get { return _qs[index]; }
            set { _qs[index] = value; }
        }

        /// <summary>
        /// Get the complete string including the <see cref="BeforeUrl"/> and
        /// all current parameters.
        /// </summary>
        /// <value>All URL.</value>
        public string AllUrl
        {
            get { return BeforeUrl + Make(); }
            set { FromUrl(value); }
        }

        /// <summary>
        /// Gets/sets the short version of an URL where the base path is
        /// replaced with a tile ("~").
        /// </summary>
        /// <value>All URL tilde.</value>
        public string AllUrlTilde
        {
            get { return makeTildeUrl(AllUrl); }
            set { AllUrl = replaceTildeComplete(value); }
        }

        /// <summary>
        /// Get the complete string including the <see cref="BeforeUrl"/> and
        /// all current parameters. Returns as an Uri class.
        /// </summary>
        /// <value>All URI.</value>
        public Uri AllUri
        {
            get { return new Uri(BeforeUrl + Make()); }
            set { FromUrl(value); }
        }

        /// <summary>
        /// The URL that comes before the actual name-value pair parameters.
        /// </summary>
        /// <value>The before URL.</value>
        public string BeforeUrl
        {
            get { return replaceTilde(_beforeUrl); }
            set { _beforeUrl = value; }
        }

        /// <summary>
        /// The URL that comes before the actual name-value pair parameters.
        /// </summary>
        /// <value>The before URL tilde.</value>
        public string BeforeUrlTilde
        {
            get { return makeTildeUrl(BeforeUrl); }
            set { BeforeUrl = replaceTildeComplete(value); }
        }

        /// <summary>
        /// The URL that comes before the actual name-value pair parameters.
        /// </summary>
        /// <value>The before URI.</value>
        public Uri BeforeUri
        {
            get { return new Uri(_beforeUrl); }
            set
            {
                var temp = new QueryString(value);
                _beforeUrl = temp.BeforeUrl;
            }
        }

        /// <summary>
        /// The single string from the current name-value pairs inside
        /// this class. Equivalent to the Make() function.
        /// </summary>
        /// <value>The raw parameters.</value>
        public string RawParameters
        {
            get { return Make(); }
            set { FromUrl(Combine(BeforeUrl, value)); }
        }

        // ------------------------------------------------------------------

        #endregion

        #region Public operations.

        // ------------------------------------------------------------------

        /// <summary>
        /// Combines an existing URL and a query string. Takes care
        /// of worrying about whether to add "&amp;..." or "?...".
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="queryString">The query string.</param>
        /// <returns>Returns the complete URL.</returns>
        public static string Combine(
            string url,
            string queryString)
        {
            string result = url.TrimEnd('?', '&');

            if (result.IndexOf(@"?") >= 0)
            {
                return url + @"&" + queryString;
            }
            else
            {
                return url + @"?" + queryString;
            }
        }

        /// <summary>
        /// Check whether a parameter with a given name exists.
        /// </summary>
        /// <param name="parameterName">The name of the parameter
        /// to check for.</param>
        /// <returns>
        /// Returns TRUE if the parameter is present and
        /// has a non-empty value, returns FALSE otherwise.
        /// </returns>
        public bool HasParameter(
            string parameterName)
        {
            if (parameterName == null ||
                parameterName.Trim().Length <= 0)
            {
                return false;
            }
            else
            {
                parameterName = parameterName.Trim();
                string v = this[parameterName];

                return v != null && v.Trim().Length > 0;
            }
        }

        /// <summary>
        /// Set or replace a single parameter.
        /// </summary>
        /// <param name="name">The name of the parameter to set.</param>
        /// <param name="val">The value of the parameter to set.</param>
        public QueryString SetParameter(
            string name,
            string val)
        {
            if (val == null || val.Trim().Length <= 0)
            {
                RemoveParameter(name);
            }
            else
            {
                _qs[name] = val;
            }

            return this;
        }

        /// <summary>
        /// Removes an parameter (if exists) with the given name.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        public QueryString RemoveParameter(
            string parameterName)
        {
            _qs.Remove(parameterName);
            return this;
        }

        /// <summary>
        /// Removes the parameters with the given names (if exists).
        /// </summary>
        /// <param name="parameterNames">The collection of parameters
        /// to remove.</param>
        public QueryString RemoveParameters(
            IEnumerable<string> parameterNames)
        {
            if (parameterNames != null)
            {
                foreach (string parameterName in parameterNames)
                {
                    RemoveParameter(parameterName);
                }
            }

            return this;
        }

        /// <summary>
        /// Removes the parameters with the given names (if exists).
        /// </summary>
        /// <param name="parameterNames">The collection of parameters
        /// to remove.</param>
        public QueryString RemoveParameters(
            IEnumerable parameterNames)
        {
            if (parameterNames != null)
            {
                foreach (string parameterName in parameterNames)
                {
                    RemoveParameter(parameterName);
                }
            }

            return this;
        }

        /// <summary>
        /// Removes the parameters with the given names (if exists).
        /// </summary>
        /// <param name="parameterNames">The collection of parameters
        /// to remove.</param>
        public QueryString RemoveParameters(
            params string[] parameterNames)
        {
            if (parameterNames != null)
            {
                foreach (string parameterName in parameterNames)
                {
                    RemoveParameter(parameterName);
                }
            }

            return this;
        }

        public QueryString RemoveAllParametersExcept(
            params string[] parameterNames)
        {
            return RemoveAllParametersExcept(parameterNames as IEnumerable);
        }

        public QueryString RemoveAllParametersExcept(
            IEnumerable<string> parameterNames)
        {
            return RemoveAllParametersExcept(parameterNames as IEnumerable);
        }

        public QueryString RemoveAllParametersExcept(
            IEnumerable parameterNames)
        {
            if (parameterNames != null)
            {
                var removes = new List<string>();

                foreach (string s in _qs.AllKeys)
                {
                    if (!contains(parameterNames, s))
                    {
                        removes.Add(s);
                    }
                }

                RemoveParameters(removes);
            }

            return this;
        }

        private static bool contains(
            IEnumerable checks,
            string s)
        {
            if (checks != null && s != null)
            {
                foreach (string check in checks)
                {
                    if (check != null && check == s)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Removes all parameters.
        /// </summary>
        public QueryString RemoveAllParameters()
        {
            _qs.Clear();
            return this;
        }

        /// <summary>
        /// Get an parameter value by a given name.
        /// </summary>
        /// <param name="name">The name of the parameter value
        /// to retrieve.</param>
        /// <returns>
        /// Returns an empty string (NOT <see langword="null"/>) if the parameter
        /// is not found.
        /// </returns>
        public string GetParameter(
            string name)
        {
            string result = _qs[name];

            if (string.IsNullOrEmpty(result))
            {
                if (_currentPage != null &&
                    _currentPage.Request != null &&
                    _currentPage.Request.Form != null)
                {
                    result = _currentPage.Request.Form[name];
                }

                // try session, also.
                if (string.IsNullOrEmpty(result))
                {
                    if (_currentPage != null &&
                        _currentPage.Session != null)
                    {
                        object o = _currentPage.Session[name];
                        if (o != null)
                        {
                            result = o.ToString();
                        }
                    }
                }

                // Try cookies, also.
                if (string.IsNullOrEmpty(result))
                {
                    if (_currentPage != null &&
                        _currentPage.Request != null &&
                        _currentPage.Request.Cookies != null)
                    {
                        HttpCookie c = _currentPage.Request.Cookies[name];
                        if (c != null)
                        {
                            result = c.Value;
                        }
                    }
                }
            }

            return result ?? string.Empty;
        }

        /// <summary>
        /// Pack the current parameters into a new dictionary object.
        /// </summary>
        /// <returns>Returns the new dictionary object</returns>
        public IDictionary ToDictionary()
        {
            if (Parameters == null || Parameters.Count <= 0)
            {
                return new Hashtable();
            }
            else
            {
                var result = new Hashtable();

                foreach (string key in Parameters)
                {
                    result[key] = Parameters[key];
                }

                return result;
            }
        }

        /// <summary>
        /// Deletes the current content of the Parameters collection
        /// and fills with the content of the passed dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary to copy from.</param>
        public QueryString FromDictionary(
            IDictionary dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(@"dictionary");
            }
            else
            {
                RemoveAllParameters();

                foreach (string key in dictionary.Keys)
                {
                    SetParameter(key, dictionary[key].ToString());
                }
            }

            return this;
        }

        /// <summary>
        /// Redirects to the currently contained URL.
        /// </summary>
        public void Redirect()
        {
            RedirectTemporary();
        }

        /// <summary>
        /// HTTP-302-redirects to the currently contained URL.
        /// </summary>
        public void RedirectTemporary()
        {
            Redirect(RedirectMethod.Temporary);
        }

        /// <summary>
        /// HTTP-301-redirects to the currently contained URL.
        /// </summary>
        public void RedirectPermanent()
        {
            Redirect(RedirectMethod.Permanent);
        }

        /// <summary>
        /// Redirects to the currently contained URL.
        /// </summary>
        /// <param name="method">The method.</param>
        public void Redirect(
            RedirectMethod method)
        {
            HttpResponse response = HttpContext.Current.Response;

            if (method == RedirectMethod.Temporary)
            {
                response.Redirect(AllUrl, true);
            }
            else if (method == RedirectMethod.Permanent)
            {
                response.Clear();

                response.StatusCode = 301;
                response.StatusDescription = @"Moved Permanently";

                response.RedirectLocation = AllUrl;

                response.Write(
                    @"<html><head><title>Object moved</title></head><body>" +
                    Environment.NewLine);
                response.Write(
                    string.Format(
                        @"<h2>Object moved to <a href=""{0}"">here</a>.</h2>" +
                        Environment.NewLine,
                        HttpUtility.HtmlAttributeEncode(AllUrl)));
                response.Write(
                    @"</body></html>" +
                    Environment.NewLine);

                response.End();
            }
            else
            {
                throw new ArgumentException(
                    string.Format(
                        @"Unknown redirect method '{0}'.",
                        method),
                    @"method");
            }
        }

        // ------------------------------------------------------------------

        #endregion

        #region Reading from an URL.

        // ------------------------------------------------------------------

        /// <summary>
        /// Parse a query string and insert the found parameters
        /// into the collection of this class.
        /// </summary>
        /// <param name="page">The page.</param>
        public QueryString FromUrl(
            Page page)
        {
            if (page != null)
            {
                _currentPage = page;
                FromUrl(_currentPage.Request.RawUrl);
            }

            return this;
        }

        /// <summary>
        /// Parse a query string and insert the found parameters
        /// into the collection of this class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        public QueryString FromUrl(
            Uri uri)
        {
            if (uri != null)
            {
                FromUrl(uri.AbsoluteUri);
            }

            return this;
        }

        /// <summary>
        /// Parse a query string and insert the found parameters
        /// into the collection of this class.
        /// </summary>
        /// <param name="url">The URL.</param>
        public QueryString FromUrl(
            string url)
        {
            if (url != null)
            {
                url = replaceTilde(url);

                _qs.Clear();

                // store the part before, too.
                int qPos = url.IndexOf(@"?");
                if (qPos >= 0)
                {
                    BeforeUrl = url.Substring(0, qPos - 0);
                    url = url.Substring(qPos + 1);
                }
                else
                {
                    BeforeUrl = url;
                }

                if (url.Length > 0 && url.Substring(0, 1) == @"?")
                {
                    url = url.Substring(1);
                }

                // Break the values.
                string[] pairs = url.Split('&');
                foreach (string pair in pairs)
                {
                    string a = string.Empty;
                    string b = string.Empty;

                    string[] singular = pair.Split('=');

                    int j = 0;
                    foreach (string one in singular)
                    {
                        if (j == 0)
                        {
                            a = one;
                        }
                        else
                        {
                            b = one;
                        }

                        j++;
                    }

                    // Store.
                    SetParameter(a, HttpUtility.UrlDecode(b));
                }
            }

            return this;
        }

        // ------------------------------------------------------------------

        #endregion

        #region Making a string from the parameters.

        // ------------------------------------------------------------------

        /// <summary>
        /// Build a single string from the current name-value pairs inside
        /// this class. Equivalent to the Parameters property.
        /// </summary>
        /// <returns>
        /// Returns the complete query string, but WITHOUT the <see cref="BeforeUrl"/>.
        /// </returns>
        public string Make()
        {
            string result = @"?";

            foreach (string name in _qs)
            {
                string val = _qs[name];

                if (!string.IsNullOrEmpty(val))
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        result +=
                            string.Format(
                                @"{0}={1}&",
                                name,
                                HttpUtility.UrlEncode(val));
                    }
                    else
                    {
                        // Allow for "no-names".
                        result +=
                            string.Format(@"{0}&",
                                          HttpUtility.UrlEncode(val));
                    }
                }
            }

            //return result;
            return result.TrimEnd('?', '&');
        }

        /// <summary>
        /// Build a single string from the current name-value pairs inside
        /// this class. Replace/add the name-value pairs passed as
        /// parameters to this function.
        /// </summary>
        /// <param name="name1">The name1.</param>
        /// <param name="value1">The value1.</param>
        /// <returns>
        /// Returns the complete query string, but WITHOUT the <see cref="BeforeUrl"/>.
        /// </returns>
        public string Make(
            string name1,
            string value1)
        {
            return Make(
                name1, value1,
                string.Empty, string.Empty,
                string.Empty, string.Empty,
                string.Empty, string.Empty,
                string.Empty, string.Empty);
        }

        /// <summary>
        /// Build a single string from the current name-value pairs inside
        /// this class. Replace/add the name-value pair(s) passed as
        /// parameters to this function.
        /// </summary>
        /// <param name="name1">The name1.</param>
        /// <param name="value1">The value1.</param>
        /// <param name="name2">The name2.</param>
        /// <param name="value2">The value2.</param>
        /// <returns>
        /// Returns the complete query string, but WITHOUT the <see cref="BeforeUrl"/>.
        /// </returns>
        public string Make(
            string name1,
            string value1,
            string name2,
            string value2)
        {
            return Make(
                name1, value1,
                name2, value2,
                string.Empty, string.Empty,
                string.Empty, string.Empty,
                string.Empty, string.Empty);
        }

        /// <summary>
        /// Build a single string from the current name-value pairs inside
        /// this class. Replace/add the name-value pair(s) passed as
        /// parameters to this function.
        /// </summary>
        /// <param name="name1">The name1.</param>
        /// <param name="value1">The value1.</param>
        /// <param name="name2">The name2.</param>
        /// <param name="value2">The value2.</param>
        /// <param name="name3">The name3.</param>
        /// <param name="value3">The value3.</param>
        /// <returns>
        /// Returns the complete query string, but WITHOUT the <see cref="BeforeUrl"/>.
        /// </returns>
        public string Make(
            string name1,
            string value1,
            string name2,
            string value2,
            string name3,
            string value3)
        {
            return Make(
                name1, value1,
                name2, value2,
                name3, value3,
                string.Empty, string.Empty,
                string.Empty, string.Empty);
        }

        /// <summary>
        /// Build a single string from the current name-value pairs inside
        /// this class. Replace/add the name-value pair(s) passed as
        /// parameters to this function.
        /// </summary>
        /// <param name="name1">The name1.</param>
        /// <param name="value1">The value1.</param>
        /// <param name="name2">The name2.</param>
        /// <param name="value2">The value2.</param>
        /// <param name="name3">The name3.</param>
        /// <param name="value3">The value3.</param>
        /// <param name="name4">The name4.</param>
        /// <param name="value4">The value4.</param>
        /// <returns>
        /// Returns the complete query string, but WITHOUT the <see cref="BeforeUrl"/>.
        /// </returns>
        public string Make(
            string name1,
            string value1,
            string name2,
            string value2,
            string name3,
            string value3,
            string name4,
            string value4)
        {
            return Make(
                name1, value1,
                name2, value2,
                name3, value3,
                name4, value4,
                string.Empty, string.Empty);
        }

        /// <summary>
        /// Build a single string from the current name-value pairs inside
        /// this class. Replace/add the name-value pair(s) passed as
        /// parameters to this function.
        /// </summary>
        /// <param name="name1">The name1.</param>
        /// <param name="value1">The value1.</param>
        /// <param name="name2">The name2.</param>
        /// <param name="value2">The value2.</param>
        /// <param name="name3">The name3.</param>
        /// <param name="value3">The value3.</param>
        /// <param name="name4">The name4.</param>
        /// <param name="value4">The value4.</param>
        /// <param name="name5">The name5.</param>
        /// <param name="value5">The value5.</param>
        /// <returns>
        /// Returns the complete query string, but WITHOUT the <see cref="BeforeUrl"/>.
        /// </returns>
        public string Make(
            string name1,
            string value1,
            string name2,
            string value2,
            string name3,
            string value3,
            string name4,
            string value4,
            string name5,
            string value5)
        {
            var clone = Clone() as QueryString;

            if (clone == null)
            {
                return null;
            }
            else
            {
                clone.SetParameter(name1, value1);
                clone.SetParameter(name2, value2);
                clone.SetParameter(name3, value3);
                clone.SetParameter(name4, value4);
                clone.SetParameter(name5, value5);

                return clone.Make();
            }
        }

        // ------------------------------------------------------------------

        #endregion

        #region Making a string from the parameters, including BeforeUrl.

        // ------------------------------------------------------------------

        /// <summary>
        /// Build a single string from the current name-value pairs inside
        /// this class.
        /// </summary>
        /// <returns>
        /// Returns the complete query string, WITH the <see cref="BeforeUrl"/>.
        /// </returns>
        public string MakeAll()
        {
            return BeforeUrl + Make();
        }

        /// <summary>
        /// Build a single string from the current name-value pairs inside
        /// this class. Replace/add the name-value pairs passed as
        /// parameters to this function.
        /// </summary>
        /// <param name="name1">The name1.</param>
        /// <param name="value1">The value1.</param>
        /// <returns>
        /// Returns the complete query string, WITH the <see cref="BeforeUrl"/>.
        /// </returns>
        public string MakeAll(
            string name1,
            string value1)
        {
            return MakeAll(
                name1, value1,
                string.Empty, string.Empty,
                string.Empty, string.Empty,
                string.Empty, string.Empty,
                string.Empty, string.Empty);
        }

        /// <summary>
        /// Build a single string from the current name-value pairs inside
        /// this class. Replace/add the name-value pair(s) passed as
        /// parameters to this function.
        /// </summary>
        /// <param name="name1">The name1.</param>
        /// <param name="value1">The value1.</param>
        /// <param name="name2">The name2.</param>
        /// <param name="value2">The value2.</param>
        /// <returns>
        /// Returns the complete query string, WITH the <see cref="BeforeUrl"/>.
        /// </returns>
        public string MakeAll(
            string name1,
            string value1,
            string name2,
            string value2)
        {
            return MakeAll(
                name1, value1,
                name2, value2,
                string.Empty, string.Empty,
                string.Empty, string.Empty,
                string.Empty, string.Empty);
        }

        /// <summary>
        /// Build a single string from the current name-value pairs inside
        /// this class. Replace/add the name-value pair(s) passed as
        /// parameters to this function.
        /// </summary>
        /// <param name="name1">The name1.</param>
        /// <param name="value1">The value1.</param>
        /// <param name="name2">The name2.</param>
        /// <param name="value2">The value2.</param>
        /// <param name="name3">The name3.</param>
        /// <param name="value3">The value3.</param>
        /// <returns>
        /// Returns the complete query string, WITH the <see cref="BeforeUrl"/>.
        /// </returns>
        public string MakeAll(
            string name1,
            string value1,
            string name2,
            string value2,
            string name3,
            string value3)
        {
            return MakeAll(
                name1, value1,
                name2, value2,
                name3, value3,
                string.Empty, string.Empty,
                string.Empty, string.Empty);
        }

        /// <summary>
        /// Build a single string from the current name-value pairs inside
        /// this class. Replace/add the name-value pair(s) passed as
        /// parameters to this function.
        /// </summary>
        /// <param name="name1">The name1.</param>
        /// <param name="value1">The value1.</param>
        /// <param name="name2">The name2.</param>
        /// <param name="value2">The value2.</param>
        /// <param name="name3">The name3.</param>
        /// <param name="value3">The value3.</param>
        /// <param name="name4">The name4.</param>
        /// <param name="value4">The value4.</param>
        /// <returns>
        /// Returns the complete query string, WITH the <see cref="BeforeUrl"/>.
        /// </returns>
        public string MakeAll(
            string name1,
            string value1,
            string name2,
            string value2,
            string name3,
            string value3,
            string name4,
            string value4)
        {
            return MakeAll(
                name1, value1,
                name2, value2,
                name3, value3,
                name4, value4,
                string.Empty, string.Empty);
        }

        /// <summary>
        /// Build a single string from the current name-value pairs inside
        /// this class. Replace/add the name-value pair(s) passed as
        /// parameters to this function.
        /// </summary>
        /// <param name="name1">The name1.</param>
        /// <param name="value1">The value1.</param>
        /// <param name="name2">The name2.</param>
        /// <param name="value2">The value2.</param>
        /// <param name="name3">The name3.</param>
        /// <param name="value3">The value3.</param>
        /// <param name="name4">The name4.</param>
        /// <param name="value4">The value4.</param>
        /// <param name="name5">The name5.</param>
        /// <param name="value5">The value5.</param>
        /// <returns>
        /// Returns the complete query string, WITH the <see cref="BeforeUrl"/>.
        /// </returns>
        public string MakeAll(
            string name1,
            string value1,
            string name2,
            string value2,
            string name3,
            string value3,
            string name4,
            string value4,
            string name5,
            string value5)
        {
            return BeforeUrl + Make(
                name1, value1,
                name2, value2,
                name3, value3,
                name4, value4,
                name5, value5);
        }

        // ------------------------------------------------------------------

        #endregion

        #region ICloneable member.

        // ------------------------------------------------------------------

        /// <summary>
        /// Makes a deep copy.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            var dst =
                new QueryString
                    {
                        _currentPage = _currentPage,
                        _beforeUrl = _beforeUrl
                    };

            // Clone.
            foreach (string key in _qs.Keys)
            {
                dst._qs[key] = _qs[key];
            }

            return dst;
        }

        // ------------------------------------------------------------------

        #endregion

        #region Collection class for the parameters.

        // ------------------------------------------------------------------

        /// <summary>
        /// Collection class for the parameters.
        /// </summary>
        public class QueryStringItemCollection :
            NameValueCollection
        {
            #region Public methods.

            /// <summary>
            /// Construct only in this class.
            /// </summary>
            internal QueryStringItemCollection()
            {
            }

            #endregion
        }

        // ------------------------------------------------------------------

        #endregion

        #region Pair class.

        // ------------------------------------------------------------------

        [Serializable]
        [DebuggerDisplay(@"Name = {_name}, Value = {_value}")]
        public class Pair<TK, TV>
            where TK : class
        {
            #region Public methods.

            /// <summary>
            /// Constructor.
            /// </summary>
            public Pair()
            {
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="name">The name.</param>
            public Pair(
                TK name)
            {
                Name = name;
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="val">The val.</param>
            public Pair(
                TK name,
                TV val)
            {
                Name = name;
                Value = val;
            }

            /// <summary>
            /// Returns a <see cref="T:System.String"></see> that represents the 
            /// current <see cref="T:System.Object"></see>.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.String"></see> that represents the current
            /// <see cref="T:System.Object"></see>.
            /// </returns>
            public override string ToString()
            {
                return Name == null ? null : Name.ToString();
            }

            #endregion

            #region Public properties.

            /// <summary>
            /// Alias.
            /// </summary>
            /// <value>The one.</value>
            public TK One
            {
                get { return Name; }
                set { Name = value; }
            }

            /// <summary>
            /// Alias.
            /// </summary>
            /// <value>The two.</value>
            public TV Two
            {
                get { return Value; }
                set { Value = value; }
            }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            public TK Name
            {
                get { return _name; }
                set { _name = value; }
            }

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            /// <value>The value.</value>
            public TV Value
            {
                get { return _value; }
                set { _value = value; }
            }

            /// <summary>
            /// Gets or sets the first.
            /// </summary>
            /// <value>The first.</value>
            public TK First
            {
                get { return _name; }
                set { _name = value; }
            }

            /// <summary>
            /// Gets or sets the second.
            /// </summary>
            /// <value>The second.</value>
            public TV Second
            {
                get { return _value; }
                set { _value = value; }
            }

            #endregion

            #region Private variables.

            private TK _name;
            private TV _value;

            #endregion
        }

        // ------------------------------------------------------------------

        #endregion

        #region Private variables.

        // ------------------------------------------------------------------

        /// <summary>
        /// The collection to store the name-value pairs.
        /// </summary>
        private readonly QueryStringItemCollection _qs =
            new QueryStringItemCollection();

        /// <summary>
        /// The URL that comes before the actual name-value pair parameters.
        /// </summary>
        private string _beforeUrl = string.Empty;

        /// <summary>
        /// The page that is currently loaded.
        /// </summary>
        private Page _currentPage;

        // ------------------------------------------------------------------

        #endregion

        #region Private helper.

        // ------------------------------------------------------------------

        private static string makeTildeUrl(
            string url)
        {
            string expt = replaceTilde(@"~");
            string mc = url;

            if (expt == null || expt.Length <= 0)
            {
                return mc;
            }
            else
            {
                return mc.Replace(expt, @"~");
            }
        }

        private static string replaceTilde(
            string path)
        {
            if (path == null ||
                path.Length <= 0 ||
                path.IndexOf('~') != 0)
            {
                return postProcessReplaceTilde(path);
            }
            else if (
                HttpContext.Current == null ||
                HttpContext.Current.Request == null)
            {
                // Try to lookup from config.
                string tilde = ConfigurationManager.AppSettings[@"replaceTildeFallBack"];

                if (!string.IsNullOrEmpty(tilde))
                {
                    tilde = tilde.TrimEnd('\\', '/');
                    return postProcessReplaceTilde(path.Replace(@"~", tilde));
                }
                else
                {
                    return postProcessReplaceTilde(path);
                }
            }
            else
            {
                string tilde =
                    HttpContext.Current.Request.ApplicationPath == @"/"
                        ? string.Empty
                        : HttpContext.Current.Request.ApplicationPath;

                return postProcessReplaceTilde(path.Replace(@"~", tilde));
            }
        }

        private static string postProcessReplaceTilde(
            string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }
            else
            {
                if (HttpContext.Current != null &&
                    HttpContext.Current.Response != null)
                {
                    // http://msdn2.microsoft.com/en-us/library/aa479314.aspx.
                    return HttpContext.Current.Response.ApplyAppPathModifier(
                        url);
                }
                else
                {
                    return url;
                }
            }
        }

        private static string replaceTildeComplete(
            string path)
        {
            if (path == null ||
                path.Length <= 0 ||
                path.IndexOf('~') != 0)
            {
                return path;
            }
            else if (
                HttpContext.Current == null ||
                HttpContext.Current.Request == null)
            {
                // Try to lookup from config.
                string tilde = ConfigurationManager.AppSettings[@"replaceTildeFallBack"];

                if (!string.IsNullOrEmpty(tilde))
                {
                    tilde = tilde.TrimEnd('\\', '/');
                    return path.Replace(@"~", tilde);
                }
                else
                {
                    return path;
                }
            }
            else
            {
                string rawUrl = HttpContext.Current.Request.Url.ToString();

                string before = null;
                if (rawUrl != null)
                {
                    int protocolEndPos = rawUrl.IndexOf(@"://");

                    if (protocolEndPos >= 0)
                    {
                        int startSearchPos = protocolEndPos + @"://".Length;
                        int slashPos = rawUrl.IndexOf(
                            @"/",
                            startSearchPos);

                        if (slashPos > startSearchPos)
                        {
                            before = rawUrl.Substring(0, slashPos);
                        }
                    }
                }

                string tilde =
                    HttpContext.Current.Request.ApplicationPath == @"/"
                        ? string.Empty
                        : HttpContext.Current.Request.ApplicationPath;

                // Add the before part.
                if (!string.IsNullOrEmpty(before))
                {
                    tilde = tilde.TrimStart('/');
                    before = before.TrimEnd('/');

                    tilde = before + @"/" + tilde;
                }

                // Fix double slash issue.
                if (tilde != null && tilde.EndsWith(@"/") && path.Contains(@"~/"))
                {
                    path = path.Replace(@"~/", @"~");
                }

                return path.Replace(@"~", tilde);
            }
        }

        // ------------------------------------------------------------------

        #endregion
    }

    /////////////////////////////////////////////////////////////////////////

    public enum RedirectMethod
    {
        #region Using directives.

        // ------------------------------------------------------------------

        /// <summary>
        /// Use the normal 302 HTTP result.
        /// </summary>
        Temporary,

        /// <summary>
        /// Use alternative 301 HTTP result.
        /// See http://www.dnnportal.de/Weblog/tabid/177/EntryID/120/Default.aspx.
        /// </summary>
        Permanent

        // ------------------------------------------------------------------

        #endregion
    }

    /////////////////////////////////////////////////////////////////////////
}