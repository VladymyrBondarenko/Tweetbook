using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Tweetbook.Services;
using Tweetbook.Contracts.V1;
using AutoMapper;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Cache;

namespace Tweetbook.Controllers.V1
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TagsController : ControllerBase
    {
        private readonly IPostTagService _postTagService;
        private readonly IMapper _mapper;

        public TagsController(IPostTagService postTagService, IMapper mapper)
        {
            _postTagService = postTagService;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all the tags in the system
        /// </summary>
        /// <response code="200">Returns all the tags in the system</response>
        [HttpGet(ApiRoutes.Tags.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var tags = await _postTagService.GetAllAsync();
            return Ok(_mapper.Map<List<GetTagResponse>>(tags));
        }

        /// <summary>
        /// Returns the tag by id in the system
        /// </summary>
        /// <response code="200">Returns the tag by id in the system</response>
        [HttpGet(ApiRoutes.Tags.Get)]
        [Cache(60)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var tag = await _postTagService.GetAsync(id);
            return Ok(_mapper.Map<GetTagResponse>(tag));
        }
    }
}
