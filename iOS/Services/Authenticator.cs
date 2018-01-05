using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using ProspectManagement.Core.Services;
using UIKit;

namespace ProspectManagement.iOS.Services
{
	public class Authenticator: BaseAuthenticator
	{

		public override async Task<AuthenticationResult> AuthenticateUser(string resource)
		{
            var controller = UIApplication.SharedApplication.KeyWindow.RootViewController;
			var _platformParams = new PlatformParameters(controller);

			var authContext = new AuthenticationContext(authority);
            if (authContext.TokenCache.ReadItems().Any())
            {
                if (authContext.TokenCache.ReadItems().First().Authority.Equals(authority))
                    authContext = new AuthenticationContext(authContext.TokenCache.ReadItems().First().Authority);
            }

			var uri = new Uri(returnUri);
			var authResult = await authContext.AcquireTokenAsync(resource, Constants.PrivateKeys.ProspectMgmtClientId, uri, _platformParams);
			return authResult;
		}

        public override async void Logout()
        {
            var authContext = new AuthenticationContext(authority);
            authContext.TokenCache.Clear();
            var cookieStorage = Foundation.NSHttpCookieStorage.SharedStorage;
            foreach (var cookie in cookieStorage.Cookies)
            {
                cookieStorage.DeleteCookie(cookie);
            }
        }
	}
}
