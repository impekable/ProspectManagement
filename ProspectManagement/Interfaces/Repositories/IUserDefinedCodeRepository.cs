using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Interfaces.Repositories
{
	public interface IUserDefinedCodeRepository : IBaseRepository
	{
        Task<List<UserDefinedCode>> GetUserDefinedCodesAsync(string productCode, string group, string accessToken);
	}
}
