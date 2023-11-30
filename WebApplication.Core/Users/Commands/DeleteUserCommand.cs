﻿using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WebApplication.Core.Common.Exceptions;
using WebApplication.Core.Users.Common.Models;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Core.Users.Commands
{
    public class DeleteUserCommand : IRequest<UserDto>
    {
        public int Id { get; set; }

        public class Validator : AbstractValidator<DeleteUserCommand>
        {
            public Validator()
            {
                RuleFor(x => x.Id)
                    .GreaterThan(0);
            }
        }

        public class Handler : IRequestHandler<DeleteUserCommand, UserDto>
        {
            private readonly IUserService _userService;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;

            public Handler(IUserService userService, IMapper mapper, ILogger<Handler> logger)
            {
                _userService = userService;
                _mapper = mapper;
                _logger = logger;
            }

            /// <inheritdoc />
            public async Task<UserDto> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
            {
                var stopwatch = Stopwatch.StartNew();

                User? deletedUser = await _userService.DeleteAsync(request.Id, cancellationToken);

                if (deletedUser is default(User)) throw new NotFoundException($"The user '{request.Id}' could not be found.");

                stopwatch.Stop();
                _logger.LogInformation("Delete User Command Handler execution took {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);

                return _mapper.Map<UserDto>(deletedUser);
            }
        }
    }
}
