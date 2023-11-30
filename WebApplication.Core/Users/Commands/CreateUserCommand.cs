using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WebApplication.Core.Users.Common.Models;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Core.Users.Commands
{
    public class CreateUserCommand : IRequest<UserDto>
    {
        public string GivenNames { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;

        public class Validator : AbstractValidator<CreateUserCommand>
        {
            public Validator()
            {
                RuleFor(x => x.GivenNames)
                    .NotEmpty();

                RuleFor(x => x.LastName)
                    .NotEmpty();

                RuleFor(x => x.EmailAddress)
                    .NotEmpty();

                RuleFor(x => x.MobileNumber)
                    .NotEmpty();
            }
        }

        public class Handler : IRequestHandler<CreateUserCommand, UserDto>
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
            public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
            {
                var stopwatch = Stopwatch.StartNew();

                User user = new User
                {
                    GivenNames = request.GivenNames,
                    LastName = request.LastName,
                    ContactDetail = new ContactDetail
                    {
                        EmailAddress = request.EmailAddress,
                        MobileNumber = request.MobileNumber
                    }
                };

                User addedUser = await _userService.AddAsync(user, cancellationToken);
                UserDto result = _mapper.Map<UserDto>(addedUser);

                stopwatch.Stop();
                _logger.LogInformation("Create User Command Handler execution took {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);

                return result;
            }
        }
    }
}
