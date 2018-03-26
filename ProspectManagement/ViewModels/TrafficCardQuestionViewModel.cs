using System;
using System.Collections.Generic;
using System.Linq;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Messages;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class TrafficCardQuestionViewModel : BaseViewModel, IMvxViewModel<KeyValuePair<int, TrafficCardResponse>>
    {
        private readonly ITrafficCardResponseService _trafficCardResponseService;
        protected IMvxMessenger Messenger;
        private readonly IMvxNavigationService _navigationService;

        private int _prospectNumber;
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

                    var result = await _trafficCardResponseService.UpdateTrafficCardResponse(_prospectNumber, _responses);

                    if (result)
                    {
                        Messenger.Publish(new TrafficCardResponseChangedMessage(this) { ChangedResponse = Response });
                    }
                    else
                    {
                        Response.AnswerNumber = originalAnswer;
                    }
                    await _navigationService.Close(this);
                });
            }
        }

        public TrafficCardQuestionViewModel(IMvxMessenger messenger, ITrafficCardResponseService trafficCardResponseService, IMvxNavigationService navigationService)
        {
            Messenger = messenger;
            _trafficCardResponseService = trafficCardResponseService;
            _navigationService = navigationService;

            Messenger.Subscribe<RefreshMessage>(async message => await _navigationService.Close(this), MvxReference.Strong);
           
        }

        public void Prepare(KeyValuePair<int, TrafficCardResponse> parameter)
        {
            Response = parameter.Value;
            _prospectNumber = parameter.Key;
            _responses = new List<TrafficCardResponse>();
            _responses.Add(Response);
            CurrentQuestion = Response.TrafficCardQuestion;
        }
    }
}