using System;
using Polly;
using Polly.Extensions.Http;

namespace DotnetPolly.PollyPolicy
{
	public class ExternalApiRateLimitPolicy
	{
		public ExternalApiRateLimitPolicy()
		{
		}

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                //.rate
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                                                                            //retryAttempt)));
        }
    }
}

