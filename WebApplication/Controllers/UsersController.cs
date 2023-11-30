using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Logging;
using WebApplication.Core.Common.Models;
using WebApplication.Core.Users.Commands;
using WebApplication.Core.Users.Common.Models;
using WebApplication.Core.Users.Queries;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IMediator mediator, ILogger<UsersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserAsync(
            [FromQuery] GetUserQuery query,
            CancellationToken cancellationToken)
        {
            try
            {
                UserDto result = await _mediator.Send(query, cancellationToken);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while processing the request: {ex.Message}");
                throw;
            }
        }

        // TODO: create a route (at /Find) that can retrieve a list of matching users using the `FindUsersQuery`
        [HttpGet("Find")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> FindUsersAsync
            ([FromQuery] FindUsersQuery query
            , CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<UserDto> result = await _mediator.Send(query, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while processing the request: {ex.Message}");
                throw;
            }
        }

        // TODO: create a route (at /List) that can retrieve a paginated list of users using the `ListUsersQuery`
        [HttpGet("List")]
        [ProducesResponseType(typeof(PaginatedDto<IEnumerable<UserDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListUsersAsync
            ([FromQuery] ListUsersQuery query
            , CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(query, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while processing the request: {ex.Message}");
                throw;
            }
        }

        // TODO: create a route that can create a user using the `CreateUserCommand`
        [HttpPost]
        [ProducesResponseType(typeof(IRequest<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateUserAsync
            ([FromBody] CreateUserCommand query
            , CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(query, cancellationToken);
                var createdUser = result.UserId;

                var locationUrl = $"/Users?id={createdUser}";
                Response.Headers.Add("Location", locationUrl);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while processing the request: {ex.Message}");
                throw;
            }
        }

        // TODO: create a route that can update an existing user using the `UpdateUserCommand`
        [HttpPut]
        [ProducesResponseType(typeof(IRequest<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserAsync
            ([FromBody] UpdateUserCommand query
            , CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(query, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while processing the request: {ex.Message}");
                throw;
            }
        }

        // TODO: create a route that can delete an existing user using the `DeleteUserCommand`
        [HttpDelete]
        [ProducesResponseType(typeof(IRequest<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUserAsync
            ([FromQuery] DeleteUserCommand query
            , CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(query, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while processing the request: {ex.Message}");
                throw;
            }
        }
    }
}
