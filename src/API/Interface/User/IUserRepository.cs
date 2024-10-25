

using TestGeneratorAPI.src.API.Base;
using TestGeneratorAPI.src.API.Model;

namespace TestGeneratorAPI.src.API.Interface
{
    public interface IUserRepository : IRepository<User, int>
    {
        Task<User> GetByUsernameOrEmailAsync(string username, string email);
    }
}
