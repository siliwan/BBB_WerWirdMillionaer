using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend
{
    public class PinSessionActionFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if(string.IsNullOrWhiteSpace(context.HttpContext.Session.GetString("SessionId")))
            {
                var SessionId = context.HttpContext.Session.Id;
                context.HttpContext.Session.SetString("SessionId", SessionId);
                context.HttpContext.Session.CommitAsync().GetAwaiter().GetResult();
            }
            base.OnResultExecuting(context);
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            if (!context.HttpContext.Response.Headers.TryGetValue("X-Quiz-Session-Id", out _))
            {
                context.HttpContext.Response.Headers.Add("X-Quiz-Session-Id", Guid.NewGuid().ToString());
            }
            base.OnResultExecuted(context);
        }
    }
}
