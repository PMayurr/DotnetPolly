﻿using DotnetPolly.Services;
using Microsoft.AspNetCore.RateLimiting;
using System.Globalization;
using System.Threading.RateLimiting;
using Polly;
using Polly.Extensions.Http;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRateLimiter(options => {
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    RateLimitPartition.GetConcurrencyLimiter(
        partitionKey : context.Request.Headers.Host.ToString(),
        factory: partition=> new ConcurrencyLimiterOptions
        {
            PermitLimit = 3
        }
    )
    );
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// builder.Services.AddRateLimiter(_ =>
// {
//     _.OnRejected = (context, _) =>
//     {
//         if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
//         {
//             context.HttpContext.Response.Headers.RetryAfter =
//                 ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
//         }

//         context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
//         context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.");

//         return new ValueTask();
//     };
//     _.GlobalLimiter = PartitionedRateLimiter.CreateChained(
//         PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
//         {
//             var userAgent = httpContext.Request.Headers.UserAgent.ToString();

//             return RateLimitPartition.GetFixedWindowLimiter
//             (userAgent, _ =>
//                 new FixedWindowRateLimiterOptions
//                 {
//                     AutoReplenishment = true,
//                     PermitLimit = 4,
//                     Window = TimeSpan.FromSeconds(20)
//                 });
//         })
//     );
// });

// Add services to the container.
builder.Services.AddTransient<ExternalApiService, ExternalApiService>();

// Allow up to 20 executions per second, with a delegate to return the
// retry-after value to use if the rate limit is exceeded.
var rateLimtiPolicy = Policy.RateLimit(4, TimeSpan.FromSeconds(20), (retryAfter, context) =>
{
    return retryAfter.Add(TimeSpan.FromSeconds(21));
});

builder.Services.AddHttpClient<ExternalApiService>(httpClient =>
{
    httpClient.BaseAddress = new Uri("https://localhost:7178/");
});
    //.AddTransientHttpErrorPolicy(policy => policy.);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

static string GetTicks() => (DateTime.Now.Ticks & 0x11111).ToString("00000");

app.MapGet("/", () => Results.Ok($"Hello {GetTicks()}"));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseRateLimiter();

app.Run();

