using EventSourcing.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using Serilog.Events;
using System.Net;

namespace EventSourcing.Extensions;

public static class AppExceptionExtensions
{
    public static void UseAppExceptionHandler(this IApplicationBuilder appBuilder)
    {
        appBuilder.UseExceptionHandler(x =>
        {
            x.Run(async context =>
            {
                var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (errorFeature != null)
                {
                    AppExceptionModel result = null;

                    if (errorFeature.Error is not AppException error)
                    {
                        if (errorFeature.Error is OperationCanceledException || errorFeature.Error is TaskCanceledException)
                        {
                            result = new AppExceptionModel
                            {
                                Type = errorFeature.Error.GetType().Name,
                                Message = errorFeature.Error.Message
                            };
                            context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                            await context.Response.WriteAsJsonAsync(result);
                            return;
                        }
                        else
                        {
                            if (Log.Logger.IsEnabled(LogEventLevel.Error))
                                Log.Logger.Error(errorFeature.Error,
                                    $"{errorFeature.Endpoint?.DisplayName} -> Exception occurred:\r\n");
                            throw errorFeature.Error;
                        }
                    }
                    else
                    {

                        result = new AppExceptionModel
                        {
                            Type = error.GetType().Name,
                            Message = error.Message
                        };
                        context.Response.StatusCode = error.HttpStatusCode;
                    }
                    await context.Response.WriteAsJsonAsync(result);
                }
            });
        });
    }
}
