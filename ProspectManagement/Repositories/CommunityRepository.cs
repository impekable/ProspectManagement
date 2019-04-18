using ProspectManagement.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Repositories
{
    public class CommunityRepository : BaseRepository, ICommunityRepository
    {
        public async Task<List<Community>> GetCommunitiesByDivisionAsync(string accessToken, string divisionCode, bool activeOnly = true)
        {
            return await GetDataObjectFromAPI<List<Community>>(string.Format(_e1Uri + "Communities?Area={0}&ActiveOnly={1}", divisionCode, activeOnly), accessToken);
        }

        public async Task<List<Community>> GetCommunitiesBySalespersonAsync(int salespersonId, string accessToken)
        {
            return await GetDataObjectFromAPI<List<Community>>(string.Format(_e1Uri + "Salesperson/{0}/Communities?Page=1&PageSize=999", salespersonId), accessToken);
        }
    }
}
