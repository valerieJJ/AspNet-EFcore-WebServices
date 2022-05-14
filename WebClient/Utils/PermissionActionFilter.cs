using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Neverland.Domain;
using Neverland.Web.ViewModels;
using Newtonsoft.Json;

namespace Neverland.Web.Utils
{
	public class PermissionActionFilter: Attribute, IActionFilter
	{
        private static ILogger<PermissionActionFilter> _ILogger;

        public PermissionActionFilter(ILogger<PermissionActionFilter> logger)
		{
            _ILogger = logger;
		}


        public void OnActionExecuting(ActionExecutingContext context)
        {
            var param = context.HttpContext.Request.QueryString.Value;
            var controllerName = context.HttpContext.GetRouteValue("controller");
            var actionName = context.HttpContext.GetRouteValue("action");
            _ILogger.LogInformation($"\nexecuting controller:{controllerName}, action:{actionName}, params: {param}\n");

            var userStr = context.HttpContext.Session.GetString("Login_User");
            _ILogger.LogInformation($"\n\nppplease log in! url={context.HttpContext.Request.Path}, rawpath={context.HttpContext.Request.PathBase}");
            if (userStr == null)
            {
                _ILogger.LogInformation($"\n\nppplease log in! url={context.HttpContext.Request.Path}");
                //var req = new RouteValueDictionary( new Dictionary<string, object>(){
                //                                            { "controller", "User" }
                //                                            , { "action", "Login" }
                //                                        }
                //                                   );
                //context.Result = new RedirectToRouteResult("default"
                //                                            , req
                //                                            , true);    //重定向
            }
            else
            {
                User user = JsonConvert.DeserializeObject<User>(userStr);
                var role = user.Role;//.ToString();
                if (role == Role.admin)
                {
                    Console.WriteLine("\nadmin executing\n");
                }
                else
                {
                    Console.WriteLine("\nuser executing\n");
                    
                    context.Result = new JsonResult(new AccountViewModel());
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {


            //var result = JsonConvert.SerializeObject(context.Result);
            var controllerName = context.HttpContext.GetRouteValue("controller");
            var actionName = context.HttpContext.GetRouteValue("action");
            _ILogger.LogInformation($"\nexecuted, controller:{controllerName}, action:{actionName}\n");

        }
    }
}

