using System;
namespace DotnetPolly.Services
{
	public class ExternalApiService
	{
        private readonly HttpClient _httpClient;

        public ExternalApiService(HttpClient httpClient)
		{
            this._httpClient = httpClient;
        }

        public async Task<string> CallExternalAPI()
        {
            var response = await _httpClient.GetAsync("http://localhost:5185/");
            Console.WriteLine("Status Code = " + response.StatusCode.ToString());
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        public async Task CallExternalAPIOnLoop()
        {
          while (true)
          {
            Parallel.For(0, 10, async count => {
                using var client = new HttpClient();
                using var response = await client.GetAsync("http://localhost:5185/");
                Console.WriteLine("Status Code = " + response.StatusCode.ToString());
            });
          }
           
        }
	}
}

