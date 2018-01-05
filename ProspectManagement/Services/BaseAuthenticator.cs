﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using ProspectManagement.Core.Interfaces.Services;

namespace ProspectManagement.Core.Services
{
	public abstract class BaseAuthenticator: IAuthenticator
	{
		protected static string authority = "https://login.windows.net/khov.onmicrosoft.com/oauth2/token";
		protected static string returnUri = "http://ProspectManagementClient.azure-mobile.net";

		public abstract Task<AuthenticationResult> AuthenticateUser(string resource);

        public abstract void Logout();
	
	}
}
