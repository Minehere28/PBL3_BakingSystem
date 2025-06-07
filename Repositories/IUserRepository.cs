using PBL3.Entities;

namespace PBL3.Repositories
{
    public interface IUserRepository
    {
        User? GetUserBySdt(string sdt);
        public bool IsUserExists(string sdt);
        void Add(User user);
        void Update(User user);
        List<User> GetAllUser();
    }
}
