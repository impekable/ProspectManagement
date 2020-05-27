using System;
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
    public class TrafficCardQuestionViewModel : BaseViewModel, IMvxViewModel<KeyValuePair<Prospect, TrafficCardResponse>>
    {
        private readonly ITrafficCardResponseService _trafficCardResponseService;
        protected IMvxMessenger Messenger;
        private readonly IMvxNavigationService _navigationService;
        private readonly IUserService _userService;
        private User _user;
        private Prospect _prospect;
        private List<TrafficCardResponse> _responses;
        private TrafficCardResponse _response;
        private TrafficCardQuestion _currentQuestion;
        private TrafficCardAnswer _currentAnswer;

        public TrafficCardResponse Response
        {
            get { return _response; }
            set
            {
                _response = value;
                RaisePropertyChanged(() => Response);
            }
        }


        public TrafficCardAnswer CurrentAnswer
        {
            get { return _currentAnswer; }
            set
            {
                _currentAnswer = value;
                RaisePropertyChanged(() => CurrentAnswer);
            }
        }

        public TrafficCardQuestion CurrentQuestion
        {
            get { return _currentQuestion; }
            set
            {
                _currentQuestion = value;
                RaisePropertyChanged(() => CurrentQuestion);
            }
        }

        public MvxCommand CancelCommand
        {
            get
            {
                return new MvxCommand(async () =>
                {
                    await _navigationService.Close(this);
                });
            }
        }

        public MvxCommand SaveCommand
        {
            get
            {
                return new MvxCommand(async () =>
                {
                    var originalAnswer = Response.AnswerNumber;
                    Response.AnswerNumber = _currentAnswer != null ? _currentAnswer.AnswerNumber : 0;

                    var result = await _trafficCardResponseService.UpdateTrafficCardResponse(_prospect.ProspectAddressNumber, _responses);

                    if (result)
                    {
                        Messenger.Publish(new TrafficCardResponseChangedMessage(this) { ChangedResponse = Response });
                    }
                    else
                    {
                        Response.AnswerNumber = originalAnswer;
                    }
                    Analytics.TrackEvent("Traffic Card Updated", new Dictionary<string, string>
                    {
                        {"Community", _prospect.ProspectCommunity.CommunityNumber + " " + _prospect.ProspectCommunity.Community.Description},
                        {"SalesAssociate", _prospect.ProspectCommunity.SalespersonAddressNumber + " " + _prospect.ProspectCommunity.SalespersonName},
                        {"User", _user.AddressBook.AddressNumber + " " + _user.AddressBook.Name},
                    });
                    await _navigationService.Close(this);
                });
            }
        }

        public TrafficCardQuestionViewModel(IMvxMessenger messenger, ITrafficCardResponseService trafficCardResponseService, IMvxNavigationService navigationService, IUserService userService)
        {
            Messenger = messenger;
            _trafficCardResponseService = trafficCardResponseService;
            _navigationService = navigationService;
            _userService = userService;
        }

        public void Prepare(KeyValuePair<Prospect, TrafficCardResponse> parameter)
        {
            Response = parameter.Value;
            _prospect = parameter.Key;
            _responses = new List<TrafficCardResponse>();
            _responses.Add(Response);
            CurrentQuestion = Response.TrafficCardQuestion;
        }

        public override async Task Initialize()
        {
            _user = await _userService.GetLoggedInUser();
        }
    }
}