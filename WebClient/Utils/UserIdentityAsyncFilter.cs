using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Neverland.Domain;
using Newtonsoft.Json;

namespace Neverland.Web.Utils
{
    public class UserIdentityAsyncFilter : Attribute, IAsyncResourceFilter
    {

        private static Dictionary<string, object> CacheDict = new Dictionary<string, object>();

        public Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            throw new NotImplementedException();
        }
    }
}
