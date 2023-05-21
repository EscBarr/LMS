using System.Net;
using Microsoft.AspNetCore.Mvc;

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