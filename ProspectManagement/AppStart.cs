using System;
using MvvmCross.Core.ViewModels;
using ProspectManagement.Core.ViewModels;

namespace ProspectManagement.Core
{
    public class AppStart : MvxNavigatingObject, IMvxAppStart
    {
        public void Start(object hint = null)
        {
            ShowViewModel<SplitRootViewModel>();
        }
    }
}