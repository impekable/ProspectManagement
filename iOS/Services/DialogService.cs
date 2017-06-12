using ProspectManagement.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace ProspectManagement.iOS.Services
{
    public class DialogService : IDialogService
    {
        public Task ShowAlertAsync(string message, string title, string buttonText)
        {
            return Task.Run(() =>
            UIApplication.SharedApplication.InvokeOnMainThread(() =>            
            {
                new UIAlertView(title, message, null, buttonText).Show();
            }));
        }
    }
}
