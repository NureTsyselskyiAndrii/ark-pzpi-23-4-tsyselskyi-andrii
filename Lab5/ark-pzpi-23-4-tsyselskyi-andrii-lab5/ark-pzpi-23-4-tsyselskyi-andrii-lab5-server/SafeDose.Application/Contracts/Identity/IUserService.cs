using SafeDose.Application.Models.Identity.UserService;

namespace SafeDose.Application.Contracts.Identity
{
    public interface IUserService
    {
        Task<long> CreateAsync(UserModel userModel);
        Task UpdateAsync(UserModel userModel, long userId);
    }
}
