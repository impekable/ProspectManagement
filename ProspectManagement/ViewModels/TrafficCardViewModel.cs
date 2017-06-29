using System;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class TrafficCardViewModel: BaseViewModel
    {
		private Prospect _prospect;
		private ICommand _showDetailTab;
		private ICommand _showCobuyerTab;

		public ICommand ShowDetailTab
		{
			get
			{
				return _showDetailTab ?? (_showDetailTab = new MvxCommand<Prospect>((prospect) => ShowViewModel<SplitDetailViewModel>(_prospect)));
			}
		}
		public ICommand ShowCobuyerTab
		{
			get
			{
				return _showCobuyerTab ?? (_showCobuyerTab = new MvxCommand<Prospect>((prospect) => ShowViewModel<CobuyerViewModel>(_prospect)));
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

		public override async void Start()
		{
			base.Start();
			await ReloadDataAsync();
		}

		public async void Init(Prospect prospect)
		{
			Prospect = prospect;
		}
    }
}
