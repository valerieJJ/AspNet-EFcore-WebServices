//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Neverland.WebClient.Models;

//namespace Neverland.Web.Utils
//{
//    public class JsonResultFilter : Attribute, IResultFilter
//    {
//        public void OnResultExecuting(ResultExecutingContext context)
//        {
//            if(context.Result is JsonResult)
//            {
//                JsonResult result = (JsonResult)context.Result;
//                context.Result = new JsonResult(new AjaxResult(){
//                    Success = true,
//                    Message = "Ok",
//                    Data = result.Value
//                });
//            }
//            Console.WriteLine("\nJsonResultFilter   OnResultExecuting\n");
//        }
//        public void OnResultExecuted(ResultExecutedContext context)
//        {
//        }
//    }
//}
