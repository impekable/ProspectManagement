using ProspectManagement.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProspectManagement.Core.Models;
using ProspectManagement.Core.Interfaces.Repositories;
using Microsoft.AppCenter.Crashes;

namespace ProspectManagement.Core.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly IAuthenticator _authenticator;
        private IDialogService _dialogService;

        public UserService(IAuthenticator authenticator, IUserRepository userRepository, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _userRepository = userRepository;
            _authenticator = authenticator;

            _userRepository.RetrievingDataFailed += async (sender, e) =>
            {
                if (e.RetrievingDataFailureMessage.Contains("403") || e.RetrievingDataFailureMessage.Contains("404"))
                {
                    await _dialogService.ShowAlertAsync("Could not get an E1 user for credentials entered. " + e.RetrievingDataFailureMessage, "Oops", "Close");
                    _authenticator.Logout();
                }
                else
                {
                    await _dialogService.ShowAlertAsync("Seems like there was a problem. " + e.RetrievingDataFailureMessage, "Oops", "Close");
                }
            };
        }

        public async Task<User> GetLoggedInUser()
        {
            try
            {
                var authResult = await _authenticator.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);
                var user = await _userRepository.GetUserByIdAsync(authResult.UserInfo.DisplayableId, authResult.AccessToken);
                //user.UsingTelephony = false;
                return user;
            }
            catch (Exception e)
            {
				Crashes.TrackError(e);
                await _dialogService.ShowAlertAsync("Seems like there was a problem." + e.Message, "Oops", "Close");
                return default(User);
            }
        }

        public async Task<User> GetUserById(string userId)
        {
            try
            {
                var authResult = await _authenticator.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);
                return await _userRepository.GetUserByIdAsync(userId, authResult.AccessToken);
            }
            catch (Exception e)
            {
				Crashes.TrackError(e);
                await _dialogService.ShowAlertAsync("Seems like there was a problem." + e.Message, "Oops", "Close");
                return default(User);
            }
        }
	}
}
