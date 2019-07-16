using Quartzmin.Models;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Quartzmin.Auth
{
    public class AccountIdentity : IAccountIdentity
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(String userName, String domain, String password,
                    int type, int provider, ref IntPtr token);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);
                
        private IntPtr _token = IntPtr.Zero;

        public AccountModel Model { get; set; }

        public AccountIdentity(string userName, string password, string domain, string groups)
        {
            Model = new AccountModel()
            {
                UserName = userName,
                Password = password,
                Groups = groups,
                Domain = domain
            };
        }

        public bool LoginAccount()
        {
            return LogonUser(Model.UserName, Model.Domain, Model.Password, 3, 0, ref _token);
        }

        public bool LoginGroups()
        {
            try
            {
                if (string.IsNullOrEmpty(Model.Groups))
                {
                    return true;
                }                

                var groups = SplitGroup(Model.Groups);

                return IsInRole(groups);
            }
            catch
            {
                return false;
            }
        }
      
        public bool CheckLogin()
        {
            try
            {
                var identity = new WindowsIdentity(_token);

                return identity != null;
            }
            catch
            {
                if (_token != IntPtr.Zero)
                {
                    CloseHandle(_token);
                }

                return false;
            }
        }

        private string [] SplitGroup(string groups)
        {
            return groups.Split(';');
        }

        private bool IsInRole(string [] groups)
        {
            var idUpn = new WindowsIdentity(Model.UserName + "@" + Model.Domain);

            var principalUpn = new WindowsPrincipal(idUpn);

            foreach (var group in groups)
            {
                if (principalUpn.IsInRole(Model.Domain + @"\" + group))
                {
                    return true;
                }
            }

            return false;
        }
    }
}