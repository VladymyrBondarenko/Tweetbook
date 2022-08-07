using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Tweetbook.Contracts.Contracts.V1.Responses;
using Tweetbook.Contracts.V1;
using Tweetbook.Contracts.V1.Requests;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Data;

namespace Tweetbook.IntegrationTests
{
    public class IntegrationTest : IDisposable
    {
        protected readonly HttpClient HttpClient;
        public readonly IServiceProvider _serviceProvider;

        protected IntegrationTest()
        {
            var factory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                    {
                        builder.ConfigureServices(services => 
                        {
                            services.RemoveAll(typeof(DbContextOptions<DataContext>));
                            services.AddDbContext<DataContext>(opt =>
                            {
                                opt.UseInMemoryDatabase("TestDb");
                            });
                        });
                    });
            HttpClient = factory.CreateClient();
            _serviceProvider = factory.Services;
        }

        protected async Task AuthenticateAsync()
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtToken());
        }

        protected async Task<Response<CreatePostResponse>> CreatePostAsync(CreatePostRequest request)
        {
            var response = await HttpClient.PostAsJsonAsync(ApiRoutes.Posts.Create, request);
            return await response.Content.ReadFromJsonAsync<Response<CreatePostResponse>>();
        }

        private async Task<string> GetJwtToken()
        {
            var response = await HttpClient.PostAsJsonAsync(
                ApiRoutes.Identity.Register, 
                new UserRegistrationRequest 
                { 
                    Email = "vladymyr.bondarenko1@gmail.com",
                    Password = "1777897Vova."
                });

            var authResponse = await response.Content.ReadFromJsonAsync<AuthSuccessResponse>();
            return authResponse?.Token;
        }

        public async void Dispose()
        {
            using var serviceScope = _serviceProvider.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<DataContext>();
            await context.Database.EnsureDeletedAsync();
        }
    }
}
