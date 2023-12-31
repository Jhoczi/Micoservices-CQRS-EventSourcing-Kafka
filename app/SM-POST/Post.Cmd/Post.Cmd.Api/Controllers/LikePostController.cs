﻿using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LikePostController : ControllerBase
{
    private ILogger<LikePostController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public LikePostController(ILogger<LikePostController> logger, ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> LikePostAsync(Guid id)
    {
        try
        {
            await _commandDispatcher.SendAsync(new LikePostCommand() { Id = id});

            return Ok(new BaseResponse()
            {
                Message = "Like post completed succesfully!",
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.Log(LogLevel.Warning, ex, "Client made a bad request!");
            return BadRequest(new BaseResponse()
            {
                Message = ex.Message,
            });
        }
        catch (AggregateNotFoundException ex)
        {
            _logger.Log(LogLevel.Warning, ex, "Could not retrieve an aggregate, client passed an incorrent post ID targeting the aggregate!");
            return BadRequest(new BaseResponse()
            {
                Message = ex.Message,
            });
        }
        catch (Exception ex)
        {
            const string SAFE_ERR_MESSAGE = "Error while processing the request to like new post.";
            _logger.Log(LogLevel.Error, ex, SAFE_ERR_MESSAGE);

            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse()
            {
                Message = SAFE_ERR_MESSAGE,
            });
        }
    }
}
