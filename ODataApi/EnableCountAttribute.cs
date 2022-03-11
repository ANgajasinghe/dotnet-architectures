using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.Extensions.Primitives;

namespace ODataApi;

public class EnableCountAttribute : ActionFilterAttribute
{


    public EnableCountAttribute()
    {
     
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.HttpContext.Request.Query.Any(x => x.Key != "$count"))
        {
            context.HttpContext.Request.Query
                .ToList()
                .Add(new KeyValuePair<string, StringValues>("$count", "true"));
        }

    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.HttpContext.Request.Query.Any(x => x.Key == "$count"))
        {
            context.HttpContext.Request.Query
                .ToList()
                .Remove(new KeyValuePair<string, StringValues>("$count", "true"));
        }
    }
    
}


