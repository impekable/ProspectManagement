using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using ProspectManagement.Core.Interfaces.Services;

namespace ProspectManagement.Core.Services
{
	public abstract class BaseAuthenticator: IAuthenticator
	{
		protected static string authority = "https://login.windows.net/khovmobileapptest.onmicrosoft.com/oauth2/token";
		protected static string nativeClientId = "1845270f-fa5e-4de7-9a49-ef1cc26e4c3a";
		protected static string returnUri = "http://ProspectRegistrationNativeClient.azure-mobile.net";
		static string restClientId = "87910174-0c14-4780-af58-c9834ef1c13b";
		static string clientSecret = "vdTTwPpHS9gVW0G5chnBotCXv9Mf+DK+3A2B+YMkUiw=";


		public async Task<AuthenticationResult> AuthenticateClient(string resource)
		{
			var authContext = new AuthenticationContext(authority);
			if (authContext.TokenCache.ReadItems().Any())
				authContext = new AuthenticationContext(authContext.TokenCache.ReadItems().First().Authority);

			var clientCredential = new ClientCredential(restClientId, clientSecret);
			var authResult = await authContext.AcquireTokenAsync(resource, clientCredential);
			return authResult;
		}

		public abstract Task<AuthenticationResult> AuthenticateUser(string resource);
		
	}
}
