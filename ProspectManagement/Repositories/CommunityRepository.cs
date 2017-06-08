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
        public async Task<List<Community>> GetCommunitiesByDivisionAsync(string divisionCode, bool activeOnly = true)
        {
            return await GetDataObjectFromAPI<List<Community>>(string.Format(_devUri + "Communities?Area={0}&ActiveOnly={1}", divisionCode, activeOnly));
        }

        public async Task<List<Community>> GetCommunitiesBySalespersonAsync(int salespersonId)
        {
			return await GetDataObjectFromAPI<List<Community>>(string.Format(_devUri + "Salesperson/{0}/Communities", salespersonId));
        }
    }
}
