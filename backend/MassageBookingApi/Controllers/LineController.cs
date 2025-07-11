using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MassageBookingApi.Models;
using MassageBookingApi.Services;

namespace MassageBookingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LineController : ControllerBase
    {
        private readonly LineService _lineService;
        private readonly LineConfig _lineConfig;

        public LineController(LineService lineService, IOptions<LineConfig> lineConfig)
        {
            _lineService = lineService;
            _lineConfig = lineConfig.Value;
        }

        [HttpPost("token")]
        public async Task<IActionResult> ExchangeToken([FromBody] LineTokenRequest request)
        {
            try
            {
                Console.WriteLine($"[LINE CONTROLLER DEBUG] Received token exchange request");
                Console.WriteLine($"[LINE CONTROLLER DEBUG] Code: {request?.Code}");
                Console.WriteLine($"[LINE CONTROLLER DEBUG] State: {request?.State}");
                Console.WriteLine($"[LINE CONTROLLER DEBUG] Config - ClientId: {_lineConfig.ClientId}");
                Console.WriteLine($"[LINE CONTROLLER DEBUG] Config - RedirectUrl: {_lineConfig.RedirectUrl}");
                Console.WriteLine($"[LINE CONTROLLER DEBUG] Config - ClientSecret: {_lineConfig.ClientSecret?.Substring(0, Math.Min(4, _lineConfig.ClientSecret.Length))}****");

                if (request == null || string.IsNullOrEmpty(request.Code))
                {
                    Console.WriteLine($"[LINE CONTROLLER ERROR] Invalid request - missing code");
                    return BadRequest(new { error = "Missing authorization code" });
                }

                var tokenResponse = await _lineService.ExchangeCodeForToken(request.Code, request.State);
                
                if (tokenResponse == null)
                {
                    Console.WriteLine($"[LINE CONTROLLER ERROR] Token exchange failed");
                    return BadRequest(new { error = "Failed to exchange token" });
                }

                Console.WriteLine($"[LINE CONTROLLER DEBUG] Token exchange successful, getting user profile");
                var userProfile = await _lineService.GetUserProfile(tokenResponse.access_token);
                
                if (userProfile == null)
                {
                    Console.WriteLine($"[LINE CONTROLLER ERROR] Failed to get user profile");
                    return BadRequest(new { error = "Failed to get user profile" });
                }

                Console.WriteLine($"[LINE CONTROLLER DEBUG] User profile retrieved successfully");
                return Ok(new 
                { 
                    success = true,
                    user = userProfile,
                    accessToken = tokenResponse.access_token
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LINE CONTROLLER EXCEPTION] {ex.Message}");
                Console.WriteLine($"[LINE CONTROLLER EXCEPTION] Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            try
            {
                var success = await _lineService.SendMessage(request.UserId, request.Message);
                
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("send-order")]
        public async Task<IActionResult> SendOrderConfirmation([FromBody] SendOrderRequest request)
        {
            try
            {
                var success = await _lineService.SendOrderConfirmation(request.UserId, request.Items, request.Total);
                
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("config")]
        public IActionResult GetConfig()
        {
            return Ok(new 
            { 
                clientId = _lineConfig.ClientId,
                redirectUrl = _lineConfig.RedirectUrl,
                enableNotifications = _lineConfig.EnableNotifications
            });
        }
    }
}