using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tweetbook.Cache;
using Tweetbook.Contracts.V1;
using Tweetbook.Contracts.V1.Requests;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Domain;
using Tweetbook.Extensions;
using Tweetbook.Services;

namespace Tweetbook.Controllers.V1
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;

        public PostsController(IPostService postService, IPostTagService postTagService,
            IMapper mapper)
        {
            _postService = postService;
            _mapper = mapper;
        }

        [HttpGet(ApiRoutes.Posts.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _postService.GetAllAsync();
            return Ok(_mapper.Map<List<GetPostResponse>>(posts));
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        [Cache(60)]
        public async Task<IActionResult> GetById(Guid postId)
        {
            var post = await _postService.GetAsync(postId);
            return Ok(_mapper.Map<GetPostResponse>(post));
        }

        /// <summary>
        /// Returns all the tags in the system
        /// </summary>
        /// <response code="201">Creates post in the system</response>
        /// <response code="400">Creation of post failed</response>
        [HttpPost(ApiRoutes.Posts.Create)]
        [ProducesResponseType(typeof(CreatePostResponse), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Create([FromBody] CreatePostRequest postRequest)
        {
            var post = await _postService.CreateAsync(new Post
            {
                Name = postRequest.Name,
                UserId = HttpContext.GetUserId(),
                Tags = postRequest.Tags
                    .Select(x => new Tag { Name = x.Name, CreatorId = HttpContext.GetUserId() })
                    .ToList()
            });

            if(post == null)
            {
                return BadRequest(new ErrorResponse 
                { 
                    Errors = new List<ErrorModel> 
                    { 
                        new ErrorModel 
                        { 
                            Message = "The post was not created." 
                        } 
                    } 
                });
            }

            var postResponse = _mapper.Map<CreatePostResponse>(post);

            return CreatedAtAction(nameof(GetById), postResponse);
        }

        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Edit([FromBody] UpdatePostRequest postRequest)
        {
            var userOwnsPost = await _postService.UserOwnsPostAsync(postRequest.Id, HttpContext.GetUserId());

            if (!userOwnsPost)
            {
                var response = new UpdatePostFailedResponse 
                { 
                    Errors = new [] { "You do not own this post." }
                };
                return BadRequest(response);
            }

            var post = await _postService.GetAsync(postRequest.Id);
            post.Name = postRequest.Name;

            if (await _postService.UpdateAsync(post))
            {
                var responsePost = _mapper.Map<UpdatePostSuccessResponse>(post);

                return Ok(responsePost);
            }

            return NotFound();
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> Delete(Guid postId)
        {
            if (await _postService.DeleteAsync(postId))
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}