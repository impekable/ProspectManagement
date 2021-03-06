﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Interfaces.Repositories
{
    public interface ICommunityRepository : IBaseRepository
    {
        Task<List<Community>> GetCommunitiesBySalespersonAsync(int salespersonId, string accessToken);

        Task<List<Community>> GetCommunitiesByDivisionAsync(string accessToken, string divisionCode, bool activeOnly);
    }
}
