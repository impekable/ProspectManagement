using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AppCenter.Analytics;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using MvvmCross.Plugin.Messenger;
using MvvmValidation;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Messages;
using ProspectManagement.Core.Models;
using MvvmCross.Commands;
using System.Collections.ObjectModel;
using System.Linq;
using ProspectManagement.Core.Extensions;

namespace ProspectManagement.Core.ViewModels
{
    public class AddActivityViewModel : BaseViewModel, IMvxViewModel<Activity>
    {
        protected IMvxMessenger Messenger;
        private readonly IMvxNavigationService _navigationService;
        private readonly IUserService _userService;
        private readonly IBuyerDecisionsService _buyerDecisionsService;
        private readonly IUserDefinedCodeService _userDefinedCodeService;

        private MvxInteraction _hideAlertInteraction = new MvxInteraction();
        public IMvxInteraction HideAlertInteraction => _hideAlertInteraction;

        private readonly IActivityService _activityService;
        private Activity _activity;
        private string _note;
        private User _user;
        private ObservableCollection<UserDefinedCode> _rankings;
        private BuyerDecisions _buyerDecisions;
        private UserDefinedCode _activeRanking;

        private ICommand _saveCommand;
        private ICommand _closeCommand;
        private ICommand _addAnalyticsScanPhotoCommand;
        private ICommand _addAnalyticsHandwritingCommand;

        private string _noteError;
        private bool _noteEntered;
        private bool _needCategory;

        public bool NeedCategory
        {
            get { return _needCategory; }
            set
            {
                _needCategory = value;
                RaisePropertyChanged(() => NeedCategory);
            }
        }

        public string NoteError
        {
            get { return _noteError; }
            set
            {
                _noteError = value;
                RaisePropertyChanged(() => NoteError);
            }
        }

        public Activity Activity
        {
            get { return _activity; }
            set
            {
                _activity = value;
                RaisePropertyChanged(() => Activity);
            }
        }

        public BuyerDecisions BuyerDecisions
        {
            get { return _buyerDecisions; }
            set
            {
                _buyerDecisions = value;
                RaisePropertyChanged(() => BuyerDecisions);
            }
        }

        public ObservableCollection<UserDefinedCode> Rankings
        {
            get { return _rankings; }
            set
            {
                _rankings = value;
                RaisePropertyChanged(() => Rankings);
            }
        }

        public UserDefinedCode ActiveRanking
        {
            get { return _activeRanking; }
            set
            {
                _activeRanking = value;
                BuyerDecisions.Ranking = _activeRanking.Code;
                RaisePropertyChanged(() => ActiveRanking);
            }
        }

        public ICommand AddAnalyticsScanPhotoCommand
        {
            get
            {
                return _addAnalyticsScanPhotoCommand ?? (_addAnalyticsScanPhotoCommand = new MvxCommand(() =>
                    Analytics.TrackEvent("Converted Photo", new Dictionary<string, string>
                    {
                        {"SalesAssociate", Activity.SalespersonAddressNumber.ToString() + " " + Activity.Prospect.ProspectCommunity.SalespersonName},
                        {"Community", Activity.Prospect.ProspectCommunity.CommunityNumber + " " + Activity.Prospect.ProspectCommunity.Community.Description},
                        {"ActivityType", Activity.ActivityType},
                        {"User", _user.AddressBook.AddressNumber + " " + _user.AddressBook.Name},
                })));
            }
        }

