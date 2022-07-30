using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tweetbook.Cache;
using Tweetbook.Contracts.Contracts.V1.Requests;
using Tweetbook.Contracts.Contracts.V1.Responses;
using Tweetbook.Contracts.V1;
using Tweetbook.Contracts.V1.Requests;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Domain;
using Tweetbook.Extensions;
using Tweetbook.Helpers;
using Tweetbook.Services;

namespace Tweetbook.Controllers.V1
{
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;

        public PostsController(IPostService postService, IMapper mapper, IUriService uriService)
        {
            _postService = postService;
            _mapper = mapper;
            _uriService = uriService;
        }

        [HttpGet(ApiRoutes.Posts.GetAll)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQueryRequest paginationQuery)
        {
            var paginationFilter = _mapper.Map<PaginationQuery>(paginationQuery);
            var posts = await _postService.GetAllAsync(paginationFilter);

            var pagedResponse = PaginationHelpers
                .CreatePaginatedResponse(_uriService, paginationFilter, _mapper.Map<List<GetPostResponse>>(posts));

            return Ok(pagedResponse);
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        [Cache(60)]
        public async Task<IActionResult> GetById(Guid postId)
        {
            var post = await _postService.GetAsync(postId);
            return Ok(new Response<GetPostResponse>(_mapper.Map<GetPostResponse>(post)));
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
            var response = new Response<CreatePostResponse>(postResponse);

            return Created(_uriService.GetPostUri(postResponse.Id.ToString()), response);
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

                return Ok(new Response<UpdatePostSuccessResponse>(responsePost));
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