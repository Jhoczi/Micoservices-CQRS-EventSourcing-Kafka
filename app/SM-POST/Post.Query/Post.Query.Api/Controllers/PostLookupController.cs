using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Common.DTOs;
using Post.Query.Api.DTOs;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PostLookupController : ControllerBase
{
    const string SAFE_ERR_MESSAGE = "Error while processing request to retrive all posts by author!";
    private readonly ILogger<PostLookupController> _logger;
    private readonly IQueryDispatcher<PostEntity> _queryDispatcher;
    
    public PostLookupController(ILogger<PostLookupController> logger, IQueryDispatcher<PostEntity> queryDispatcher)
    {
        _logger = logger;
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllPostsAsync()
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindAllPostsQuery());

            if (posts == null || !posts.Any())
            {
                return NoContent();
            }

            var count = posts.Count;
            return Ok(new PostLookupResponse()
            {
                Posts = posts,
                Message = $"Successful returned {count} post{(count > 1 ? "s" : string.Empty)}!",
            });
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    [HttpGet("byId/{postId}")]
    public async Task<ActionResult> GetPostByIdAsync(Guid postId)
    {
        try
        {
            var post = await _queryDispatcher.SendAsync(new FindPostByIdQuery() {Id = postId});

            if (post == null || !post.Any())
            {
                return NoContent();
            }
            
            return Ok(new PostLookupResponse()
            {
                Posts = post,
                Message = $"Successful returned post!",
            });
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    [HttpGet("byAuthor/{author}")]
    public async Task<ActionResult> GetPostsByAuthorAsync(string author)
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostByAuthor() {Author = author});

            if (posts == null || !posts.Any())
            {
                return NoContent();
            }

            var count = posts.Count;
            return Ok(new PostLookupResponse()
            {
                Posts = posts,
                Message = $"Successful returned {count} post{(count > 1 ? "s" : string.Empty)}!",
            });
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    [HttpGet("withComments")]
    public async Task<ActionResult> GetAllPostsWithCommentsAsync()
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostsWithCommentsQuery());

            if (posts == null || !posts.Any())
            {
                return NoContent();
            }

            var count = posts.Count;
            return Ok(new PostLookupResponse()
            {
                Posts = posts,
                Message = $"Successful returned {count} post{(count > 1 ? "s" : string.Empty)}!",
            });
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }
    
    [HttpGet("withLikes/{numberOfLikes}")]
    public async Task<ActionResult> GetAllPostsWithLikesAsync(int numberOfLikes)
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostsWithLikesQuery() {NumberOfLikes = numberOfLikes});

            if (posts == null || !posts.Any())
            {
                return NoContent();
            }

            var count = posts.Count;
            return Ok(new PostLookupResponse()
            {
                Posts = posts,
                Message = $"Successful returned {count} post{(count > 1 ? "s" : string.Empty)}!",
            });
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }
    
    private ActionResult ErrorResponse(Exception ex)
    {
        
        _logger.LogError(ex, SAFE_ERR_MESSAGE);
        return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse()
        {
            Message = SAFE_ERR_MESSAGE
        });
    }
}