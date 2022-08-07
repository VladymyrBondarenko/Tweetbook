using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tweetbook.Contracts.Contracts.V1.Requests;
using Tweetbook.Contracts.V1;
using Tweetbook.Contracts.V1.Requests;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Services;

namespace Tweetbook.Controllers.V1
{
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost(ApiRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            var res = await _identityService.RegisterAsync(request.Email, request.Password);

            if (!res.Success)
            {
                return BadRequest(new AuthFailedResponse { Errors = res.Errors });
            }

            return Ok(new AuthSuccessResponse { Token = res.Token, RefreshToken = res.RefreshToken });
        }

        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var res = await _identityService.LoginAsync(request.Email, request.Password);

            if (!res.Success)
            {
                return BadRequest(new AuthFailedResponse { Errors = res.Errors });
            }

            return Ok(new AuthSuccessResponse { Token = res.Token, RefreshToken = res.RefreshToken });
        }

        [HttpPost(ApiRoutes.Identity.FacebookAuth)]
        public async Task<IActionResult> LoginWithFacebook([FromBody] UserFacebookAuthRequest request)
        {
            var res = await _identityService.LoginWithFacebookAsync(request.AccessToken);

            if (!res.Success)
            {
                return BadRequest(new AuthFailedResponse { Errors = res.Errors });
            }

            return Ok(new AuthSuccessResponse { Token = res.Token, RefreshToken = res.RefreshToken });
        }

        [HttpPost(ApiRoutes.Identity.Refresh)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var res = await _identityService.RefreshTokenAsync(request.Token, request.RefreshToken);

            if (!res.Success)
            {
                return BadRequest(new AuthFailedResponse { Errors = res.Errors });
            }

            return Ok(new AuthSuccessResponse { Token = res.Token, RefreshToken = res.RefreshToken });
        }
    }
}
