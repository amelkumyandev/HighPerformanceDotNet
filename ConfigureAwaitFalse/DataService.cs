namespace ConfigureAwaitFalse
{
    public class DataService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<string> FetchDataAsync(string url)
        {
            // Simulate a real HTTP request to fetch data
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string data = await response.Content.ReadAsStringAsync();
            return data;
        }
    }
}
