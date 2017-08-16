using System;
using System.Collections.Generic;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Repositories
{
    public class ProspectCache: IProspectCache
    {

        private Prospect _propsect;
        private KeyValuePair<int, TrafficCardResponse> _response;

        public Prospect GetProspectFromCache(int id)
        {
            return id == _propsect.ProspectAddressNumber ? _propsect : null;
        }

        public TrafficCardResponse GetTrafficCardResponseFromCache(int id)
        {
            return id == _response.Key ? _response.Value : null;
        }

        public void SaveProspectToCache(Prospect prospect)
        {
            _propsect = prospect;
        }

        public void SaveTrafficCardResponseToCache(int prospectId, TrafficCardResponse response)
        {
            _response =  new KeyValuePair<int, TrafficCardResponse>(prospectId, response);
        }
    }
}
