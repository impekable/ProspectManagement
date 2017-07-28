using System;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Repositories
{
    public class ProspectCache: IProspectCache
    {

        private Prospect _propsect;

        public Prospect GetProspectFromCache(int id)
        {
            return id == _propsect.ProspectAddressNumber ? _propsect : null;
        }

        public void SaveProspectToCache(Prospect prospect)
        {
            _propsect = prospect;
        }
    }
}
