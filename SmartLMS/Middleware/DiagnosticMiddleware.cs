using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SmartLMS.API.Middleware
{
    public class DiagnosticMiddleware
    {
        private readonly RequestDelegate _next;

        public DiagnosticMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // سجل تفاصيل الطلب
            Console.WriteLine($"Request Path: {context.Request.Path}");
            Console.WriteLine($"Request Method: {context.Request.Method}");

            // سجل محتوى الطلب
            if (context.Request.ContentLength > 0)
            {
                context.Request.EnableBuffering();

                using (StreamReader reader = new StreamReader(
                    context.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    var body = await reader.ReadToEndAsync();
                    Console.WriteLine($"Request Body: {body}");

                    // إعادة تعيين موضع القارئ
                    context.Request.Body.Position = 0;
                }
            }

            // التقط الاستجابة
            var originalBodyStream = context.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                try
                {
                    await _next(context);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in middleware pipeline: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    throw;
                }

                // سجل الاستجابة
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                Console.WriteLine($"Response Status: {context.Response.StatusCode}");
                Console.WriteLine($"Response Body: {responseText}");

                // نسخ الاستجابة مرة أخرى إلى الدفق الأصلي
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
    }

    // امتداد للتسجيل في خط الأنابيب
    public static class DiagnosticMiddlewareExtensions
    {
        public static IApplicationBuilder UseDiagnostics(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DiagnosticMiddleware>();
        }
    }
}