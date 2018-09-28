using System;
using System.Windows.Input;
using MvvmCross.ViewModels;
using ProspectManagement.Core.Models;
using System.Collections.Generic;
using System.Linq;
using MvvmCross.Plugin.Messenger;
using ProspectManagement.Core.Interactions;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Messages;
using MvvmCross.Navigation;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using MvvmCross.Commands;

namespace ProspectManagement.Core.ViewModels
{
    public class ActivitiesViewModel : BaseViewModel, IMvxViewModel<Prospect>
    {
        private Prospect _prospect;
        private User _user;
        private Activity _activity;
        private ICommand _showDetailTab;
        private ICommand _showTrafficCardTab;
        private ICommand _showCobuyerTab;
        private ICommand _selectionChangedCommand;

        private readonly IUserService _userService;
        private readonly IActivityService _activitiesService;

        protected IMvxMessenger Messenger;
        private readonly IMvxNavigationService _navigationService;

        private MvxInteraction _clearDetailsInteraction = new MvxInteraction();
        public IMvxInteraction ClearDetailsInteraction => _clearDetailsInteraction;

        private List<Activity> _activities;

        public ICommand ShowDetailTab
        {
            get
            {
                return _showDetailTab ?? (_showDetailTab = new MvxCommand<Prospect>((prospect) => _navigationService.Navigate<SplitDetailViewModel, KeyValuePair<Prospect, User>>(new KeyValuePair<Prospect, User>(_prospect, _user))));
            }
        }

        public ICommand ShowTrafficCardTab
        {
            get
            {
                return _showTrafficCardTab ?? (_showTrafficCardTab = new MvxCommand<Prospect>((prospect) => _navigationService.Navigate<TrafficCardViewModel, Prospect>(_prospect)));
            }
        }

        public ICommand ShowCobuyerTab
        {
            get
            {
                return _showCobuyerTab ?? (_showCobuyerTab = new MvxCommand<Prospect>((prospect) => _navigationService.Navigate<CobuyerViewModel, Prospect>(_prospect)));
            }
        }

        public ICommand SelectionChangedCommand
        {
            get
            {
                return _selectionChangedCommand ?? (_selectionChangedCommand = new MvxCommand<Activity>(async (activity) =>
                 {
                     activity = await _activitiesService.GetActivityForProspectAsync(_prospect.ProspectAddressNumber, activity.InstanceID);
                     Analytics.TrackEvent("Contact History Detail Viewed", new Dictionary<string, string>
                        {
                            {"Community", Prospect.ProspectCommunity.CommunityNumber + " " + Prospect.ProspectCommunity.Community.Description},
                            {"SalesAssociate", Prospect.ProspectCommunity.SalespersonAddressNumber + " " + Prospect.ProspectCommunity.SalespersonName},
                            {"User", _user.AddressBook.AddressNumber + " " + _user.AddressBook.Name},
                        });
                     _navigationService.Navigate<ActivityDetailViewModel, KeyValuePair<Prospect, Activity>>(new KeyValuePair<Prospect, Activity>(_prospect, activity));
                 }));
            }
        }

        public Prospect Prospect
        {
            get { return _prospect; }
            set
            {
                _prospect = value;
                RaisePropertyChanged(() => Prospect);
            }
        }

        public List<Activity> ActivitiesList
        {
            get { return _activities; }
            set
            {
                _activities = value;
                RaisePropertyChanged(() => ActivitiesList);
            }
        }

        public ActivitiesViewModel(IMvxMessenger messenger, IActivityService activitiesService, IMvxNavigationService navigationService, IUserService userService)
        {
            Messenger = messenger;
            _activitiesService = activitiesService;
            _navigationService = navigationService;
            _userService = userService;

            Messenger.Subscribe<RefreshMessage>(message => _clearDetailsInteraction.Raise(), MvxReference.Strong);
            Messenger.Subscribe<UserLogoutMessage>(message => _clearDetailsInteraction.Raise(), MvxReference.Strong);
        }

        public override async Task Initialize()
        {
            _user = await _userService.GetLoggedInUser();
            Analytics.TrackEvent("Contact History Viewed", new Dictionary<string, string>
            {
                {"Community", Prospect.ProspectCommunity.CommunityNumber + " " + Prospect.ProspectCommunity.Community.Description},
                {"SalesAssociate", Prospect.ProspectCommunity.SalespersonAddressNumber + " " + Prospect.ProspectCommunity.SalespersonName},
                {"User", _user.AddressBook.AddressNumber + " " + _user.AddressBook.Name},
            });

            ActivitiesList = await _activitiesService.GetActivitiesForProspectAsync(_prospect.ProspectAddressNumber);
        }

        public void Prepare(Prospect prospect)
        {
            Prospect = prospect;
        }
    }
}
