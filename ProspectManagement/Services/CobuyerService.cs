using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Services
{
    public class CobuyerService : ICobuyerService
    {
        private readonly IAuthenticator _authenticator;
        private IDialogService _dialogService;
        private readonly ICobuyerRepository _cobuyerRepository;

        public CobuyerService(ICobuyerRepository cobuyerRepository, IAuthenticator authenticator, IDialogService dialogService)
        {
            _cobuyerRepository = cobuyerRepository;
            _authenticator = authenticator;
            _dialogService = dialogService;
            _cobuyerRepository.RetrievingDataFailed += (sender, e) =>
            {
                _dialogService.ShowAlertAsync("Seems like there was a problem." + e.RetrievingDataFailureMessage, "Oops", "Close");
            };
        }

        public async Task<Cobuyer> AddCobuyerToProspectAsync(int prospectId, Cobuyer cobuyer)
        {
            try
            {
                var authResult = await _authenticator.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);
                return await _cobuyerRepository.AddCobuyerToProspectAsync(prospectId, cobuyer, authResult.AccessToken);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                _dialogService.ShowAlertAsync("Seems like there was a problem.", "Oops", "Close");
                return null;
            }
        }

        public async Task<Cobuyer> GetCobuyerAsync(int prospectId, int cobuyerId, string authToken)
        {
            try
            {
                return await _cobuyerRepository.GetCobuyerAsync(prospectId, cobuyerId, authToken);
            }

			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.ToString());
				_dialogService.ShowAlertAsync("Seems like there was a problem.", "Oops", "Close");
				return null;
			}

        }

        public async Task<List<Cobuyer>> GetCobuyersForProspectAsync(Prospect prospect)
        {
            try
            {
                var authResult = await _authenticator.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);
                var cobuyers = await _cobuyerRepository.GetCobuyersForProspectAsync(prospect.ProspectAddressNumber, authResult.AccessToken);
                foreach(Cobuyer cobuyer in cobuyers)
                {
                    cobuyer.Prospect = prospect;
                }
                return cobuyers; 

            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                _dialogService.ShowAlertAsync("Seems like there was a problem.", "Oops", "Close");
                return null;
            }
        }

        public async Task<bool> UpdateCobuyerAsync(Cobuyer cobuyer)
        {
            try
            {
                var authResult = await _authenticator.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);
                return await _cobuyerRepository.UpdateCobuyerAsync(cobuyer, authResult.AccessToken);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                _dialogService.ShowAlertAsync("Seems like there was a problem.", "Oops", "Close");
                return false;
            }

        }

        public async Task<bool> DeleteCobuyerFromProspectAsync(int cobuyerId)
        {
            try
            {
                var authResult = await _authenticator.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);
                return await _cobuyerRepository.DeleteCobuyerFromProspectAsync(cobuyerId, authResult.AccessToken);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                _dialogService.ShowAlertAsync("Seems like there was a problem.", "Oops", "Close");
                return false;
            }

        }
    }
}
