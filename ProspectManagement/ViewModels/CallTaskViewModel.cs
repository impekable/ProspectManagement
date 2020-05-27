using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.AppCenter.Analytics;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace ProspectManagement.Core.ViewModels
{
    public class CallTaskViewModel: BaseViewModel, IMvxViewModel<KeyValuePair<Activity, User>>
    {
        private IMvxCommand _closeCommand;

        private readonly IMvxNavigationService _navigationService;
        private readonly IActivityService _activityService;
        private IMvxCommand<String> _twilioPhoneCallCommand;

        public Activity Activity { get; set; }
        public User User { get; set; }

        private ObservableCollection<KeyValuePair<string, string>> _phones;
        private KeyValuePair<string, string> _selectedCall;

        public ObservableCollection<KeyValuePair<string, string>> Phones
        {
            get { return _phones; }
            set
            {
                _phones = value;
                RaisePropertyChanged(() => Phones);
            }
        }

        public KeyValuePair<string, string> SelectedCall
        {
            get { return _selectedCall; }
            set
            {
                _selectedCall = value;
                if (_selectedCall.Key.Equals("Mobile"))
                    if (User.UsingTelephony)
                        TwilioPhoneCallCommand.Execute(Activity.Prospect.MobilePhoneNumber.Phone);
                else if (_selectedCall.Key.Equals("Home"))
                    if (User.UsingTelephony)
                        TwilioPhoneCallCommand.Execute(Activity.Prospect.HomePhoneNumber.Phone);
                else if (_selectedCall.Key.Equals("Work"))
                    if (User.UsingTelephony)
                        TwilioPhoneCallCommand.Execute(Activity.Prospect.WorkPhoneNumber.Phone);
                RaisePropertyChanged(() => SelectedCall);
            }
        }

        public IMvxCommand<String> TwilioPhoneCallCommand
        {
            get
            {
                return _twilioPhoneCallCommand ?? (_twilioPhoneCallCommand = new MvxCommand<String>(async (param) =>
                {

                    TwilioClient.Init(Constants.PrivateKeys.TwilioAccountSid, Constants.PrivateKeys.TwilioAuthToken);

                    var call = CallResource.Create(
                        from: new Twilio.Types.PhoneNumber(Activity.Prospect.ProspectCommunity.Community.SalesOffice.TwilioPhoneNumber),
                        to: new Twilio.Types.PhoneNumber(User.MobilePhoneNumber.Phone),
                        machineDetection: "Enable",
                        url: new Uri($"https://optoutdv.khov.com/E1CRMWebApp/Call/Connect?phoneNumber={param}")
                    );

                    if (!string.IsNullOrEmpty(call.Sid))
                    {
                        var callActivity = new Activity()
                        {
                            ProspectAddressNumber = Activity.Prospect.ProspectAddressNumber,
                            SalespersonAddressNumber = User.AddressNumber,
                            ProspectCommunityId = Activity.Prospect.ProspectCommunity.ProspectCommunityId,
                            CallSid = call.Sid
                        };
                        var callActivityResult = await _activityService.LogCallAsync(Activity.Prospect.ProspectAddressNumber, callActivity);
                        if (!string.IsNullOrEmpty(callActivityResult.ActivityID))
                        {
                            var phoneCallActivity = new PhoneCallActivity()
                            {
                                ActivityId = Activity.ActivityID,
                                InstanceId = Activity.InstanceID,
                                CallPlanId = Activity.CallPlanId,
                                FromPhoneNumber = User.MobilePhoneNumber.Phone,
                                ToPhoneNumber = param,
                                PhoneCallActivityId = callActivityResult.ActivityID,
                                PhoneCallInstanceId = callActivityResult.InstanceID,
                                CallSid = call.Sid,
                                ProspectAddressBook = Activity.Prospect.ProspectAddressNumber,
                                ProspectCommunityId = Activity.Prospect.ProspectCommunity.ProspectCommunityId,
                                SalespersonAddressBook = User.AddressNumber,
                                CallTime = DateTime.UtcNow,
                                Community = Activity.Prospect.ProspectCommunity.CommunityNumber,
                                FollowUpTaskStartDate = Activity.TimeDateStart.Value,
                                FollowUpTaskEndDate = Activity.TimeDateEnd.Value,
                                FollowUpTaskSubject = Activity.Subject
                            };
                            await _navigationService.Close(this);
                            await _navigationService.Navigate<CallResultViewModel, PhoneCallActivity>(phoneCallActivity);
                        }
                    }
                }));
            }
        }

        public IMvxCommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new MvxCommand(async () =>
                {
                    await _navigationService.Close(this);
                }));
            }
        }

        public CallTaskViewModel(IMvxNavigationService navigationService, IActivityService activityService)
        {
            _navigationService = navigationService;
            _activityService = activityService;
        }

        public bool WorkPhoneEntered
        {
            get { return Activity.Prospect.WorkPhoneNumber != null && !String.IsNullOrEmpty(Activity.Prospect.WorkPhoneNumber.Phone); }
        }

        public bool MobilePhoneEntered
        {
            get { return Activity.Prospect.MobilePhoneNumber != null && !String.IsNullOrEmpty(Activity.Prospect.MobilePhoneNumber.Phone); }
        }

        public bool HomePhoneEntered
        {
            get { return Activity.Prospect.HomePhoneNumber != null && !String.IsNullOrEmpty(Activity.Prospect.HomePhoneNumber.Phone); }
        }

        public void Prepare(KeyValuePair<Activity, User> param)
        {
            User = param.Value;
            Activity = param.Key;
            Phones = new ObservableCollection<KeyValuePair<string, string>>();
            if (HomePhoneEntered)
                Phones.Add(new KeyValuePair<string, string>("Home", Activity.Prospect.HomePhoneNumber.Phone));
            if (MobilePhoneEntered)
                Phones.Add(new KeyValuePair<string, string>("Mobile", Activity.Prospect.MobilePhoneNumber.Phone));
            if (WorkPhoneEntered)
                Phones.Add(new KeyValuePair<string, string>("Work", Activity.Prospect.WorkPhoneNumber.Phone));
        }
    }
}
