﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Repositories
{
	public class TrafficCardResponseRepository : BaseRepository, ITrafficCardResponseRepository
	{
        const string _baseUri = _e1Uri + "Prospects/{0}/TrafficCardResponses";

		public async Task<List<TrafficCardResponse>> GetTrafficCardResponsesForProspectAsync(int prospectAddressNumber, bool requiredOnly, string accessToken)
		{
			var responses = await GetDataObjectFromAPI<List<TrafficCardResponse>>(string.Format(_baseUri + "?Fields=trafficCardQuestion.trafficCardAnswer&PageSize=99", prospectAddressNumber), accessToken);
			if (requiredOnly)
				responses = responses.Where(r => r.TrafficCardQuestion.WeightingValue > 0).ToList();
			return responses == null ? new List<TrafficCardResponse>() : responses.OrderBy(r => r.TrafficCardQuestion.QuestionSequenceNumber).ToList();
		}

		public async Task<bool> UpdateTrafficCardResponseAsync(int prospectAddressNumber, List<TrafficCardResponse> response, string accessToken)
		{
			return await PutDataObjectToAPI(string.Format(_baseUri, prospectAddressNumber), response, accessToken);
		}
	}
}
