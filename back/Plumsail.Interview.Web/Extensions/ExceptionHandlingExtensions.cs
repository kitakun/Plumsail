using Plumsail.Interview.Web.Middleware;

namespace Plumsail.Interview.Web.Extensions;

public static class ExceptionHandlingExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}

