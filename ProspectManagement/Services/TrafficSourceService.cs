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
    public class TrafficSourceService : ITrafficSourceService
    {
        private readonly IAuthenticator _authenticator;
        private IDialogService _dialogService;
        private readonly ITrafficSourceRepository _trafficSourceRepository;

        public TrafficSourceService(ITrafficSourceRepository trafficSourceRepository, IAuthenticator authenticator, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _authenticator = authenticator;
            _trafficSourceRepository = trafficSourceRepository;
        }
        public async Task<TrafficSource> GetTrafficSourceDetails(int sourceId)
        {
            try
            {
                var authResult = await _authenticator.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);

                return await _trafficSourceRepository.GetTrafficSourceDetailsAsync(sourceId, authResult.AccessToken);
            }
            catch (Exception ex)
            {
				Crashes.TrackError(ex);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                _dialogService.ShowAlertAsync("Seems like there was a problem." + ex.Message, "Oops", "Close");
                return default(TrafficSource);
            }
        }

        public async Task<List<TrafficSource>> GetTrafficSourcesByDivision(string divisionCode)
        {
            try
            {
                var authResult = await _authenticator.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);

                return await _trafficSourceRepository.GetTrafficSourcesByDivisionAsync(divisionCode, authResult.AccessToken);
            }
            catch (Exception ex)
            {
				Crashes.TrackError(ex);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                _dialogService.ShowAlertAsync("Seems like there was a problem." + ex.Message, "Oops", "Close");
                return default(List<TrafficSource>);
            }
        }
    }
}
