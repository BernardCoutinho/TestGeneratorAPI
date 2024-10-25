using TestGeneratorAPI.src.API.Base;
using TestGeneratorAPI.src.API.Model;
using TestGeneratorAPI.src.API.View.User;

namespace TestGeneratorAPI.src.API.Interface
{
    public interface IUserService : IService<User, int>
    {
        Task<User> CreateUserAsync(UserRequest request);

        Task<User> GetByUsernameOrEmailAsync(string username, string email);
    }
}
