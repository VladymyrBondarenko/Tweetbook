// See https://aka.ms/new-console-template for more information
using Refit;
using Tweetbook.Contracts.V1.Requests;
using Tweetbook.Sdk;

var cachedToken = string.Empty;

var identityApi = RestService.For<IIdentityApi>("https://localhost:7201");
var tweetBookApi = RestService.For<ITweetbookApi>("https://localhost:7201", new RefitSettings 
{
    AuthorizationHeaderValueGetter = () => Task.FromResult(cachedToken)
});

//var registerResponse = await identityApi.RegisterAsync(
//    new UserRegistrationRequest 
//    { 
//        Email = "vladymyr.bondarenko.sdktest@test.com",
//        Password = "1777897Vova."
//    });

var loginResponse = await identityApi.LoginAsync(
    new UserLoginRequest 
    {
        Email = "vladymyr.bondarenko.sdktest@test.com",
        Password = "1777897Vova."
    });

cachedToken = loginResponse?.Content?.Token;

var allPosts = await tweetBookApi.GetAllAsync();

var createdPost = await tweetBookApi.CreateAsync(new CreatePostRequest 
{
    Name = "Create post 1",
    Tags = new List<CreateTagRequest> 
    { 
        new CreateTagRequest 
        { 
            Name = "Post tag 1" 
        } 
    }
});

var retrievedPost = await tweetBookApi.GetAsync(createdPost.Content.Id);

var updateReponse = await tweetBookApi.UpdateAsync(
    new UpdatePostRequest { Id = retrievedPost.Content.Id, Name = "Create post 1 edited" });

var deleteResponse = await tweetBookApi.DeleteAsync(createdPost.Content.Id);
