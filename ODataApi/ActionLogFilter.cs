using System.Diagnostics;
using System.Diagnostics.Tracing;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ODataApi;

public class ActionLogFilter : IActionFilter
{
    private readonly Stopwatch stopwatch;
    private DateTime traceStart;

    public ActionLogFilter()
    {
        stopwatch = new Stopwatch();
        Console.WriteLine("New Incomming Reqiuest");
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        traceStart = DateTime.UtcNow;
        stopwatch.Start();
        Console.WriteLine("Reqiuest Started at :" + DateTime.Now);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        stopwatch.Stop();
        var traceEnd = traceStart
            .AddMilliseconds(stopwatch.ElapsedMilliseconds);

        Console.WriteLine("Reqiuest Ended at :" + DateTime.Now);
        Console.WriteLine("Request :" + context.HttpContext.Request.GetDisplayUrl());
        Console.WriteLine("trace End :" + traceEnd);
        Console.WriteLine("Request Processing Time :" + stopwatch.ElapsedMilliseconds + "ms");
    }
}

[EventSource(Name = "Sample.EventCounter.Minimal")]
public sealed class MinimalEventCounterSource : EventSource
{
    public static readonly MinimalEventCounterSource Log = new();

    private EventCounter _requestCounter;

    private MinimalEventCounterSource()
    {
        _requestCounter = new EventCounter("request-time", this)
        {
            DisplayName = "Request Processing Time",
            DisplayUnits = "ms"
        };
    }

    public void Request(string url, long elapsedMilliseconds)
    {
        WriteEvent(1, url, elapsedMilliseconds);
        _requestCounter?.WriteMetric(elapsedMilliseconds);
    }

    protected override void Dispose(bool disposing)
    {
        _requestCounter?.Dispose();
        _requestCounter = null;

        base.Dispose(disposing);
    }
}

public class LogRequestTimeFilterAttribute : ActionFilterAttribute
{
    private readonly Stopwatch _stopwatch = new();

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        _stopwatch.Start();
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        _stopwatch.Stop();

        MinimalEventCounterSource.Log.Request(
            context.HttpContext.Request.GetDisplayUrl(), _stopwatch.ElapsedMilliseconds);
    }
}