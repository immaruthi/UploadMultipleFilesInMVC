using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using UploadMultipleFilesInMVC.Models;

namespace UploadMultipleFilesInMVC.Services
{

    public class CognitiveAPIDataRequestModel
    {
        public string SubscriptionKey { get; set; }
        public string uriBase { get; set; }
        public byte[] imageData { get; set; }
    }

    public class CognitiveAPIService
    {
        public async Task<List<APIData>> GetCognitiveAPIResponse(CognitiveAPIDataRequestModel cognitiveAPIDataRequestModel)
        {
            string contentString = string.Empty;
            HttpClient client = new HttpClient();
            string subscriptionKey = "908a6575a1da43a9aa736f8bf8dd5124";
            string uriBase = "https://westcentralus.api.cognitive.microsoft.com/vision/v2.0/recognizeText";

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            string requestParameters = "mode=Handwritten";
            string uri = uriBase + "?" + requestParameters;
            HttpResponseMessage response;

            string operationLocation;

            using (ByteArrayContent content = new ByteArrayContent(cognitiveAPIDataRequestModel.imageData))
            {

                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                response = await client.PostAsync(uri, content);
            }

            if (response.IsSuccessStatusCode)
            {
                operationLocation = response.Headers.GetValues("Operation-Location").FirstOrDefault();

                
                int res = 0;
                do
                {
                    System.Threading.Thread.Sleep(1000);
                    response = await client.GetAsync(operationLocation);
                    contentString = await response.Content.ReadAsStringAsync();
                    ++res;
                }
                while (res < 10 && contentString.IndexOf("\"status\":\"Succeeded\"") == -1);

                if (res == 10 && contentString.IndexOf("\"status\":\"Succeeded\"") == -1)
                {
                    //return null;
                }

                 //return JsonConvert.DeserializeObject<RootObject>(contentString);
            }
            else
            {
                string errorString = await response.Content.ReadAsStringAsync();
                var errorObject = JsonConvert.DeserializeObject<VisionAPIErrorMessage>(errorString);

                //apidata.Add(new APIData()
                //{
                //    imageData = fileData,
                //    ImageUrl = fileSas.Uri.AbsoluteUri.ToString(),
                //    imageText = errorObject.error.message.ToString(),
                //    Error = "YES",
                //    Remarks = errorObject.error.message.ToString()
                //});
                //return null;
            }

            return null;//Task.FromResult<RootObject>(null);
        }
    }
}