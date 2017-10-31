using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Services
{
    public class TrafficCardResponseService : ITrafficCardResponseService
    {
        private readonly ITrafficCardResponseRepository _responseRepository;
        private readonly IAuthenticator _authenticator;
        private IDialogService _dialogService;

        public TrafficCardResponseService(IAuthenticator authenticator, ITrafficCardResponseRepository responseRepository, IDialogService dialogService)
        {
            _responseRepository = responseRepository;
            _authenticator = authenticator;
            _dialogService = dialogService;
            _responseRepository.RetrievingDataFailed += (sender, e) =>
            {
                _dialogService.ShowAlertAsync("Seems like there was a problem. " + e.RetrievingDataFailureMessage, "Oops", "Close");
            };
        }

        public async Task<bool> UpdateTrafficCardResponse(int prospectAddressNumber, List<TrafficCardResponse> response)
        {
            try
            {
                var authResult = await _authenticator.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);
                return await _responseRepository.UpdateTrafficCardResponseAsync(prospectAddressNumber, response, authResult.AccessToken);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                _dialogService.ShowAlertAsync("Seems like there was a problem." + ex.Message, "Oops", "Close");
                return false;
            }
        }

        public async Task<List<TrafficCardResponse>> GetTrafficCardResponsesForProspect(int prospectAddressNumber, bool requiredOnly)
        {
            try
            {
                var authResult = await _authenticator.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);
                return await _responseRepository.GetTrafficCardResponsesForProspectAsync(prospectAddressNumber, requiredOnly, authResult.AccessToken);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                _dialogService.ShowAlertAsync("Seems like there was a problem." + ex.Message, "Oops", "Close");
                return default(List<TrafficCardResponse>);
            }
        }
    }
}
