using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ProspectManagement.Core.Constants;

namespace ProspectManagement.Core.Services
{
	public class CognitiveVisionService
	{

		// Replace <Subscription Key> with your valid subscription key.
		//const string subscriptionKey = "";

		// You must use the same region in your REST call as you used to
		// get your subscription keys. For example, if you got your
		// subscription keys from westus, replace "westcentralus" in the URL
		// below with "westus".
		//
		// Free trial subscription keys are generated in the westcentralus region.
		// If you use a free trial subscription key, you shouldn't need to change
		// this region.
		//const string uriBase = "https://eastus.api.cognitive.microsoft.com/vision/v1.0/recognizeText";

		/// <summary>
		/// Gets the handwritten text from the specified image file by using
		/// the Computer Vision REST API.
		/// </summary>
		/// <param name="imageFilePath">The image file with handwritten text.</param>
		private async Task<string> ReadHandwrittenText(string imageFilePath)
		{
			try
			{
				HttpClient client = new HttpClient();

				// Request headers.
				client.DefaultRequestHeaders.Add(
					"Ocp-Apim-Subscription-Key", PrivateKeys.CognitiveVisionSubscriptionKey);

				// Request parameter.
				// Note: The request parameter changed for APIv2.
				// For APIv1, it is "handwriting=true".
				string requestParameters = "handwriting=true";

				// Assemble the URI for the REST API Call.
				string uri = ConnectionURIs.CognitiveVisionUri + "?" + requestParameters;

				HttpResponseMessage response;

				// Two REST API calls are required to extract handwritten text.
				// One call to submit the image for processing, the other call
				// to retrieve the text found in the image.
				// operationLocation stores the REST API location to call to
				// retrieve the text.
				string operationLocation;

				// Request body.
				// Posts a locally stored JPEG image.
				byte[] byteData = getImageAsByteArray(imageFilePath);

				using (ByteArrayContent content = new ByteArrayContent(byteData))
				{
					// This example uses content type "application/octet-stream".
					// The other content types you can use are "application/json"
					// and "multipart/form-data".
					content.Headers.ContentType =
						new MediaTypeHeaderValue("application/octet-stream");

					// The first REST call starts the async process to analyze the
					// written text in the image.
					response = await client.PostAsync(uri, content);
				}

				// The response contains the URI to retrieve the result of the process.
				if (response.IsSuccessStatusCode)
					operationLocation =
						response.Headers.GetValues("Operation-Location").FirstOrDefault();
				else
				{
					// Display the JSON error data.
					string errorString = await response.Content.ReadAsStringAsync();
					//Console.WriteLine("\n\nResponse:\n{0}\n",
					//JToken.Parse(errorString).ToString());
					return String.Empty;
				}

				// The second REST call retrieves the text written in the image.
				//
				// Note: The response may not be immediately available. Handwriting
				// recognition is an async operation that can take a variable amount
				// of time depending on the length of the handwritten text. You may
				// need to wait or retry this operation.
				//
				// This example checks once per second for ten seconds.
				string contentString;
				int i = 0;
				do
				{
					System.Threading.Thread.Sleep(1000);
					response = await client.GetAsync(operationLocation);
					contentString = await response.Content.ReadAsStringAsync();
					++i;
				}
				while (i < 10 && contentString.IndexOf("\"status\":\"Succeeded\"") == -1);

				if (i == 10 && contentString.IndexOf("\"status\":\"Succeeded\"") == -1)
				{
					Console.WriteLine("\nTimeout error.\n");
					return String.Empty;
				}

				// Display the JSON response.
				return contentString;
			}
			catch (Exception e)
			{
				Console.WriteLine("\n" + e.Message);
			}
			return String.Empty;
		}

		public async Task<string> ReadHandwrittenText(Stream imgStream)
		{
			try
			{

				HttpClient client = new HttpClient();

				// Request headers.
				client.DefaultRequestHeaders.Add(
					"Ocp-Apim-Subscription-Key", PrivateKeys.CognitiveVisionSubscriptionKey);

				// Request parameter.
				// Note: The request parameter changed for APIv2.
				// For APIv1, it is "handwriting=true".
				string requestParameters = "handwriting=true";

				// Assemble the URI for the REST API Call.
				string uri = ConnectionURIs.CognitiveVisionUri + "?" + requestParameters;

				HttpResponseMessage response;

				// Two REST API calls are required to extract handwritten text.
				// One call to submit the image for processing, the other call
				// to retrieve the text found in the image.
				// operationLocation stores the REST API location to call to
				// retrieve the text.
				string operationLocation;

				// Request body.
				// Posts a JPEG image.
				BinaryReader binaryReader = new BinaryReader(imgStream);
				byte[] byteData = binaryReader.ReadBytes((int)imgStream.Length);

				using (ByteArrayContent content = new ByteArrayContent(byteData))
				{
					// This example uses content type "application/octet-stream".
					// The other content types you can use are "application/json"
					// and "multipart/form-data".
					content.Headers.ContentType =
						new MediaTypeHeaderValue("application/octet-stream");

					// The first REST call starts the async process to analyze the
					// written text in the image.
					response = await client.PostAsync(uri, content);
				}

				// The response contains the URI to retrieve the result of the process.
				if (response.IsSuccessStatusCode)
					operationLocation =
						response.Headers.GetValues("Operation-Location").FirstOrDefault();
				else
				{
					// Display the JSON error data.
					string errorString = await response.Content.ReadAsStringAsync();
					//Console.WriteLine("\n\nResponse:\n{0}\n",
					//JToken.Parse(errorString).ToString());
					return String.Empty;
				}

				// The second REST call retrieves the text written in the image.
				//
				// Note: The response may not be immediately available. Handwriting
				// recognition is an async operation that can take a variable amount
				// of time depending on the length of the handwritten text. You may
				// need to wait or retry this operation.
				//
				// This example checks once per second for ten seconds.
				string contentString;
				int i = 0;
				do
				{
					System.Threading.Thread.Sleep(1000);
					response = await client.GetAsync(operationLocation);
					contentString = await response.Content.ReadAsStringAsync();
					++i;
				}
				while (i < 10 && contentString.IndexOf("\"status\":\"Succeeded\"") == -1);

				if (i == 10 && contentString.IndexOf("\"status\":\"Succeeded\"") == -1)
				{
					Console.WriteLine("\nTimeout error.\n");
					return String.Empty;
				}

				// Display the JSON response.
				JObject jObject = JObject.Parse(contentString);
				var jRecognitionResult = jObject["recognitionResult"];
				var jLines = jRecognitionResult["lines"].ToArray();
				var result = String.Empty;
				foreach (var item in jLines)
				{
					var text = item["text"];
					result += text + "\n";
				}

				return result;
			}
			catch (Exception e)
			{
				Console.WriteLine("\n" + e.Message);
			}
			return String.Empty;
		}


		/// <summary>
		/// Returns the contents of the specified file as a byte array.
		/// </summary>
		/// <param name="imageFilePath">The image file to read.</param>
		/// <returns>The byte array of the image data.</returns>
		private byte[] getImageAsByteArray(string imageFilePath)
		{
			using (FileStream fileStream =
				new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
			{
				BinaryReader binaryReader = new BinaryReader(fileStream);
				return binaryReader.ReadBytes((int)fileStream.Length);
			}
		}
	}
}
