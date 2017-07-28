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

		public Task<int> ShowAlertAsync(string title, string message, params string[] buttons)
		{
			var tcs = new TaskCompletionSource<int>();
			var alert = new UIAlertView
			{
				Title = title,
				Message = message
			};
			foreach (var button in buttons)
				alert.AddButton(button);
			alert.Clicked += (s, e) => tcs.TrySetResult((int)e.ButtonIndex);
			alert.Show();
			return tcs.Task;
		}
    }
}
