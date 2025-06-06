using PBL3.Entities;
using PBL3.Models;

namespace PBL3.Services
{
    public interface IUserService
    {
        User? GetUserBySdt(string sdt);
        public bool IsUserExists(string sdt);
        public void RegisterUser(User user);
        public User? Authenticate(string sdt, string password);
        public bool ChangePassword(string sdt, string newPassword);
    }
}
