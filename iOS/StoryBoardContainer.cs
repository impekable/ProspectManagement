using System;
using UIKit;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.ViewModels;

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
