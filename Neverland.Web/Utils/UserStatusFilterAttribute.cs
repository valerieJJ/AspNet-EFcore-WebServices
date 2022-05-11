using Microsoft.AspNetCore.Mvc.Filters;
using Neverland.Domain;
using Newtonsoft.Json;

namespace Neverland.Web.Utils
{
    public class UserStatusFilterAttribute : Attribute, IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            string userStr = context.HttpContext.Session.GetString("Login_User");
            User user = null;
            if (userStr != null)
            {
                user = JsonConvert.DeserializeObject<User>(userStr);
                var roleType = user.Role.GetType();
                Console.WriteLine("role = {0}", user.Role);
                var role = user.Role.ToString();

                if (role == "admin")
                {
                    Console.WriteLine("\nAdmin OnResourceExecuted\n");
                }
                else
                {
                    Console.WriteLine("\nUser OnResourceExecuted\n");
                }
            }
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            string userStr = context.HttpContext.Session.GetString("Login_User");
            User user = null;
            if (userStr != null)
            {
                user = JsonConvert.DeserializeObject<User>(userStr);
                var roleType = user.Role.GetType();
                Console.WriteLine("role = {0}", user.Role);
                var role = user.Role.ToString();

                if (role == "admin")
                {
                    Console.WriteLine("\nAdmin OnResourceExecuted\n");
                }
                else
                {
                    Console.WriteLine("\nUser OnResourceExecuted\n");
                }
            }
        }
    }
}
