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

        public async Task<User> GetByUsernameOrEmailAsync(string username, string email)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username.ToUpper() == username.ToUpper() || u.Email.ToUpper() == email.ToUpper());
        }

        public async Task<User> AddAsync(User entity)
        {
            _context.Users.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(User entity)
        {
            _context.Users.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var user = await GetByIdAsync(id);
            if (user is not null)
            {
                _context.Users.Remove(user);
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
            return await _context.Users.AsNoTracking().ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<User> UpdateAsync(User entity)
        {
            _context.Users.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
