using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIX.SCS.Tools.Helper;

public static class UrlHelper
{
    /// <summary>
    ///     Replaces placeholders in url with actual values. If no placeholder exists, the value is added to query string.
    /// </summary>
    public static string UpdateUrlParams(string url, IDictionary<string, string> paramsToSet)
    {
        if (paramsToSet == null || !paramsToSet.Any())
        {
            return url;
        }

        var resolvedPath = url;
        foreach (var p in paramsToSet)
        {
            if (resolvedPath.Contains($"{{{p.Key}}}"))
            {
                // param is within url, e.g. "//root/child/{param}/edit"
                resolvedPath = resolvedPath.Replace($"{{{p.Key}}}", HttpUtility.UrlEncode(p.Value));
            }
            else if (p.Value != null)
            {
                // param is added as query parameter
                var querySeparator = resolvedPath.Contains("?") ? "&" : "?";
                resolvedPath += $"{querySeparator}{HttpUtility.UrlEncode(p.Key)}={HttpUtility.UrlEncode(p.Value)}";
            }
        }

        return resolvedPath;
    }
}