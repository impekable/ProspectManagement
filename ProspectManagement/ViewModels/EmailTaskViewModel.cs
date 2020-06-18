using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using ProspectManagement.Core.Constants;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Messages;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class EmailTaskViewModel: BaseViewModel, IMvxViewModel<KeyValuePair<User, Activity>>
    {
        private MvxInteraction _hideAlertInteraction = new MvxInteraction();
        public IMvxInteraction HideAlertInteraction => _hideAlertInteraction;
        private MvxInteraction _showAlertInteraction = new MvxInteraction();
        public IMvxInteraction ShowAlertInteraction => _showAlertInteraction;

        private IMvxCommand _sendEmailCommand;
        private IMvxCommand _getContentCommand;
        private IMvxCommand _closeCommand;

        protected IMvxMessenger Messenger;
        private readonly IMvxNavigationService _navigationService;
        private readonly IActivityService _activityService;
        private readonly IUserService _userService;
        private readonly IDialogService _dialogService;

        private MvxInteraction _getContentInteraction = new MvxInteraction();
        public IMvxInteraction GetContentInteraction => _getContentInteraction;

        public Activity Activity { get; set; }
        public User User { get; set; }

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

        public IMvxCommand GetContentCommand
        {
            get
            {
                return _getContentCommand ?? (_getContentCommand = new MvxCommand(() =>
                {
                    _getContentInteraction.Raise();

                    //var activityWithTemplateData = await _activityService.GetActivityWithTemplateDataAsync(Activity.ProspectAddressNumber, Activity.InstanceID, "Phone");
                    //_navigationService.Navigate<ReviewRequestViewModel, PriceControlRequestViewModel>(this);
                }));
            }
        }

        public IMvxCommand SendEmailCommand
        {
            get
            {
                return _sendEmailCommand ?? (_sendEmailCommand = new MvxCommand(async () =>
                {
                    var editedHTML = Activity.TemplateData;
                    var optoutHTML = "<div align=\"center\" style=\"font-family: Arial, Helvetica, sans-serif; color: #000000; font-size: 14px\">"
                                + "To opt out of receiving future emails"
                                + "<a href=\"" + ConnectionURIs.E1CRMWebAppBaseURI + "/Home/OptOut/" + Activity.ProspectAddressNumber + "?ActivityId=" + Activity.ActivityID + "\">"
                                + " click here </a></div>";

                    if (!String.IsNullOrEmpty(editedHTML))
                    {
                        _showAlertInteraction.Raise();
                        Activity.DateCompleted = DateTime.UtcNow;
                        Activity.FollowUpStatus = "Completed";
                        Activity.FollowUpMethod = "Email";

                        EmailMessage email = new EmailMessage()
                        {
                            ActivityId = Activity.ActivityID,
                            UserName = User.UserId,
                            FromEmailAddress = User.Email.EmailAddress,
                            ToEmailAddress = Activity.Prospect.Email.EmailAddress,
                            SalespesonAddressNumber = Activity.SalespersonAddressNumber,
                            Subject = Activity.EmailSubject,
                            Body = editedHTML + optoutHTML
                        };

                        var success = await _activityService.SendEmailAsync(email, Activity);
                        if (success)
                        {
                            
                            Messenger.Publish(new TaskCompletedMessage(this) { Activity = Activity });

                            await _dialogService.ShowAlertAsync("Message sent", "Task Completed", "OK");
                            await _navigationService.Close(this);
                        }
                        _hideAlertInteraction.Raise();
                    }
                }));
            }
        }

        public EmailTaskViewModel(IDialogService dialogService, IMvxMessenger messenger, IMvxNavigationService navigationService, IActivityService activityService, IUserService userService)
        {
            _dialogService = dialogService;
            Messenger = messenger;
            _navigationService = navigationService;
            _activityService = activityService;
            _userService = userService;
        }

        public void Prepare(KeyValuePair<User, Activity> param)
        {
            Activity = param.Value;
            User = param.Key;
        }
    }
}
