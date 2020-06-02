using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

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
				Crashes.TrackError(ex);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                await _dialogService.ShowAlertAsync("Seems like there was a problem.", "Oops", "Close");
                return null;
            }
        }

        public async Task<Activity> LogCallAsync(int prospectId, Activity activity)
        {
            try
            {
                var authResult = await _authenticator.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);
                return await _activityRepository.LogCallAsync(prospectId, activity, authResult.AccessToken);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                await _dialogService.ShowAlertAsync("Seems like there was a problem.", "Oops", "Close");
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
                var activities = allActivities.Where(a => a.DateCompleted != null).OrderByDescending(a => a.UpdatedDate).ToList();
                return activities;
            } 

            catch (Exception ex)
            {
				Crashes.TrackError(ex);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                await _dialogService.ShowAlertAsync("Seems like there was a problem.", "Oops", "Close");
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
				Crashes.TrackError(ex);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                await _dialogService.ShowAlertAsync("Seems like there was a problem.", "Oops", "Close");
                return null;
            }
        }

        public async Task<List<Activity>> GetDailyToDoActivitiesAsync(string accessToken, List<UserDefinedCode> categories, int salespersonId, string category, DateTime dueAsOfDate, int page, int pageSize)
        {
            try
            {
                var activities = await _activityRepository.GetDailyToDoActivitiesAsync(accessToken, salespersonId, category, dueAsOfDate, page, pageSize);
                var a = activities?.Join(categories, a2 => a2.Prospect.Ranking, c => c.Code, (a2, c) =>
                {
                    a2.Prospect.Ranking = c.Description1;
                    return a2;

                }).ToList();
                return a;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                await _dialogService.ShowAlertAsync("Seems like there was a problem." + ex.Message, "Oops", "Close");
                return default(List<Activity>);
            }
        }

        public async Task<bool> UpdateActivityForProspectAsync(Activity activity)
        {
            try
            {
                var authResult = await _authenticator.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);
                return await _activityRepository.UpdateActivityForProspectAsync(activity, authResult.AccessToken);
            }

            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                await _dialogService.ShowAlertAsync("Seems like there was a problem.", "Oops", "Close");
                return false;
            }
        }

        public async Task<Activity> GetActivityWithTemplateDataAsync(int prospectId, string instanceId, string templateType)
        {
            try
            {
                var authResult = await _authenticator.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);
                return await _activityRepository.GetActivityWithTemplateDataAsync(prospectId, instanceId, templateType, authResult.AccessToken);
            }

            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                await _dialogService.ShowAlertAsync("Seems like there was a problem.", "Oops", "Close");
                return null;
            }
        }

        public async Task<Activity> SendAdHocSMSAsync(int prospectId, Activity activity)
        {
            try
            {
                TwilioClient.Init(Constants.PrivateKeys.TwilioAccountSid, Constants.PrivateKeys.TwilioAuthToken);

                var message = MessageResource.Create(
                            body: activity.Notes,
                            from: new Twilio.Types.PhoneNumber(activity.Prospect.ProspectCommunity.Community.SalesOffice.TwilioPhoneNumber),
                            to: new Twilio.Types.PhoneNumber(activity.Prospect.MobilePhoneNumber.Phone)
                        );
                if (message.ErrorCode == null)
                {
                    return await AddActivityToProspectAsync(prospectId, activity);
                }
                else
                {
                    await _dialogService.ShowAlertAsync(message.ErrorMessage, "Oops", "Close");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                await _dialogService.ShowAlertAsync("Seems like there was a problem.", "Oops", "Close");
                return null;
            }
        }

        public async Task<bool> SendSMSAsync(Activity activity)
        {
            try
            {
                TwilioClient.Init(Constants.PrivateKeys.TwilioAccountSid, Constants.PrivateKeys.TwilioAuthToken);

                var message = MessageResource.Create(
                            body: activity.Notes,
                            from: new Twilio.Types.PhoneNumber(activity.Prospect.ProspectCommunity.Community.SalesOffice.TwilioPhoneNumber),
                            to: new Twilio.Types.PhoneNumber(activity.Prospect.MobilePhoneNumber.Phone)
                        );
                if (message.ErrorCode == null)
                {
                    return await UpdateActivityForProspectAsync(activity);
                }
                else
                {
                    await _dialogService.ShowAlertAsync(message.ErrorMessage, "Oops", "Close");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                await _dialogService.ShowAlertAsync("Seems like there was a problem.", "Oops", "Close");
                return false;
            }
        }

        public async Task<List<SmsActivity>> GetSmsActivitiesAsync(string accessToken, int salespersonId, bool newOnly, int page, int pageSize)
        {
            try
            {
                var activities = await _activityRepository.GetSmsActivitiesAsync(accessToken, salespersonId, newOnly, page, pageSize);
                return activities;

                
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                await _dialogService.ShowAlertAsync("Seems like there was a problem." + ex.Message, "Oops", "Close");
                return default(List<SmsActivity>);
            }
        }

        public async Task<PhoneCallActivity> AddAdHocPhoneCallActivityAsync(int prospectId, PhoneCallActivity activity)
        {
            try
            {
                var authResult = await _authenticator.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);
                return await _activityRepository.AddAdHocPhoneCallActivityAsync(prospectId, activity, authResult.AccessToken);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                await _dialogService.ShowAlertAsync("Seems like there was a problem.", "Oops", "Close");
                return null;
            }
        }

        public async Task<bool> UpdatePhoneCallActivityForProspectAsync(PhoneCallActivity activity)
        {
            try
            {
                var authResult = await _authenticator.AuthenticateUser(Constants.PrivateKeys.ProspectMgmtRestResource);
                return await _activityRepository.UpdatePhoneCallActivityForProspectAsync(activity, authResult.AccessToken);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                await _dialogService.ShowAlertAsync("Seems like there was a problem.", "Oops", "Close");
                return false;
            }
        }
    }
}
