using MeshcapadeDemo.Api.Services;
using MeshcapadeDemo.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace MeshcapadeDemo.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly MeshcapadeService _meshcapadeService;

        public HomeController(MeshcapadeService meshcapadeService)
        {
            _meshcapadeService = meshcapadeService;
        }

        [HttpGet("Heartbeat")]
        public async Task<IActionResult> Heartbeat()
        {
            return Ok(new { message = "API endpoint is working fine!" });
        }


        [HttpGet("Login")]
        public async Task<LoginResponse> Login(string username, string password)
        {
            try
            {
                var result = await _meshcapadeService.Login(username, password);
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpPost("CreateAvatarFromImage")]
        public async Task<bool> CreateAvatarFromImage(string token, IFormFile uploadedFile)
        {
            try
            {
                var assetID = await _meshcapadeService.InitiateAvatarCreation(token, uploadedFile);

                // Call GetPath method to retrieve path using assetID
                var path = await _meshcapadeService.RequestImageUploads(token, assetID);

                var isUploaded = await _meshcapadeService.UploadImageToS3(token, uploadedFile, path);
                if (isUploaded)
                    return (await _meshcapadeService.StartFittingProcess(assetID, token));
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

       
    }
}
