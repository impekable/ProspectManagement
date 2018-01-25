using System;
using MvvmCross.Plugins.Messenger;
namespace ProspectManagement.Core.Messages
{
    public class UserLogoutMessage: MvxMessage
    {
        public UserLogoutMessage(object sender): base(sender)
        {
        }
    }
}
