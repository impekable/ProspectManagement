using ProspectManagement.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProspectManagement.Core.Models;
using ProspectManagement.Core.Interfaces.Repositories;

namespace ProspectManagement.Core.Services
{
	public class UserService : IUserService
	{
		const string resource = "1845270f-fa5e-4de7-9a49-ef1cc26e4c3a";
		private readonly IUserRepository _userRepository;
		private readonly IAuthenticator _authenticator;

		public UserService(IAuthenticator authenticator, IUserRepository userRepository)
		{
			_userRepository = userRepository;
			_authenticator = authenticator;
		}

		public async Task<User> GetLoggedInUser()
		{
			try
			{
				var authResult = await _authenticator.AuthenticateUser(resource);
				return await _userRepository.GetUserByIdAsync(authResult.UserInfo.DisplayableId);
			}
			catch (Exception e)
			{
				return default(User);
			}
		}

		public async Task<User> GetUserById(string userId)
		{
			return await _userRepository.GetUserByIdAsync(userId);
		}
	}
}
