using ProspectManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Interfaces.Services
{
    public interface IUserService
    {
		Task<User> GetLoggedInUser();

        Task<User> GetUserById(string userId);
    }
}
