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


        [HttpPost("Login")]
        public async Task<LoginResponse> Login([FromBody] LoginRequestModel request)
        {
            try
            {
                var result = await _meshcapadeService.Login(request.Username, request.Password);
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpPost("CreateAvatar")]
        public async Task<IActionResult> CreateAvatar([FromForm] CreateAvatarRequest request)
        {
            try
            {
                return Ok(await _meshcapadeService.GenerateAvatar(request.Token, request.UploadedFile, request.MediaType));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }


        [HttpPost("GetAvatar")]
        public async Task<IActionResult> GetAvatar([FromBody] GetAvatarRequest request)
        {
            try
            {
                return Ok(await _meshcapadeService.GetAvatar(request.Token, request.AssetId));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }



        [HttpPost("ExportAvatar")]
        public async Task<IActionResult> ExportAvatar([FromBody] GetAvatarRequest request)
        {
            try
            {
                var exportedAvatar = await _meshcapadeService.ExportAvatar(request.Token, request.AssetId);
                if (exportedAvatar == null)
                    return NoContent();
                return Ok(exportedAvatar);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }








    }

    public class LoginRequestModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }


    public class CreateAvatarRequest
    {
        public string Token { get; set; }
        public string MediaType { get; set; }
        public IFormFile UploadedFile { get; set; }
    }


    public class GetAvatarRequest
    {
        public string Token { get; set; }
        public string AssetId { get; set; }
    }
}
