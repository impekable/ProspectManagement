using System;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Services
{
    public class BuyerDecisionsService : IBuyerDecisionsService
    {
        private readonly IAuthenticator _authenticator;
        private IDialogService _dialogService;
        private readonly IBuyerDecisionsRepository _buyerDecisionsRepository;

        public BuyerDecisionsService(IBuyerDecisionsRepository buyerDecisionsRepository, IAuthenticator authenticator, IDialogService dialogService)
        {
            _buyerDecisionsRepository = buyerDecisionsRepository;
            _authenticator = authenticator;
            _dialogService = dialogService;
            _buyerDecisionsRepository.RetrievingDataFailed += (sender, e) =>
            {
                _dialogService.ShowAlertAsync("Seems like there was a problem." + e.RetrievingDataFailureMessage, "Oops", "Close");
            };
        }

        public async Task<BuyerDecisions> GetBuyerDecisionsAsync(int prospectId)
        {
            try
            {
                var authResult = await _authenticator.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);
                var decisions = await _buyerDecisionsRepository.GetBuyerDecisionsAsync(prospectId, authResult.AccessToken);

                return decisions ?? new BuyerDecisions { ProspectAddressNumber =  prospectId };
            }

            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                _dialogService.ShowAlertAsync("Seems like there was a problem.", "Oops", "Close");
                return null;
            }
        }

        public async Task<bool> UpdateBuyerDecisionsAsync(BuyerDecisions decisions)
        {
            try
            {
                var authResult = await _authenticator.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);
                return await _buyerDecisionsRepository.UpdateBuyerDecisionsAsync(decisions, authResult.AccessToken);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                _dialogService.ShowAlertAsync("Seems like there was a problem.", "Oops", "Close");
                return false;
            }
        }
    }
}
