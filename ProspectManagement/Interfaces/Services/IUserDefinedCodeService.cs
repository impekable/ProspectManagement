using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Interfaces.Services
{
    public interface IUserDefinedCodeService
    {
        Task<List<UserDefinedCode>> GetPrefixUserDefinedCodes();
        Task<List<UserDefinedCode>> GetSuffixUserDefinedCodes();
        Task<List<UserDefinedCode>> GetContactPreferenceUserDefinedCodes();
        Task<List<UserDefinedCode>> GetStateUserDefinedCodes();
        Task<List<UserDefinedCode>> GetCountryUserDefinedCodes();
        Task<List<UserDefinedCode>> GetExcludeReasonUserDefinedCodes();
        Task<List<UserDefinedCode>> GetRankingUserDefinedCodes();
    }
}
