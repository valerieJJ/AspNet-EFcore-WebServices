using Microsoft.AspNetCore.Mvc.Filters;

namespace MovieService.Utils
{
    public class MovieResourceFilterAttribute : Attribute, IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            Console.WriteLine("\nOnResourceExecuted\n");
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            Console.WriteLine("\nOnResourceExecuting\n");
        }
    }
}
