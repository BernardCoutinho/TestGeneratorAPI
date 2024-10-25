using TestGeneratorAPI.src.API.Base.Context;
using TestGeneratorAPI.src.API.Interface;
using TestGeneratorAPI.src.API.Model;
using Microsoft.EntityFrameworkCore;

namespace TestGeneratorAPI.src.API.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly PrincipalDbContext _context;

        public UserRepository(PrincipalDbContext context)
        {
            _context = context;
        }

        // Busca um usuário por username ou email
        public async Task<User> GetByUsernameOrEmailAsync(string username, string email)
        {
            return await _context.User.AsNoTracking().FirstOrDefaultAsync(u => u.Username.ToUpper() == username.ToUpper() || u.Email.ToUpper() == email.ToUpper());
        }

        public async Task<User> AddAsync(User entity)
        {
            _context.User.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(User entity)
        {
            _context.User.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var user = await GetByIdAsync(id);
            if (user is not null)
            {
                _context.User.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.User.AsNoTracking().ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.User.FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<User> UpdateAsync(User entity)
        {
            _context.User.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
