﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Repositories
{
    public class ProspectRepository: BaseRepository, IProspectRepository
    {

        public async Task<object> AssignProspectToSalespersonAsync(string communityNumber, int prospectId, int salespersonId, string accessToken)
        {
            return await PostDataObjectToAPI(string.Format(_devUri + "Communities/{0}/Prospects/{1}/Salesperson/{2}", communityNumber, prospectId, salespersonId), null as object, accessToken);
        }

        public async Task<Prospect> GetProspectAsync(int prospectId, string accessToken)
        {
            return await GetDataObjectFromAPI<Prospect>(string.Format(_devUri + "Prospects/{0}?Fields=prospectCommunity", prospectId), accessToken);
        }

        public async Task<List<Prospect>> GetProspectsAsync(string accessToken, int? salespersonId, string type, List<Community> communities, int page = 1, int pageSize = 20, string searchTerm = null)
        {
            var communityList = string.Join(",",communities.Select(c => c.CommunityNumber));
            return await GetDataObjectFromAPI <List<Prospect>>(string.Format(_devUri + "Prospects?CommunityList={0}&SalespersonId={1}&Page={2}&PageSize={3}&SearchTerm={4}&Type={5}", communityList, salespersonId == null ? "" : salespersonId.Value.ToString(), page, pageSize, searchTerm, type), accessToken);
        }

        public async Task<bool> UpdateProspectAsync(Prospect prospect, string accessToken)
        {
            return await PutDataObjectToAPI(string.Format(_devUri + "Prospects/{0}", prospect.ProspectAddressNumber), prospect, accessToken);
        }
    }
}
