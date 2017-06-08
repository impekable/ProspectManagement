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
		const string _baseUri = _devUri + "Users/{0}";
        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await GetDataObjectFromAPI<User>(string.Format(_baseUri, userId));
        }
    }
}
