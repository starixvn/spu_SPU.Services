using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Quartzmin.Models;
using System.Web;
using System.Configuration;

#region Target-Specific Directives
#if NETSTANDARD
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
#endif

#if NETFRAMEWORK
using System.IdentityModel.Tokens;
using System.Security.Principal;
using System.Linq;
#endif
#endregion

namespace Quartzmin.Auth
{
    public class AuthenticationHandler : DelegatingHandler
    {
        private const string SCHEME = "Basic";
        private List<IAccountIdentity> _listAccountIndentity = new List<IAccountIdentity>();

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var headers = request.Headers;                

                var accountIdentity = GetAccountIdentity(headers);

                if (accountIdentity != null)
                {
                    if (accountIdentity.CheckLogin())
                    {
#if NETFRAMEWORK
                        var principal = GetPrincipal(accountIdentity.Model.UserName);

                        SetPrincipal(request, principal);
#endif
                    }
                    else
                    {
                        Login(request, accountIdentity);
                    }
                }

                var response = await base.SendAsync(request, cancellationToken).ContinueWith(t =>
                {
                    var result = t.Result;
                    result.Headers.ConnectionClose = false;

                    return result;
                });

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(SCHEME));
                }

                AddAccountIdentity(accountIdentity);

                return response;
            }
            catch (Exception)
            {
#if NETFRAMEWORK
                var response = request.CreateResponse(HttpStatusCode.Unauthorized);
                response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(SCHEME));

                return response;
#endif
            }

            return null;
        }

        private IAccountIdentity GetAccountIdentity(HttpRequestHeaders requestHeader)
        {
            if (requestHeader.Authorization != null && SCHEME.Equals(requestHeader.Authorization.Scheme))
            {
                Encoding encoding = Encoding.GetEncoding("iso-8859-1");

                string credentials = encoding.GetString(Convert.FromBase64String(requestHeader.Authorization.Parameter));

                string[] parts = credentials.Split(':');

                if (parts.Length >= 2)
                {
                    string userName = parts[0].Trim();
                    string password = parts[1].Trim();

                    string domain = ConfigurationManager.AppSettings["Auth-Domain"];
                    string groups = ConfigurationManager.AppSettings["Auth-Groups"];

                    var account = _listAccountIndentity.Find(c => c.Model.UserName == userName && c.Model.Password == password);

                    if (account != null)
                    {
                        return account;
                    }

                    return new AccountIdentity(userName, password, domain, groups);
                }
            }

            return null;
        }

        private bool Login(HttpRequestMessage request, IAccountIdentity accountIdentity)
        {
            if (accountIdentity == null)
            {
                return false;
            }

            var isAccess = accountIdentity.LoginAccount() && accountIdentity.LoginGroups();                       

            if (isAccess)
            {
#if NETFRAMEWORK
                 var principal = GetPrincipal(accountIdentity.Model.UserName);

                 SetPrincipal(request, principal);                
#endif
            }

            return isAccess;
        }

#if NETFRAMEWORK
        private IPrincipal GetPrincipal(string userName)
        {
            var claims = new List<Claim>
                {
                        new Claim(ClaimTypes.Name, userName),
                        new Claim(ClaimTypes.AuthenticationMethod, AuthenticationMethods.Password)
                };

            var principal = new ClaimsPrincipal(
                                                new[] { new ClaimsIdentity(claims, SCHEME) });

            return principal;
        }

        private void SetPrincipal(HttpRequestMessage request, IPrincipal principal)
        {
            var context = request.GetOwinContext();
                
            if (context.Authentication != null)
	        {
                context.Authentication.User = (ClaimsPrincipal)principal;
	        }
        }
#endif

        private void AddAccountIdentity(IAccountIdentity account)
        {
            if (account != null && !_listAccountIndentity.Contains(account))
            {
                _listAccountIndentity.Add(account);
            }
        }

    }
}