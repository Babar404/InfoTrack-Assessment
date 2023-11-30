using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using WebApplication.Core.Common.Models;
using WebApplication.Core.Users.Common.Models;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Core.Users.Queries
{
    public class ListUsersQuery : IRequest<PaginatedDto<IEnumerable<UserDto>>>
    {
        public int PageNumber { get; set; }
        public int ItemsPerPage { get; set; } = 10;

        public class Validator : AbstractValidator<ListUsersQuery>
        {
            public Validator()
            {
                // TODO: Create a validation rule so that PageNumber is always greater than 0
                RuleFor(x => x.PageNumber)
                    .GreaterThan(0);
            }
        }

        public class Handler : IRequestHandler<ListUsersQuery, PaginatedDto<IEnumerable<UserDto>>>
        {
            private readonly IUserService _userService;
            private readonly IMapper _mapper;

            public Handler(IUserService userService, IMapper mapper)
            {
                _userService = userService;
                _mapper = mapper;
            }
            /// <inheritdoc />
            public async Task<PaginatedDto<IEnumerable<UserDto>>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
            {
                IEnumerable<User> users = await _userService.GetPaginatedAsync
                    (request.PageNumber, request.ItemsPerPage, cancellationToken);

                Task<int> TotalCount = _userService.CountAsync();
                
                return new PaginatedDto<IEnumerable<UserDto>> 
                { Data = users.Select(u => _mapper.Map<UserDto>(u))
                , HasNextPage = (request.PageNumber * request.ItemsPerPage) < TotalCount.Result
                };
            }
        }
    }
}
