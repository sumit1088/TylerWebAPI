namespace efilling_api.Services
{
    public interface IUserService
    {
        string GetEmailFromToken(string token);
        Task<string> GetPasswordHashByEmailAsync(string email);
    }

}
