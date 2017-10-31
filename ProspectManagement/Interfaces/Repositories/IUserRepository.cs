using ProspectManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Interfaces.Repositories
{
    public interface IUserRepository : IBaseRepository
    {
        Task<User> GetUserByIdAsync(string userId, string accessToken);
    }
}
