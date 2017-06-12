﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Services
{
    public class ProspectService : IProspectService
    {
        const string resource = "http://ProspectManagementRestService.azure-mobile.net";

        private readonly IAuthenticator _authenticator;
        private IDialogService _dialogService;

        private readonly IProspectRepository _prospectRepository;
        private readonly ICommunityRepository _communityRepository;

        public ProspectService(ICommunityRepository communityRepository, IProspectRepository prospectRepository, IAuthenticator authenticator, IDialogService dialogService)
        {
            _communityRepository = communityRepository;
            _prospectRepository = prospectRepository;
            _authenticator = authenticator;
            _dialogService = dialogService;
            _prospectRepository.RetrievingDataFailed += (sender, e) =>
            {
                _dialogService.ShowAlertAsync("Seems like there was a problem." + e.RetrievingDataFailureMessage, "Oops", "Close");
            };
        }

        public async Task<bool> AssignProspectToSalespersonAsync(string communityNumber, int prospectId, int salespersonId)
        {
            try
            {
                var authResult = await _authenticator.AuthenticateUser(resource);
                var result = await _prospectRepository.AssignProspectToSalespersonAsync(communityNumber, prospectId, salespersonId, authResult.AccessToken);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                _dialogService.ShowAlertAsync("Seems like there was a problem.", "Oops", "Close");
                return false;
            }
        }

        public async Task<Prospect> GetProspectAsync(int prospectId)
        {
            return await _prospectRepository.GetProspectAsync(prospectId);
        }

        public async Task<List<Prospect>> GetProspectsAsync(int salespersonId, bool unassigned, int page, int pageSize, string searchTerm)
        {
            try
            {
                var communities = await _communityRepository.GetCommunitiesBySalespersonAsync(salespersonId);
                var prospects = await _prospectRepository.GetProspectsAsync(unassigned ? 0 : (int?)null, communities, page, pageSize, searchTerm);
                var p = prospects.Join(communities, p2 => p2.ProspectCommunity.CommunityNumber, c => c.CommunityNumber, (p2, c) =>
                {
                    p2.ProspectCommunity.Community = c;
                    return p2;

                }).ToList();
                return p;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                _dialogService.ShowAlertAsync("Seems like there was a problem.", "Oops", "Close");
                return null;
            }
        }

        public async Task<bool> UpdateProspectAsync(Prospect prospect)
        {
            try
            {
                var authResult = await _authenticator.AuthenticateUser(resource);
                return await _prospectRepository.UpdateProspectAsync(prospect, authResult.AccessToken);
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
