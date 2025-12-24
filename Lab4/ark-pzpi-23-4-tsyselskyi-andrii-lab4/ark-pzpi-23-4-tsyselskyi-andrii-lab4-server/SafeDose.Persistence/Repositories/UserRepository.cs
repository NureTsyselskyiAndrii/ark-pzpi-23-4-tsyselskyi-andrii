using SafeDose.Application.Contracts.Persistence;
using SafeDose.Application.Exceptions;
using SafeDose.Domain.Entities;
using SafeDose.Persistence.DbContexts;

namespace SafeDose.Persistence.Repositories
{
    public class UserRepository : GenericRepository<User, long>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {

        }

        public async Task UpdateProfileImageAsync(long userId, string imagePath)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            user.AvatarUrl = imagePath;
            await _context.SaveChangesAsync();
        }

    }
}
