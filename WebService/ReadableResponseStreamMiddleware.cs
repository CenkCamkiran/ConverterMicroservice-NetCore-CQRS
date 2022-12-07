using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WebService
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ReadableResponseStreamMiddleware
    {
        private readonly RequestDelegate _next;

        public ReadableResponseStreamMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Stream originalBody = httpContext.Response.Body;

            try
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    httpContext.Response.Body = memStream;

                    await _next(httpContext);

                    memStream.Position = 0;
                    string responseBody = await new StreamReader(memStream).ReadToEndAsync();

                    memStream.Position = 0;
                    await memStream.CopyToAsync(originalBody);
                }
            }
            finally
            {
                httpContext.Response.Body = originalBody;
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ReadableResponseStreamMiddlewareExtensions
    {
        public static IApplicationBuilder UseReadableResponseStreamMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ReadableResponseStreamMiddleware>();
        }
    }
}
