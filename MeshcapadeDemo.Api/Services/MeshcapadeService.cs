using MeshcapadeDemo.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace MeshcapadeDemo.Api.Services
{
    public class MeshcapadeService
    {
        private readonly HttpClient _httpClient;
        public MeshcapadeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LoginResponse> Login(string username, string password)
        {
            string url = "https://auth.meshcapade.com/realms/meshcapade-me/protocol/openid-connect/token";

            var formContent = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("client_id", "meshcapade-me"),
            new KeyValuePair<string, string>("username", username),
            new KeyValuePair<string, string>("password", password)
            });

            // Set the Content-Type header
            formContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            HttpResponseMessage response = await _httpClient.PostAsync(url, formContent);

            // Ensure the response status code is successful
            response.EnsureSuccessStatusCode();

            // Read and return the response content
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return result;
        }



        public async Task<bool> GenerateAvatar(string token, IFormFile uploadedFile, string media)
        {
            //Initiate Avatar Creation
            var response = await _httpClient.PostAsync($"avatars/create/from-{media}", null);
            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadAsStringAsync();
            var jsonObject = JObject.Parse(responseData);


            string assetId = jsonObject["data"]["id"].ToString();


            //Request for Media Upload URL
            response = await _httpClient.PostAsync($"/avatars/{assetId}/{media}", null);
            response.EnsureSuccessStatusCode();

            string path = JObject.Parse(await response.Content.ReadAsStringAsync())["data"]["attributes"]["url"]["path"].ToString();



            //Upload Media to the URL
            response = await _httpClient.PutAsync(path, new StreamContent(uploadedFile.OpenReadStream()));
            response.EnsureSuccessStatusCode();


            //Start Fitting Process
            response = await _httpClient.PostAsync($"avatars/{assetId}/fit-to-{media}", null);

            return response.StatusCode == System.Net.HttpStatusCode.OK;

        }

    }
}


