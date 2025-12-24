using SafeDose.Domain.Entities;

namespace SafeDose.Application.Contracts.Persistence
{
    public interface IUserRepository : IGenericRepository<User, long>
    {
        Task UpdateProfileImageAsync(long userId, string imagePath);
    }
}
