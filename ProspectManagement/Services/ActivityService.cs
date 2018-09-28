using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Services
{
    public class ActivityService: IActivityService
    {
        private readonly IAuthenticator _authenticator;
        private IDialogService _dialogService;
        private readonly IActivityRepository _activityRepository;

        public ActivityService(IActivityRepository activityRepository, IAuthenticator authenticator, IDialogService dialogService)
        {
            _activityRepository = activityRepository;
            _authenticator = authenticator;
            _dialogService = dialogService;
            _activityRepository.RetrievingDataFailed += (sender, e) =>
            {
                _dialogService.ShowAlertAsync("Seems like there was a problem." + e.RetrievingDataFailureMessage, "Oops", "Close");
            };
        }

        public async Task<Activity> AddActivityToProspectAsync(int prospectId, Activity activity)
        {
            try
            {
                var authResult = await _authenticator.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);
                return await _activityRepository.AddProspectActivityAsync(prospectId, activity, authResult.AccessToken);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                _dialogService.ShowAlertAsync("Seems like there was a problem.", "Oops", "Close");
                return null;
            }
        }

        public async Task<Activity> AddNoteToProspectAsync(Prospect prospect, String note)
        {
            Activity noteActivity = new Activity()
            {
                ActivityType = "Note",
                DateCompleted = DateTime.UtcNow,
                ProspectCommunityId = prospect.ProspectCommunity.ProspectCommunityId,
                ProspectAddressNumber = prospect.ProspectAddressNumber,
                SalespersonAddressNumber = prospect.ProspectCommunity.SalespersonAddressNumber,
                Notes = note         
            };
            return await AddActivityToProspectAsync(prospect.ProspectAddressNumber, noteActivity);
        }

        public async Task<Activity> AddVisitToProspectAsync(Prospect prospect, String note)
        {
            Activity visitActivity = new Activity()
            {
                ActivityType = "In-Person",
                DateCompleted = DateTime.UtcNow,
                ProspectCommunityId = prospect.ProspectCommunity.ProspectCommunityId,
                ProspectAddressNumber = prospect.ProspectAddressNumber,
                SalespersonAddressNumber = prospect.ProspectCommunity.SalespersonAddressNumber,
                Notes = note
            };
            return await AddActivityToProspectAsync(prospect.ProspectAddressNumber, visitActivity);
        }

        public async Task<List<Activity>> GetActivitiesForProspectAsync(int prospectId)
        {
            try
            {
                var authResult = await _authenticator.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);
                var allActivities = await _activityRepository.GetProspectActivitiesAsync(prospectId, EmailFormat.None, authResult.AccessToken);
                var activities = allActivities.Where(a => a.DateCompleted != null).OrderBy(a => a.DateCompleted).ToList();
                return activities;
            } 

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                _dialogService.ShowAlertAsync("Seems like there was a problem.", "Oops", "Close");
                return null;
            }
        }

        public async Task<Activity> GetActivityForProspectAsync(int prospectId, string instanceId)
        {
            try
            {
                var authResult = await _authenticator.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);
                return await _activityRepository.GetProspectActivityAsync(prospectId, instanceId, EmailFormat.HTML, authResult.AccessToken);
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                _dialogService.ShowAlertAsync("Seems like there was a problem.", "Oops", "Close");
                return null;
            }
        }
    }
}
