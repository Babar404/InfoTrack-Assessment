using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication.Infrastructure.Contexts;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly InMemoryContext _dbContext;

        public UserService(InMemoryContext dbContext)
        {
            _dbContext = dbContext;

            // this is a hack to seed data into the in memory database. Do not use this in production.
            _dbContext.Database.EnsureCreated();
        }

        /// <inheritdoc />
        public async Task<User?> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            User? user = await _dbContext.Users.Where(user => user.Id == id)
                                         .Include(x => x.ContactDetail)
                                         .FirstOrDefaultAsync(cancellationToken);

            return user;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<User>> FindAsync(string? givenNames, string? lastName, CancellationToken cancellationToken = default)
        {
            IQueryable<User> query = from u in _dbContext.Users select u;
            
            if (!string.IsNullOrWhiteSpace(givenNames))
                query = query.Where(u => u.GivenNames.IndexOf(givenNames, StringComparison.OrdinalIgnoreCase) != -1);

            if (!string.IsNullOrWhiteSpace(lastName))
                query = query.Where(u => u.LastName.ToLower().Contains(lastName.ToLower()));

            IEnumerable<User>? results = await query.Include(u => u.ContactDetail).ToListAsync(cancellationToken);

            return results;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<User>> GetPaginatedAsync(int page, int count, CancellationToken cancellationToken = default)
        {
            IEnumerable<User> user = await _dbContext.Users
                                         .Skip((page - 1) * count).Take(count)
                                         .Include(x => x.ContactDetail)
                                         .ToListAsync(cancellationToken);
            return user;
        }

        /// <inheritdoc />
        public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return user;
        }

        /// <inheritdoc />
        public async Task<User?> UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            User existingUser = await _dbContext.Users.Where(u => u.Id == user.Id)
                                        .Include(x => x.ContactDetail)
                                        .FirstOrDefaultAsync(cancellationToken);
            if (existingUser != null)
            {
                existingUser.GivenNames = user.GivenNames;
                existingUser.LastName = user.LastName;
                existingUser.ContactDetail = user.ContactDetail;

                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            
            return existingUser;
        }

        /// <inheritdoc />
        public async Task<User?> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            User existingUser = await _dbContext.Users.Where(u => u.Id == id)
                                        .Include(x => x.ContactDetail)
                                        .FirstOrDefaultAsync(cancellationToken);
            if (existingUser != null)
            {
                _dbContext.Entry(existingUser).State = EntityState.Deleted;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return existingUser;
        }

        /// <inheritdoc />
        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return (await _dbContext.Users.ToListAsync(cancellationToken)).Count();
        }
    }
}
