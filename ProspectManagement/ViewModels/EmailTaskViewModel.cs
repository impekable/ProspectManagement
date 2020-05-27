using System;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class EmailTaskViewModel: BaseViewModel, IMvxViewModel<Activity>
    {

        private IMvxCommand _sendEmailCommand;
        private IMvxCommand _getContentCommand;
        private IMvxCommand _closeCommand;

        private readonly IMvxNavigationService _navigationService;
        private readonly IActivityService _activityService;

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
                    //var activityWithTemplateData = await _activityService.GetActivityWithTemplateDataAsync(Activity.ProspectAddressNumber, Activity.InstanceID, "Phone");
                    //_navigationService.Navigate<ReviewRequestViewModel, PriceControlRequestViewModel>(this);
                }));
            }
        }

        public EmailTaskViewModel(IMvxNavigationService navigationService, IActivityService activityService)
        {
            _navigationService = navigationService;
            _activityService = activityService;
        }

        public void Prepare(Activity activity)
        {
            Activity = activity;
        }
    }
}
