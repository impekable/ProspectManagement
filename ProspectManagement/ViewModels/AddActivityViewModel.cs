using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using MvvmValidation;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Messages;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class AddActivityViewModel : BaseViewModel, IMvxViewModel<Activity>
    {
        protected IMvxMessenger Messenger;
        private readonly IMvxNavigationService _navigationService;

        private MvxInteraction _hideAlertInteraction = new MvxInteraction();
        public IMvxInteraction HideAlertInteraction => _hideAlertInteraction;

        private readonly IActivityService _activityService;
        private Activity _activity;
        private string _note;

        private ICommand _saveCommand;
        private ICommand _closeCommand;

        private string _noteError;

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

        public ICommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new MvxCommand(() => Close(this)));
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
                       Activity.Notes = Note;
                       var activityAdded = await _activityService.AddActivityToProspectAsync(Activity.ProspectAddressNumber, Activity);
                        if (activityAdded != null)
                        {
                            Messenger.Publish(new ActivityAddedMessage(this) { AddedActivity = activityAdded });
                        }
                       Close(this);
                   }
                   _hideAlertInteraction.Raise();

               }));
            }
        }

        public string Note
        {
            get { return _note; }
            set
            {
                _note = value;
                RaisePropertyChanged(() => Note);
                Validator.ValidateAsync(nameof(Note));
            }
        }

        public AddActivityViewModel(IMvxMessenger messenger, IActivityService activityService, IMvxNavigationService navigationService)
        {
            Messenger = messenger;
            _activityService = activityService;
            _navigationService = navigationService;

            ConfigureValidationRules();
            Validator.ResultChanged += OnValidationResultChanged;

            Messenger.Subscribe<RefreshMessage>(async message => Close(this), MvxReference.Strong);
        }

        public async void Prepare(Activity activity)
        {
            Activity = activity;
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
            Validator.AddRule(nameof(Note),
                              () => RuleResult.Assert(!string.IsNullOrEmpty(Note) && Note.Length <= 1999, "Note is required and cannot be more than 1999 characters"));
        }
    }
}
