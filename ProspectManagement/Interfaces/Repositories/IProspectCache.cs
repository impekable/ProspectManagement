using System;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Interfaces.Repositories
{
    public interface IProspectCache
    {
        Prospect GetProspectFromCache(int id);
        void SaveProspectToCache(Prospect prospect);
    }
}
