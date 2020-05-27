using ProspectManagement.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        const string _baseUri = _e1Uri + "Users/{0}";
        public async Task<User> GetUserByIdAsync(string userId, string accessToken)
        {
            return await GetDataObjectFromAPI<User>(string.Format(_baseUri + "?Fields=usingTelephony,phoneNumbers,unreadSMSCount", userId), accessToken);
        }
    }
}
