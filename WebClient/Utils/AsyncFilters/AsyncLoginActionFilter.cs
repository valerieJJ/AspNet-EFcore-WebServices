
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Neverland.Domain;
using Neverland.Web.ViewModels;
using Newtonsoft.Json;

namespace Neverland.Web.Utils.AsyncFilters
{
	public class AsyncLoginActionFilter: Attribute, IAsyncActionFilter
    {
        private static ILogger<AsyncLoginActionFilter> _ILogger;

        public AsyncLoginActionFilter(ILogger<AsyncLoginActionFilter> logger)
        {
            _ILogger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controllerName = context.HttpContext.GetRouteValue("controller");
            var actionName = context.HttpContext.GetRouteValue("action");

            var param = context.HttpContext.Request.QueryString.Value;
            _ILogger.LogInformation($"\nexecuting controller:{controllerName}, action:{actionName}, params: {param}\n");
            var userStr = context.HttpContext.Session.GetString("Login_User");
            //_ILogger.LogInformation($"\n\nppplease log in! url={context.HttpContext.Request.Path}, rawpath={context.HttpContext.Request.PathBase}");
            if (userStr == null)
            {
                _ILogger.LogInformation($"\n\nppplease log in! url={context.HttpContext.Request.Path}");
                //var req = new RouteValueDictionary(new Dictionary<string, object>(){
                //                                        {"controller", "User"}, { "action", "Login" }
                //                                     }
                //                                   );
                //context.Result = new RedirectToRouteResult("default", req, true);    //重定向
            }


            ActionExecutedContext executedContext = await next.Invoke();


            var result = JsonConvert.SerializeObject(context.Result);
            _ILogger.LogInformation($"\nexecuted, controller:{controllerName}, action:{actionName}，result:{result}\n");

        }
    }
}