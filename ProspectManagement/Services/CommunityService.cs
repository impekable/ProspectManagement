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
    public class CommunityService : ICommunityService
    {
        private readonly ICommunityRepository _communityRepository;
        private readonly IAuthenticator _authenticator;
        private IDialogService _dialogService;

        public CommunityService(ICommunityRepository communityRepository, IAuthenticator authenticator, IDialogService dialogService)
        {
            _authenticator = authenticator;
            _communityRepository = communityRepository;
            _dialogService = dialogService;
        }

        public async Task<List<Community>> GetCommunitiesByDivision(string divisionCode, bool activeOnly)
        {
            try
            {
                var authResult = await _authenticator.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);
                return await _communityRepository.GetCommunitiesByDivisionAsync(authResult.AccessToken, divisionCode, activeOnly);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                _dialogService.ShowAlertAsync("Seems like there was a problem." + ex.Message, "Oops", "Close");
                return default(List<Community>);
            }
        }

        public async Task<List<Community>> GetCommunitiesBySalesperson(int salespersonId)
        {
            try
            {
                var authResult = await _authenticator.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);
                return await _communityRepository.GetCommunitiesBySalespersonAsync(salespersonId, authResult.AccessToken);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                _dialogService.ShowAlertAsync("Seems like there was a problem." + ex.Message, "Oops", "Close");
                return default(List<Community>);
            }
        }
    }
}
