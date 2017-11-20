using MvvmCross.iOS.Views;
using System;
using System.Collections.Generic;
using System.Text;
using MvvmCross.Core.ViewModels;
using UIKit;

namespace ProspectManagement.iOS
{
    public class StoryBoardContainer: MvxIosViewsContainer
    {
        public override IMvxIosView CreateViewOfType(Type viewType, MvxViewModelRequest request)
        {
            return (IMvxIosView)UIStoryboard.FromName("Main", null).InstantiateViewController(viewType.Name);
        }
    }
}
