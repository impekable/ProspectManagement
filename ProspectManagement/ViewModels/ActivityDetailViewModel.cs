using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using MvvmCross.Plugin.Messenger;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Messages;
using ProspectManagement.Core.Models;
using MvvmCross.Commands;

namespace ProspectManagement.Core.ViewModels

{
    public class ActivityDetailViewModel : BaseViewModel, IMvxViewModel<KeyValuePair<Prospect, Activity>>
    {
        private readonly IActivityService _activitiesService;
        protected IMvxMessenger Messenger;
        private readonly IMvxNavigationService _navigationService;
        private readonly IUserService _userService;
        private User _user;
        private Prospect _prospect;
        private Activity _activity;

        private ICommand _closeCommand;

        public Prospect Prospect
        {
            get { return _prospect; }
            set
            {
                _prospect = value;
                RaisePropertyChanged(() => Prospect);
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

        public string Notes
        {
            get
            {
                string size = "<meta name='viewport' content='width=device-width, initial-scale=1'>";

                if (Activity.EmailActivity != null)
                {
                    if (Activity.EmailActivity.HtmlBody != null)
                    {
                        return size + Activity.EmailActivity?.HtmlBody;
                    }
                    else if (Activity.EmailActivity.TextBody != null)
                    {
                        return size + Activity.EmailActivity?.TextBody.Replace(System.Environment.NewLine, "<br>");

                    }
                    else return "NO EMAIL BODY";

                }
                else if (Activity.Transcription != null || Activity.RecordingURL != null)
                {
                    var html = size + Activity.Transcription.Replace(System.Environment.NewLine, "<br>");
                    if (Activity.RecordingURL != null)
                    {
                        html += $"<br><audio controls><source src='{Activity.RecordingURL}.mp3' type='audio/mpeg'></source</audio>";
                    }
                    return html;
                }
                else if (!string.IsNullOrEmpty(Activity.Notes))
                    return size + Activity.Notes.Replace(System.Environment.NewLine, "<br>");
                else
                    return "NO NOTES";
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new MvxCommand(async () => await _navigationService.Close(this)));
            }
        }

        public ActivityDetailViewModel(IMvxMessenger messenger, IMvxNavigationService navigationService, IUserService userService, IActivityService activitiesService)
        {
            Messenger = messenger;
            _activitiesService = activitiesService;
            _navigationService = navigationService;
            //_userService = userService;
        }

        public void Prepare(KeyValuePair<Prospect, Activity> parameter)
        {
            Activity = parameter.Value;
            Prospect = parameter.Key;
        }
    }
}
