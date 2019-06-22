using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebFramework
{
   public class ApiResultFilterAttribute:ActionFilterAttribute
    {




        public override void OnResultExecuting(ResultExecutingContext context)
        {
            
            if (context.Result is OkObjectResult okbjectResult)
            {
                var apiResult = new ApiResult<object>(true, Common.ApiResultStatusCode.Success, okbjectResult.Value);
                context.Result = new JsonResult(apiResult) { StatusCode=okbjectResult.StatusCode};
            }


           else if (context.Result is OkResult okResult)
            {
                var apiResult = new ApiResult(true, Common.ApiResultStatusCode.Success);
                context.Result = new JsonResult(apiResult) { StatusCode = okResult.StatusCode };
            }

           else if(context.Result is BadRequestResult badRequestResult)
            {
                var apiResult = new ApiResult(false, Common.ApiResultStatusCode.BadRequest);
                context.Result = new JsonResult(apiResult) { StatusCode = badRequestResult.StatusCode };

            }


            //متن مشکل داره دیکشنری مدا استیت مت ارور اور لود
            else if (context.Result is BadRequestObjectResult badRequestObjectResult)
            {
                var message = badRequestObjectResult.Value.ToString();
                if (badRequestObjectResult.Value is SerializableError errors)
                {
                    var errorMessages = errors.SelectMany(p => (string[])p.Value).Distinct();
                    message = string.Join("|", errorMessages);
                }
                var apiResult = new ApiResult(false, Common.ApiResultStatusCode.BadRequest, message);
                context.Result = new JsonResult(apiResult) { StatusCode = badRequestObjectResult.StatusCode };
            }

            //متن بنویسیم 
            else if (context.Result is ContentResult contentResult)
            {
                var apiResult = new ApiResult(true, Common.ApiResultStatusCode.Success, contentResult.Content);
                context.Result = new JsonResult(apiResult) { StatusCode = contentResult.StatusCode };
            }
            else if(context.Result is NotFoundResult notFoundResult)
            {
                var apiResult = new ApiResult(false, Common.ApiResultStatusCode.NotFound);
                context.Result = new JsonResult(apiResult) { StatusCode = notFoundResult.StatusCode };
            }

            else if (context.Result is NotFoundObjectResult notFoundObjectResult)
            {
                var apiResult = new ApiResult<object>(false, Common.ApiResultStatusCode.NotFound, notFoundObjectResult.Value);
                context.Result = new JsonResult(apiResult) { StatusCode = notFoundObjectResult.StatusCode };

            }
            //فقط یوزر استتوس کد nul return user
            //return file جزو انهاس ک نمیشه تبدیل کرد
            else if(context.Result is ObjectResult objectResult && objectResult.StatusCode==null
                && !(objectResult.Value is ApiResult))
            {
                var apiResult = new ApiResult<object>(true, Common.ApiResultStatusCode.Success, objectResult.Value);
                context.Result = new JsonResult(apiResult) { StatusCode = objectResult.StatusCode };

            }
            base.OnResultExecuting(context);    
        }
    }
}
