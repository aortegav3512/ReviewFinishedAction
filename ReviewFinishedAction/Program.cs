using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ReviewFinishedAction
{
    class Program
    {
        async static Task Main(string[] args)
        {
            Console.WriteLine("Starting action...");            
            string version = Environment.GetEnvironmentVariable("GITHUB_HEAD_REF");
            string repository = Environment.GetEnvironmentVariable("GITHUB_REPOSITORY");
            string productId = repository.Substring(repository.LastIndexOf("/") + 1);

            Console.WriteLine($"ProductId: {productId}");
            Console.WriteLine($"Version: {version}");

            string token = await GenerateToken();
            if (!String.IsNullOrEmpty(token))
            {
                await NotifyReviewAsync(productId, version, token);
            }
        }

        private async static Task NotifyReviewAsync(string productId, string version, string token)
        {
            Console.WriteLine($"NotifyReviewAsync for productId {productId} and version {version}");
            AddReviewDTO addReviewDTO = new AddReviewDTO();
            addReviewDTO.Version = version;
            addReviewDTO.Result = false; //TODO:fix
            var contentSerialized = JsonConvert.SerializeObject(addReviewDTO);
            var content = new StringContent(contentSerialized, Encoding.UTF8);

            var url = Environment.GetEnvironmentVariable("APIURL");
            Console.WriteLine($"URL: {url}");

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(url);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            await httpClient.PostAsync($"products/{productId}/reviews/", content);
        }

        private async static Task<string> GenerateToken()
        {
            string accessToken = "";
            string tokenURL = Environment.GetEnvironmentVariable("TOKENURL");
            string username = Environment.GetEnvironmentVariable("TOKENUSERNAME");
            string password = Environment.GetEnvironmentVariable("TOKENPASSWORD");

            Console.WriteLine($"TokenURL: {tokenURL}, username:{username}");

            string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(username + ":" + password));

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var postMessage = new Dictionary<string, string>();
            postMessage.Add("grant_type", "client_credentials");
            postMessage.Add("scope", "kong");

            var request = new HttpRequestMessage(HttpMethod.Post, tokenURL)
            {
                Content = new FormUrlEncodedContent(postMessage)
            };

            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Token obtained...");
                var responseContent = await response.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<Token>(responseContent);
                if (token != null)
                {
                    accessToken = token.access_token;
                    Console.WriteLine($"Access token:{accessToken.Substring(0, 5)}");
                }
                else
                {
                    Console.WriteLine($"Access token is null");
                }
            }
            else
            {
                Console.WriteLine($"Error getting response. Status code: {response.StatusCode}");
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Content: {content}");
            }
            return accessToken;
        }
    }
}
