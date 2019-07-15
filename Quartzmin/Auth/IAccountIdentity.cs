using Quartzmin.Models;

namespace Quartzmin.Auth
{
    public interface IAccountIdentity
    {
        AccountModel Model { get; set; }

        bool LoginAccount();

        bool LoginGroups();

        bool CheckLogin();
    }
}