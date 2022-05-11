using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Neverland.Domain;
using Newtonsoft.Json;

namespace Neverland.Web.Utils
{
    public class UserIdentityFilterAttribute : Attribute, IResourceFilter
    {

        private static Dictionary<string, object> CacheDict = new Dictionary<string, object>();

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            string key = "User";

            //if (!CacheDict.ContainsKey(key))
            //{
            //    string userStr = context.HttpContext.Session.GetString("Login_User");
            //    User user = null;
            //    if (userStr != null)
            //    {
            //        user = JsonConvert.DeserializeObject<User>(userStr);
            //        CacheDict.Add(key, user);
            //    }
            //}
            if (CacheDict.ContainsKey(key))
            {
                // 只要给context.Result赋值了，就会中断后面的执行，直接返回给调用方
                context.Result = (IActionResult)CacheDict[key];
            }
            
            Console.WriteLine("\nplease sign in\n");
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            string key = "User";
            var actionResult = context.Result;
            CacheDict[key] = context.Result;

            Console.WriteLine($"\nOnResourceExecuted: {CacheDict[key]}\n");
        }
    }
}
