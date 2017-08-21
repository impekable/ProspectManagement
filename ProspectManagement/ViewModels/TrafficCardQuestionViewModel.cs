using System;
using System.Collections.Generic;
using System.Linq;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Messages;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class TrafficCardQuestionViewModel : BaseViewModel
    {
        private Prospect _prospect;

        private readonly ITrafficCardResponseService _trafficCardResponseService;
        private readonly IProspectCache _prospectCache;
        protected IMvxMessenger Messenger;

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
                return new MvxCommand(() =>
                {
                    Close(this);
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
                    Close(this);
                });
            }
        }

        public TrafficCardQuestionViewModel(IMvxMessenger messenger, IProspectCache prospectCache, ITrafficCardResponseService trafficCardResponseService)
        {
            Messenger = messenger;
            _prospectCache = prospectCache;
            _trafficCardResponseService = trafficCardResponseService;
        }

        public override async void Start()
        {
            base.Start();
            await ReloadDataAsync();
        }

        public async void Init(Prospect prospect)
        {
            _prospect = prospect;
            Response = _prospectCache.GetTrafficCardResponseFromCache(prospect.ProspectAddressNumber);
            _responses = new List<TrafficCardResponse>();
            _responses.Add(Response);
            CurrentQuestion = Response.TrafficCardQuestion;
        }
    }
}