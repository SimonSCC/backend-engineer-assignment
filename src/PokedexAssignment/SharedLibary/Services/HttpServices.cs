
using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SharedLibary.Services
{
    public class HTTPServices
    {
        HttpClient ApiClient { get; set; }

        public HTTPServices()
        {
            HttpClientHandler handler = new();
            handler.ServerCertificateCustomValidationCallback = VerifySSL;
            ApiClient = new HttpClient(handler);
        }
        private bool VerifySSL(HttpRequestMessage requestMessage, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors sslErrors)
        {
            return true;
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
