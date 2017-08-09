﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Repositories
{
	public class UserDefinedCodeRepository : BaseRepository, IUserDefinedCodeRepository
	{
        const string _baseUri = _devUri + "UserDefinedCodes/{0}/{1}";

        public async Task<List<UserDefinedCode>> GetUserDefinedCodesAsync(string productCode, string group)
        {
            return await GetDataObjectFromAPI<List<UserDefinedCode>>(string.Format(_baseUri, productCode, group));
        }
    }
}
