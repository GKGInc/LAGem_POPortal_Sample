using LAGem_POPortal.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LAGem_POPortal.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        // ----------------------------------------------------------------------------------

        #region Variables

        private readonly ProtectedSessionStorage _sessionStorage;
        public ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        private Task<AuthenticationState> _authenticationStateTask;

        #endregion

        // ----------------------------------------------------------------------------------

        #region Public Functions

        public CustomAuthenticationStateProvider(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }
        
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var userSessionStorageResult = await _sessionStorage.GetAsync<UserSession>("UserSession");
                var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;

                if (userSession == null)
                    return await Task.FromResult(new AuthenticationState(_anonymous));

                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userSession.UserName),
                    new Claim("password", userSession.Password),
                    new Claim(ClaimTypes.Role, userSession.Role),
                    new Claim("PromoCode", userSession.PromoCode),
                    new Claim("Oid", userSession.Oid.ToString())

                }, "CustomAuth"));
                //return await Task.FromResult(new AuthenticationState(claimsPrincipal));

                Task<AuthenticationState> result = Task.FromResult(new AuthenticationState(claimsPrincipal));
                SetAuthenticationState(result);

                return await result;
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }
        }

        public void SetAuthenticationState(Task<AuthenticationState> authenticationStateTask)
        {
            _authenticationStateTask = authenticationStateTask ?? throw new ArgumentNullException(nameof(authenticationStateTask));
            NotifyAuthenticationStateChanged(_authenticationStateTask);
        }

        public async Task UpdateAuthenticationState(UserSession userSession)
        {
            ClaimsPrincipal claimsPrincipal;

            if (userSession != null)
            {
                await _sessionStorage.SetAsync("UserSession", userSession);

                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userSession.UserName),
                    new Claim("password", userSession.Password),
                    new Claim(ClaimTypes.Role, userSession.Role),
                    new Claim("PromoCode", userSession.PromoCode)

                }));
            }
            else
            {
                await _sessionStorage.DeleteAsync("UserSession");
                claimsPrincipal = _anonymous;
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        #endregion

        // ----------------------------------------------------------------------------------
    }
}
