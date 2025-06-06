using Microsoft.EntityFrameworkCore;
using PBL3.Data;
using PBL3.Entities;

namespace PBL3.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly BMContext _context;
        public UserRepository(BMContext context)
        {
            _context = context;
        }
        public  User? GetUserBySdt(string sdt)
        {
            return _context.Users.FirstOrDefault(u => u.Sdt == sdt);
        }
        public void Add(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        public void Update(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            _context.Users.Update(user);
            _context.SaveChanges();
        }
        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public bool IsUserExists(string sdt)
        {
            return _context.Users.Any(u => u.Sdt == sdt);
        }
    }
}
