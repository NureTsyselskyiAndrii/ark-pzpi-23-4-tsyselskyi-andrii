namespace SafeDose.Application.Contracts.Identity;

public interface IUserContext
{
    long? GetApplicationUserId();
}