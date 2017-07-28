using System;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.ViewModels
{
    public class CobuyerDetailViewModel: BaseViewModel
    {
        private ICommand _closeCommand;
        private Cobuyer _cobuyer;

		public ICommand CloseCommand
		{
			get
			{
				return _closeCommand ?? (_closeCommand = new MvxCommand(() => Close(this)));
			}
		}
		public Cobuyer Cobuyer
		{
			get { return _cobuyer; }
			set
			{
				_cobuyer = value;
				RaisePropertyChanged(() => Cobuyer);
			}
		}

		public override async void Start()
		{
			base.Start();
			await ReloadDataAsync();
		}

		public async void Init(Cobuyer cobuyer)
		{
			Cobuyer = cobuyer;
		}
    }
}
