using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tweetbook.Data;
using Tweetbook.Domain;
using Tweetbook.External.Contracts;
using Tweetbook.Options;
using Tweetbook.Services;
using Xunit;

namespace Tweetbook.UnitTests
{
    public class IdentityServiceTests
    {
        private readonly IIdentityService _identityService;
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<DataContext> _dataContextMock = new();
        private readonly Mock<IFacebookAuthService> _facebookAuthServiceMock = new();

        public IdentityServiceTests()
        {
            var jwtOptions = new JwtSettingsOptions
            {
                Secret = Guid.NewGuid().ToString(),
                TokenLifeTime = new TimeSpan(0, 0, 45)
            };

            var tokenParams = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.Secret)),
                ValidateIssuerSigningKey = true
            };

            var store = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);

            _identityService = new IdentityService(_userManagerMock.Object, jwtOptions, tokenParams, _dataContextMock.Object, _facebookAuthServiceMock.Object);
        }

        [Fact]
        public async Task LoginWithFacebookAsync_ShouldReturnAccessToken_WhenIdentityUserExists()
        {
            //Arrange
            var token = Guid.NewGuid().ToString();
            const string email = "vladymyr.bondarenko1@gmail.com";

            _facebookAuthServiceMock
                .Setup(x => x.ValidateAccessTokenAsync(token))
                .ReturnsAsync(new FacebookTokenValidationResult 
                { 
                    Data = new ValidationResultData
                    {
                        IsValid = true
                    }
                });

            _facebookAuthServiceMock
                .Setup(x => x.GetUserInfo(token))
                .ReturnsAsync(new FacebookUserInfoResult 
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = "Vladymyr",
                    LastName = "Bondarenko",
                    Email = email,
                    Name = "Vladymyr"
                });

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(new IdentityUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = email,
                    UserName = email
                });

            var refreshTokenMockSet = new Mock<DbSet<RefreshToken>>();
            _dataContextMock
                .Setup(x => x.RefreshTokens)
                .Returns(refreshTokenMockSet.Object);

            //Act
            var facebookAuthResult = await _identityService.LoginWithFacebookAsync(token);

            //Assert
            facebookAuthResult.Success.Should().BeTrue();
            facebookAuthResult.Token.Should().NotBeNullOrWhiteSpace();
            facebookAuthResult.RefreshToken.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task LoginWithFacebookAsync_ShouldReturnAccessToken_WhenIdentityUserDoesNotExist()
        {
            //Arrange
            var token = Guid.NewGuid().ToString();
            const string email = "vladymyr.bondarenko1@gmail.com";

            _facebookAuthServiceMock
                .Setup(x => x.ValidateAccessTokenAsync(token))
                .ReturnsAsync(new FacebookTokenValidationResult
                {
                    Data = new ValidationResultData
                    {
                        IsValid = true
                    }
                });

            _facebookAuthServiceMock
                .Setup(x => x.GetUserInfo(token))
                .ReturnsAsync(new FacebookUserInfoResult
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = "Vladymyr",
                    LastName = "Bondarenko",
                    Email = email,
                    Name = "Vladymyr"
                });

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(() => null);

            var identityRes = new IdentityResult();
            var propInfo = identityRes.GetType().GetProperty("Succeeded");
            propInfo.SetValue(identityRes, true, null);

            var identity = new IdentityUser
            {
                Email = email,
                UserName = email,
            };
            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(identityRes);

            var refreshTokenMockSet = new Mock<DbSet<RefreshToken>>();
            _dataContextMock
                .Setup(x => x.RefreshTokens)
                .Returns(refreshTokenMockSet.Object);

            //Act
            var facebookAuthResult = await _identityService.LoginWithFacebookAsync(token);

            //Assert
            facebookAuthResult.Success.Should().BeTrue();
            facebookAuthResult.Token.Should().NotBeNullOrWhiteSpace();
            facebookAuthResult.RefreshToken.Should().NotBeNullOrWhiteSpace();
        }
    }
}