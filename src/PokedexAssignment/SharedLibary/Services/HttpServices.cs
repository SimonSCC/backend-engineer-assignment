
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SharedLibary.Services
{
    public class HTTPServices
    {
        HttpClient ApiClient { get; set; }

        public HTTPServices()
        {
            ApiClient = new HttpClient();
        }



        public async Task<string> GETModel(string url)
        {
            try
            {
                using (HttpResponseMessage response = await ApiClient.GetAsync(url))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string resultAsString = await response.Content.ReadAsStringAsync();
                        return resultAsString;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
