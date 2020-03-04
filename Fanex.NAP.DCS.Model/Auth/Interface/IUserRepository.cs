using System.Threading.Tasks;

namespace Fanex.NAP.DCS.Model.Auth
{
    public interface IUserRepository
    {
        Task<UserModel> GetAsync(string email);
        Task AddAsync(UserModel user);
    }
}