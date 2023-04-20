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
            var response = await _httpClient.GetAsync("");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
	}
}

