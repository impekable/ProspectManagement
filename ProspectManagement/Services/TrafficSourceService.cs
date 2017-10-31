﻿using ProspectManagement.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProspectManagement.Core.Models;
using ProspectManagement.Core.Interfaces.Repositories;

namespace ProspectManagement.Core.Services
{
    public class TrafficSourceService : ITrafficSourceService
    {
        private readonly IAuthenticator _authenticator;
        private IDialogService _dialogService;
        private readonly ITrafficSourceRepository _trafficSourceRepository;
        private readonly IDefaultCommunityRepository _defaultCommunityRepository;

        public TrafficSourceService(IDefaultCommunityRepository defaultCommunityRepository, ITrafficSourceRepository trafficSourceRepository, IAuthenticator authenticator, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _authenticator = authenticator;
            _defaultCommunityRepository = defaultCommunityRepository;
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
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                _dialogService.ShowAlertAsync("Seems like there was a problem." + ex.Message, "Oops", "Close");
                return default(List<TrafficSource>);
            }
        }

        public async Task<List<TrafficSource>> GetTrafficSourcesForDefaultCommunity()
        {
            try
            {
                var authResult = await _authenticator.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);
                var community = await _defaultCommunityRepository.GetDefaultCommunity();
                return await _trafficSourceRepository.GetTrafficSourcesByDivisionAsync(community.Division, authResult.AccessToken);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                _dialogService.ShowAlertAsync("Seems like there was a problem." + ex.Message, "Oops", "Close");
                return default(List<TrafficSource>);
            }
        }
    }
}
