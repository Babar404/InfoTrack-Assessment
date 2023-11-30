using System;
using System.Diagnostics;
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
    public class UpdateUserCommand : IRequest<UserDto>
    {
        public int Id { get; set; }
        public string GivenNames { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;

        public class Validator : AbstractValidator<UpdateUserCommand>
        {
            public Validator()
            {
                // TODO: Create validation rules for UpdateUserCommand so that all properties are required.
                RuleFor(x => x.Id)
                    .GreaterThan(0);

                RuleFor(x => x.GivenNames)
                    .NotEmpty();

                RuleFor(x => x.LastName)
                    .NotEmpty();

                RuleFor(x => x.EmailAddress)
                    .NotEmpty();

                RuleFor(x => x.MobileNumber)
                    .NotEmpty();
                // If you are feeling ambitious, also create a validation rule that ensures the user exists in the database.
            }
        }

        public class Handler : IRequestHandler<UpdateUserCommand, UserDto>
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
            public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
            {
                var stopwatch = Stopwatch.StartNew();

                User user = new User
                {
                    Id = request.Id,
                    GivenNames = request.GivenNames,
                    LastName = request.LastName,
                    ContactDetail = new ContactDetail
                    {
                        EmailAddress = request.EmailAddress,
                        MobileNumber = request.MobileNumber
                    }
                };

                User? userInDb = await _userService.GetAsync(request.Id, cancellationToken);
                if (userInDb is default(User)) throw new NotFoundException($"The user '{request.Id}' could not be found.");

                User addedUser = await _userService.UpdateAsync(user, cancellationToken);

                UserDto result = _mapper.Map<UserDto>(addedUser);

                stopwatch.Stop();
                _logger.LogInformation("Update User Command Handler execution took {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);

                return result;
            }
        }
    }
}
