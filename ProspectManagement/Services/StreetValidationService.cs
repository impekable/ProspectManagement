using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;
using ProspectManagement.Core.Models.App;
using ProspectManagement.Core.Repositories;

namespace ProspectManagement.Core.Services
{
    public class StreetValidationService : BaseRepository, IStreetValidationService
    {
		const string authId = "3e13897f-a5d5-4882-ab54-8462b3b5a46d";
		const string authToken = "DsNvkWKWsPBhZKcSDqZd";
		protected const string _smartystrUri = "https://api.smartystreets.com/street-address";

        public async Task<bool> Validate(StreetAddress streetAddress)
        {
            var _url = String.Format(_smartystrUri + "?auth-id={0}&auth-token={1}&street={2}&city={3}&state={4}&zipcode={5}", authId, authToken, streetAddress.AddressLine1, streetAddress.City, streetAddress.State, streetAddress.PostalCode);
            var results = await GetDataObjectFromAPI<StreetAddressValidationResult[]>(_url);
            if (results.Length > 0)
            {
                streetAddress.AddressLine1 = results[0].delivery_line_1;
                streetAddress.City = results[0].components.city_name;
                streetAddress.State = results[0].components.state_abbreviation;
                streetAddress.PostalCode = results[0].components.zipcode + "-" + results[0].components.plus4_code;
                streetAddress.County = results[0].metadata.county_name;
                streetAddress.StreetAddressVerified = true;
                return true;
            }
            else
            {
                streetAddress.StreetAddressVerified = false;
                return false;
            }
        }
    }
}
