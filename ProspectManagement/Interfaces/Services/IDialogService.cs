using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Interfaces.Services
{
    public interface IDialogService
    {
        Task ShowAlertAsync(string message, string title, string buttonText);
    }
}
