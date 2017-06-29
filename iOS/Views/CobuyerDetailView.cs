using Foundation;
using System;
using UIKit;
using MvvmCross.iOS.Views;
using ProspectManagement.Core.ViewModels;
using MvvmCross.iOS.Views.Presenters.Attributes;
using MvvmCross.Binding.BindingContext;

namespace ProspectManagement.iOS.Views
{
	[MvxFromStoryboard("Main")]
	[MvxModalPresentation]
    public partial class CobuyerDetailView : MvxViewController<CobuyerDetailViewModel>
    {
        public CobuyerDetailView (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var set = this.CreateBindingSet<CobuyerDetailView, CobuyerDetailViewModel>();
			set.Bind(CloseButton).To(vm => vm.CloseCommand);
			set.Apply();			

		}
    }
}