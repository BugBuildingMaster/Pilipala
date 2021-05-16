using MvcApp.Controllers;
using System.Web;
using System.Web.Mvc;
using MvcThrottle;

namespace MvcApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //访问拦截器，超过限制的请求将被拦截
            var throttleFilter = new ThrottlingFilter
            {
                Policy = new ThrottlePolicy(perSecond: 1, perMinute: 10, perHour: 60 * 10, perDay: 600 * 10)
                {
                    IpThrottling = true
                },
                Repository = new CacheRepository()
            };
            filters.Add(throttleFilter);

            filters.Add(new HandleErrorAttribute());
        }
    }
}
