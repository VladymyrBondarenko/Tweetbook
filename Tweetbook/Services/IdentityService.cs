using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tweetbook.Data;
using Tweetbook.Domain;
using Tweetbook.Options;

namespace Tweetbook.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettingsOptions _jwtSettings;
        private readonly TokenValidationParameters _tokenParams;
        private readonly DataContext _dataContext;
        private readonly IFacebookAuthService _facebookAuthService;

        public IdentityService(
            UserManager<IdentityUser> userManager, JwtSettingsOptions jwtSettings,
            TokenValidationParameters tokenParams, DataContext dataContext,
            IFacebookAuthService facebookAuthService)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _tokenParams = tokenParams;
            _dataContext = dataContext;
            _facebookAuthService = facebookAuthService;
        }

        public async Task<AuthenticationResult> RegisterAsync(string email, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { $"User with email '{email}' already exists. Try to use another one." }
                };
            }

            var user = new IdentityUser
            {
                Email = email,
                UserName = email
            };

            var createdUser = await _userManager.CreateAsync(user, password);

            if (!createdUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    Errors = createdUser.Errors.Select(x => x.Description)
                };
            }

            return await generateAuthResultAsync(user);
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { $"User doest not exist." }
                };
            }

            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                return new AuthenticationResult
                {
                    Errors = new [] { "Password is not correct." }
                };
            }

            return await generateAuthResultAsync(user);
        }

        public async Task<AuthenticationResult> LoginWithFacebookAsync(string accessToken)
        {
            var validationResult = await _facebookAuthService.ValidateAccessTokenAsync(accessToken);

            if (!validationResult.Data.IsValid)
            {
                return new AuthenticationResult { Errors = new[] { validationResult.Data.Error.Message } };
            }

            var userInfo = await _facebookAuthService.GetUserInfo(accessToken);

            var user = await _userManager.FindByEmailAsync(userInfo.Email);

            if(user == null)
            {
                user = new IdentityUser
                {
                    Email = userInfo.Email,
                    UserName = userInfo.Email
                };

                var identityResult = await _userManager.CreateAsync(user);

                if (!identityResult.Succeeded)
                {
                    return new AuthenticationResult { Errors = identityResult.Errors.Select(x => x.Description) };
                }
            }

            return await generateAuthResultAsync(user);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, Guid refreshToken)
        {
            var validatedToken = getPrincipalFromToken(token);

            if(validatedToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "Invalid Token" } };
            }

            var expiryDateUnix =
                long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expityDateUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if(expityDateUtc > DateTime.UtcNow)
            {
                return new AuthenticationResult { Errors = new[] { "This token hasn't expired yet" } };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await _dataContext.RefreshTokens.FindAsync(refreshToken);

            if(storedRefreshToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token does not exist" } };
            }

            if(DateTime.UtcNow > expityDateUtc)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has expired" } };
            }

            if (storedRefreshToken.Invalidated)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has been invalidated" } };
            }

            if (storedRefreshToken.IsUsed)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has been used" } };
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token does not match this JWT" } };
            }

            storedRefreshToken.IsUsed = true;

            _dataContext.RefreshTokens.Update(storedRefreshToken);
            await _dataContext.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);

            return await generateAuthResultAsync(user);
        }

        private ClaimsPrincipal getPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenParams, out SecurityToken validateToken);

                if (!isJwtValidSecurityAlgorithm(validateToken))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool isJwtValidSecurityAlgorithm(SecurityToken token)
        {
            return token is JwtSecurityToken securityToken && 
                securityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task<AuthenticationResult> generateAuthResultAsync(IdentityUser identity)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, identity.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, identity.Email),
                    new Claim("id", identity.Id),
                }),
                Expires = DateTime.UtcNow.AddSeconds(_jwtSettings.TokenLifeTime.Seconds),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = identity.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            await _dataContext.RefreshTokens.AddAsync(refreshToken);
            await _dataContext.SaveChangesAsync();

            var res = new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token.ToString()
            };

            return res;
        }
    }
}