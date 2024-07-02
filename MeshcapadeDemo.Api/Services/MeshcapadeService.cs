using MeshcapadeDemo.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace MeshcapadeDemo.Api.Services
{
    public class MeshcapadeService
    {
        private readonly HttpClient _httpClient;
        public MeshcapadeService(HttpClient httpClient) { 
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

        public async Task<string> InitiateAvatarCreation(string token, IFormFile imgFile)
        {
            var url = "https://api.meshcapade.com/api/v1/avatars/create/from-images";
            var formData = new MultipartFormDataContent();

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            formData.Add(new StringContent(token), "token");

            using (var stream = imgFile.OpenReadStream())
            {
                formData.Add(new StreamContent(stream), "file", imgFile.FileName);

                var response = await _httpClient.PostAsync(url, formData);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Failed to create avatar: {response.ReasonPhrase}");
                }
                var responseData = await response.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(responseData);
                string assetId = jsonObject["data"]["id"].ToString();
                return assetId;
            }
        }

        public async Task<string> RequestImageUploads(string token, string assetID)
        {
            var url = $"https://api.meshcapade.com/api/v1/avatars/{assetID}/images";
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            request.Headers.Add("Authorization", $"Bearer {token}");
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to create avatar: {response.ReasonPhrase}");
            }
            var responseData = await response.Content.ReadAsStringAsync();
            var jsonObject = JObject.Parse(responseData);
            string path = jsonObject["data"]["attributes"]["url"]["path"].ToString();

            return path;
        }

        public async Task<bool> UploadImageToS3(string token, IFormFile file, string uploadUrl)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Put, uploadUrl);
            using (var content = new StreamContent(file.OpenReadStream()))
            {
                request.Content = content;
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task<bool> StartFittingProcess(string assetId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsync($"avatars/{assetId}/fit-to-images", null);
            response.EnsureSuccessStatusCode();
            return true;
        }
    }
}
