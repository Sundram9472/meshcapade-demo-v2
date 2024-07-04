using MeshcapadeDemo.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Net.Http.Headers;
using System.Text;

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



        public async Task<string> GenerateAvatar(string token, IFormFile uploadedFile, string media)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


            //Initiate Avatar Creation
            var response = await _httpClient.PostAsync($"avatars/create/from-{media}", null);
            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadAsStringAsync();
            var jsonObject = JObject.Parse(responseData);

            string assetId = jsonObject["data"]["id"].ToString();

            //Request for Media Upload URL
            response = await _httpClient.PostAsync(requestUri: $"avatars/{assetId}/{media}", null);
            response.EnsureSuccessStatusCode();

            string path = JObject.Parse(await response.Content.ReadAsStringAsync())["data"]["attributes"]["url"]["path"].ToString();


            //Upload Media to the URL
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Put, path);
            using (var content = new StreamContent(uploadedFile.OpenReadStream()))
            {
                request.Content = content;
                response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }

            dynamic payload = new ExpandoObject();
            payload.avatarname = $"{media}_{assetId}";
            
            if (media == "images")
                payload.imageMode = "AFI";

            string jsonPayload = JsonConvert.SerializeObject(payload);

            //Start Fitting Process
            response = await _httpClient.PostAsync($"avatars/{assetId}/fit-to-{media}", new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            return assetId;

        }


        public async Task<AvatarMeasurements> GetAvatar(string token, string assetId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync(requestUri: $"avatars/{assetId}");
            response.EnsureSuccessStatusCode();
            return  await response.Content.ReadFromJsonAsync<AvatarMeasurements>();
        }


        public async Task<AvatarExport> ExportAvatar(string token, string assetId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var avatar = await GetAvatar(token, assetId);
            if (avatar.data.attributes.state != "READY")
                return null;


                var requestBody = new
                {
                    format = "obj",
                    pose = "a"
                };

            var response = await _httpClient.PostAsJsonAsync($"avatars/{assetId}/export", requestBody);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AvatarExport>();
        }

    }
}


