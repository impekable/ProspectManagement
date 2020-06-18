using System;
using System.Collections.Generic;
using System.Windows.Input;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Messages;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class DailyToDoTaskViewModel : BaseViewModel, IMvxViewModel<User>
    {
        private MvxInteraction<String> _showAlertInteraction = new MvxInteraction<String>();
        public IMvxInteraction<String> ShowAlertInteraction => _showAlertInteraction;

        private MvxInteraction _hideAlertInteraction = new MvxInteraction();
        public IMvxInteraction HideAlertInteraction => _hideAlertInteraction;

        private IMvxCommand _dismissCommand;
        private IMvxCommand _smsCommand;
        private IMvxCommand _phoneCallCommand;
        private IMvxCommand _composeEmailCommand;

        public IMvxMessenger Messenger { get; }

        private readonly IMvxNavigationService _navigationService;
        private readonly IActivityService _activityService;

        public Activity Activity { get; set; }
        public User User { get; set; }

        public bool AllowTexting
        {
            get { return Activity.Prospect.FollowUpSettings.ConsentToText; }
        }

        public bool AllowCalling
        {
            get { return Activity.Prospect.FollowUpSettings.ConsentToPhone; }
        }

        public bool AllowEmailing
        {
            get { return Activity.Prospect.FollowUpSettings.ConsentToEmail; }
        }

        public ICommand DismissCommand
        {
            get
            {
                return _dismissCommand ?? (_dismissCommand = new MvxCommand<DailyToDoTaskViewModel>(async (followUpTask) =>
                {
                    _showAlertInteraction.Raise("Dismissing...");
                    followUpTask.Activity.FollowUpStatus = "Dismissed";
                    var success = await _activityService.UpdateActivityForProspectAsync(followUpTask.Activity);
                    if (success)
                    {
                        Messenger.Publish(new TaskDismissedMessage(this) { Activity = followUpTask.Activity });
                    }
                    _hideAlertInteraction.Raise();
                }));
            }
        }

        public ICommand SmsCommand
        {
            get
            {
                return _smsCommand ?? (_smsCommand = new MvxCommand(async () =>
                {
                    _showAlertInteraction.Raise("Getting Data...");
                    var activityWithTemplateData = await _activityService.GetActivityWithTemplateDataAsync(Activity.ProspectAddressNumber, Activity.InstanceID, "Text");
                    activityWithTemplateData.Prospect.ProspectCommunity.Community = Activity.Prospect.ProspectCommunity.Community;
                    _hideAlertInteraction.Raise();
                    await _navigationService.Navigate<SMSTaskViewModel, Activity>(activityWithTemplateData);
                }));
            }
        }

        public ICommand ComposeEmailCommand
        {
            get
            {
                return _composeEmailCommand ?? (_composeEmailCommand = new MvxCommand(async () =>
                {
                    _showAlertInteraction.Raise("Getting Data...");
                    var activityWithTemplateData = await _activityService.GetActivityWithTemplateDataAsync(Activity.ProspectAddressNumber, Activity.InstanceID, "Email");
                    _hideAlertInteraction.Raise();
                    await _navigationService.Navigate<EmailTaskViewModel, KeyValuePair<User, Activity>>(new KeyValuePair<User, Activity>(User, activityWithTemplateData));
                }));
            }
        }

        public ICommand PhoneCallCommand
        {
            get
            {
                return _phoneCallCommand ?? (_phoneCallCommand = new MvxCommand(async () =>
                {
                    _showAlertInteraction.Raise("Getting Data...");
                    var activityWithTemplateData = await _activityService.GetActivityWithTemplateDataAsync(Activity.ProspectAddressNumber, Activity.InstanceID, "Phone");
                    activityWithTemplateData.Prospect.ProspectCommunity.Community = Activity.Prospect.ProspectCommunity.Community;
                    _hideAlertInteraction.Raise();
                    await _navigationService.Navigate<CallTaskViewModel, KeyValuePair<Activity, User>> (new KeyValuePair<Activity, User>(activityWithTemplateData, User));
                }));
            }
        }

        public DailyToDoTaskViewModel(IMvxMessenger messenger, IMvxNavigationService navigationService, IActivityService activityService)
        {
            Messenger = messenger;
            _navigationService = navigationService;
            _activityService = activityService;
        }

        public void Prepare(User parameter)
        {
            User = parameter;
        }
    }
}
