using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Neverland.Domain;
using Neverland.Web.ViewModels;
using Newtonsoft.Json;

namespace Neverland.Web.Utils
{
    public class UserResourceFilterAttribute : Attribute, IResourceFilter
    {
        private static Dictionary<string, object> CacheDict = new Dictionary<string, object>();

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            string key = "User";
            if (CacheDict.ContainsKey(key))
            {

                var actionResult = (IActionResult)CacheDict[key];
                if (!actionResult.GetType().Equals(typeof(RedirectToActionResult)))
                {

                    // 只要给context.Result赋值了，就会中断后面的执行，直接返回给调用方
                    context.Result = (IActionResult)CacheDict[key];
                    ViewResult accountViewModel = (ViewResult)CacheDict[key];
                    if(accountViewModel != null)
                    {
                        AccountViewModel model = (AccountViewModel)accountViewModel.Model;

                        var role = model.UserViewModel.Role.ToString();
                        if (role == "admin")
                        {
                            Console.WriteLine("\nAdmin OnResourceExecuting\n");
                        }
                        else
                        {
                            Console.WriteLine("\nUser OnResourceExecuting\n");
                        }
                        Console.WriteLine($"\nOnResourceExecuted: {CacheDict[key]}\n");
                    }
                }
                    
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            string key = "User";
            var actionResult = context.Result;

            if (!actionResult.GetType().Equals(typeof(RedirectToActionResult)))
            {
                CacheDict[key] = context.Result;

                //ViewResult accountViewModel = (ViewResult) CacheDict[key];
                //if(accountViewModel != null)
                //{
                //    AccountViewModel model = (AccountViewModel) accountViewModel.Model;

                //    var role = model.UserViewModel.Role.ToString();

                //    if (role == "admin")
                //    {
                //        Console.WriteLine("\nAdmin OnResourceExecuted\n");
                //    }
                //    else
                //    {
                //        Console.WriteLine("\nUser OnResourceExecuted\n");
                //    }
                //    Console.WriteLine($"\nOnResourceExecuted: {CacheDict[key]}\n");
                //}

                }

            }
        
    }
}
