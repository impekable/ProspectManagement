using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace ProspectManagement.Core.Interfaces.Services
{
	public interface IAuthenticator
	{
		Task<AuthenticationResult> AuthenticateUser(string resource);
		Task<AuthenticationResult> AuthenticateClient(string resource);
	}
}
