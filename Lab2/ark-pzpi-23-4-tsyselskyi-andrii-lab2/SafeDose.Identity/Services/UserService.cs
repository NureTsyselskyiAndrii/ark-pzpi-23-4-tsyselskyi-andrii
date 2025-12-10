using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SafeDose.Application.Contracts.Identity;
using SafeDose.Application.Contracts.Persistence;
using SafeDose.Application.Exceptions;
using SafeDose.Application.Models.Identity.UserService;
using SafeDose.Identity.Models;
using System.Transactions;

namespace SafeDose.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AuthUser> _userManager;
        private readonly IUserRepository _userRepository;

        public UserService(UserManager<AuthUser> userManager, IUserRepository userRepository, IHttpContextAccessor contextAccessor)
        {
            _userManager = userManager;
            _userRepository = userRepository;
        }

        public async Task<long> CreateAsync(UserModel userModel)
        {
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var newAuthUser = new AuthUser()
                    {
                        UserName = userModel.UserName,
                        Email = userModel.Email,
                        PhoneNumber = userModel.PhoneNumber,
                        EmailConfirmed = userModel.EmailConfirmed,
                        PhoneNumberConfirmed = userModel.PhoneNumberConfirmed
                    };

                    var createResult = await _userManager.CreateAsync(newAuthUser);

                    if (!createResult.Succeeded)
                    {
                        throw new InternalServerException();
                    }

                    var newUser = new Domain.Entities.User()
                    {
                        Id = newAuthUser.Id,
                        Email = userModel.Email,
                        UserName = userModel.UserName,
                        FirstName = userModel.FirstName,
                        LastName = userModel.LastName,
                        BirthDate = userModel.BirthDate,
                        Biography = userModel.Biography,
                        PhoneNumber = userModel.PhoneNumber,
                        AvatarUrl = userModel.AvatarUrl
                    };

                    await _userRepository.CreateAsync(newUser);

                    scope.Complete();
                    return newAuthUser.Id;
                }
            }
            catch (Exception)
            {
                throw new InternalServerException();
            }

        }

        public async Task UpdateAsync(UserModel userModel, long userId)
        {
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var authUser = await _userManager.FindByIdAsync(userId.ToString());

                    if (authUser == null)
                    {
                        throw new NotFoundException("The user wasn't found.");
                    }


                    authUser.Email = userModel.Email;
                    authUser.UserName = userModel.UserName;
                    authUser.PhoneNumber = userModel.PhoneNumber;
                    authUser.EmailConfirmed = userModel.EmailConfirmed;
                    authUser.PhoneNumberConfirmed = userModel.PhoneNumberConfirmed;

                    var updateResult = await _userManager.UpdateAsync(authUser);

                    if (!updateResult.Succeeded)
                    {
                        throw new InternalServerException();
                    }

                    var businessUser = await _userRepository.GetByIdAsync(userId);

                    if (businessUser == null)
                    {
                        businessUser = new Domain.Entities.User()
                        {
                            Id = authUser.Id,
                            Email = userModel.Email,
                            UserName = userModel.UserName,
                            FirstName = userModel.FirstName,
                            LastName = userModel.LastName,
                            BirthDate = userModel.BirthDate,
                            Biography = userModel.Biography,
                            PhoneNumber = userModel.PhoneNumber,
                            AvatarUrl = userModel.AvatarUrl
                        };

                        await _userRepository.CreateAsync(businessUser);
                    }
                    else
                    {
                        businessUser.Email = userModel.Email;
                        businessUser.UserName = userModel.UserName;
                        businessUser.FirstName = userModel.FirstName;
                        businessUser.LastName = userModel.LastName;
                        businessUser.Biography = userModel.Biography;
                        businessUser.BirthDate = userModel.BirthDate;
                        businessUser.PhoneNumber = userModel.PhoneNumber;
                        businessUser.AvatarUrl = userModel.AvatarUrl;

                        await _userRepository.UpdateAsync(businessUser);
                    }

                    scope.Complete();
                }
            }
            catch (Exception)
            {
                throw new InternalServerException();
            }
        }
    }
}
