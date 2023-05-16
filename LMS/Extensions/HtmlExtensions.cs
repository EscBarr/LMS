using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace LMS.Extensions
{
    public static class HtmlExtensions
    {
        public static string UnencodedRouteLink(this IUrlHelper helper, string routeName, object routeValues)
        {
            return WebUtility.UrlDecode(helper.RouteUrl(routeName, routeValues));
        }
    }
}