        public ICommand AddAnalyticsHandwritingCommand
        {
            get
            {
                return _addAnalyticsHandwritingCommand ?? (_addAnalyticsHandwritingCommand = new MvxCommand(() =>
                    Analytics.TrackEvent("Converted Writing", new Dictionary<string, string>
                    {
                        {"SalesAssociate", Activity.SalespersonAddressNumber.ToString() + " " + Activity.Prospect.ProspectCommunity.SalespersonName},
                        {"Community", Activity.Prospect.ProspectCommunity.CommunityNumber + " " + Activity.Prospect.ProspectCommunity.Community.Description},
                        {"ActivityType", Activity.ActivityType},
                        {"User", _user.AddressBook.AddressNumber + " " + _user.AddressBook.Name},
                })));
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new MvxCommand(async () => await _navigationService.Close(this)));
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new MvxCommand(async () =>
               {

                   await ValidateAsync();
                   if (IsValid.GetValueOrDefault())
                   {
                       Activity.Notes = _noteEntered ? Note : "";
                       var activityAdded = await _activityService.AddActivityToProspectAsync(Activity.ProspectAddressNumber, Activity);
                       if (activityAdded != null)
                       {
                           var decisionsUpdated = await _buyerDecisionsService.UpdateBuyerDecisionsAsync(BuyerDecisions);
                       }

                       await _navigationService.Close(this);
                       if (activityAdded != null)
                       {
                           Messenger.Publish(new ActivityAddedMessage(this) { AddedActivity = activityAdded });
                       }
                   }
                   _hideAlertInteraction.Raise();

                   Analytics.TrackEvent("Activity Added", new Dictionary<string, string>
                    {
                        {"SalesAssociate", Activity.SalespersonAddressNumber.ToString() + " " + Activity.Prospect.ProspectCommunity.SalespersonName},
                        {"Community", Activity.Prospect.ProspectCommunity.CommunityNumber + " " + Activity.Prospect.ProspectCommunity.Community.Description},
                        {"ActivityType", Activity.ActivityType},
                        {"User", _user.AddressBook.AddressNumber + " " + _user.AddressBook.Name},
                    });

               }));
            }
        }

        public string Note
        {
            get { return _note; }
            set
            {
                _note = value;
                _noteEntered = !_note.Equals("Enter Note Here...") && !String.IsNullOrEmpty(_note);
                RaisePropertyChanged(() => Note);
                Validator.ValidateAsync(nameof(Note));
            }
        }

        public AddActivityViewModel(IMvxMessenger messenger, IBuyerDecisionsService buyerDecisionsService, IUserDefinedCodeService userDefinedCodeService, IActivityService activityService, IMvxNavigationService navigationService, IUserService userService)
        {
            Messenger = messenger;
            _activityService = activityService;
            _navigationService = navigationService;
            _userService = userService;
            _buyerDecisionsService = buyerDecisionsService;
            _userDefinedCodeService = userDefinedCodeService;

            ConfigureValidationRules();
            Validator.ResultChanged += OnValidationResultChanged;
        }

        public void Prepare(Activity activity)
        {
            Activity = activity;
            Note = "Enter Note Here...";
        }

        public override async Task Initialize()
        {
            _user = await _userService.GetLoggedInUser();

            BuyerDecisions = await _buyerDecisionsService.GetBuyerDecisionsAsync(Activity.ProspectAddressNumber);

            Rankings = (await _userDefinedCodeService.GetRankingUserDefinedCodes())
                            .Where(r => !r.Code.Equals("0") && !r.Code.Equals("D")).ToObservableCollection();

            if (BuyerDecisions != null && !String.IsNullOrEmpty(BuyerDecisions.Ranking))
            {
                ActiveRanking = Rankings.FirstOrDefault(p => p.Code == BuyerDecisions.Ranking);
            }

            NeedCategory = Activity.ContactMethod == "In-Person" && (ActiveRanking == null || String.IsNullOrEmpty(ActiveRanking.Code));

        }

        protected async void Validate()
        {
            await ValidateAsync();
        }

        protected async Task ValidateAsync()
        {
            var result = await Validator.ValidateAllAsync();

            UpdateValidationSummaryAndDetails(result);
        }

        private void UpdateValidationSummaryAndDetails(ValidationResult validationResult)
        {
            UpdateValidationSummary(validationResult);
            NoteError = Validator.GetResult(nameof(Note)).ToString();
        }

        private void OnValidationResultChanged(object sender, ValidationResultChangedEventArgs e)
        {
            if (!IsValid.GetValueOrDefault(true))
            {
                ValidationResult validationResult = Validator.GetResult();

                UpdateValidationSummaryAndDetails(validationResult);
            }
        }

        private void ConfigureValidationRules()
        {
            Validator.AddRule(() => ActiveRanking,
                   () =>
                   {
                       var result = !(Activity.ContactMethod == "In-Person" && (ActiveRanking == null || String.IsNullOrEmpty(ActiveRanking.Code)));
                       return RuleResult.Assert(result, string.Format("Category is required"));
                   });

            Validator.AddRule(nameof(Note),
                              () => RuleResult.Assert(_noteEntered && Note.Length <= 1999, "Note is required and cannot be more than 1999 characters"));
        }
    }
}
