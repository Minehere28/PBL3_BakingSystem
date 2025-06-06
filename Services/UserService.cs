using PBL3.Entities;
using PBL3.Models;
using PBL3.Repositories;

namespace PBL3.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public bool IsUserExists(string sdt)
        {
            return _userRepo.IsUserExists(sdt);
        }
        public void RegisterUser(User user)
        {
            _userRepo.Add(user);
        }
        public User? Authenticate(string sdt, string password)
        {
            var user = GetUserBySdt(sdt);
            if (user == null) return null;

            if (user.Role == "Admin" && user.PasswordHash.Trim() == password.Trim())
                return user;

            if (user.Role == "Customer" && user.VerifyPassword(password))
                return user;

            return null;

        }

        public User? GetUserBySdt(string sdt)
        {
            return _userRepo.GetUserBySdt(sdt);
        }

        public bool ChangePassword(string sdt, string newPassword)
        {
            var user = GetUserBySdt(sdt);
            if (user == null) return false;
            user.SetPassword(newPassword);
            _userRepo.Update(user);
            return true;
        }
    }
}
