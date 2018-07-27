using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using UploadMultipleFilesInMVC.Models;

namespace UploadMultipleFilesInMVC.DocumentService
{
    public class CosmosAPIService
    {
        public async Task<bool> CreateDocumentCollection(List<APIData> apiData)
        {
            bool response = await Task.FromResult<bool>(true);
            
            try
            {
                DocumentClient documentClient = new DocumentClient(new Uri(ConfigurationManager.AppSettings["endpoint"]), ConfigurationManager.AppSettings["authKey"]);

                var createDataBaseResponse = await documentClient.CreateDatabaseIfNotExistsAsync(new Microsoft.Azure.Documents.Database { Id = "CognitiveAPIDemoDataSource" });

                var createDataBaseCollectionResponse = await documentClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("CognitiveAPIDemoDataSource"), new DocumentCollection { Id = "PocCollection" }, new RequestOptions { OfferThroughput = 1000 });

                foreach (var apidata in apiData)
                {
                    string OCRID = Guid.NewGuid().ToString();
                    var objCollection = new CosmosDocumentCollection()
                    {
                        extracthandwrittenText = new ExtractHandwrittenText()
                        {
                            OCR_Id = OCRID,
                            DocURL = apidata.ImageUrl,
                            SourceName = "File Storage",
                            Status = "Inserted"
                        },
                        text = new Text()
                        {
                            Id = "1",
                            DocUserId = "sample",
                            OCR_Id = OCRID,
                            TextDetails = "sample"
                        },
                        textLocation = new TextLocation()
                        {
                            Id = "1",
                            LocationName = "sample",
                            OCR_Id = OCRID,
                            OtherDetails = "sample"
                        },
                        textDocumentType = new TextDocumentType()
                        {
                            Id = "1",
                            Details = "sample",
                            OCR_Texttid = "sample"
                        },
                        docUserDetails = new DocUserDetails()
                        {
                            Id = "1",
                            OtherDetails = "sample",
                            UserName = "sample"
                        }
                    };

                    var insertDocumentResponse = await documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("CognitiveAPIDemoDataSource", "PocCollection"), objCollection);
                }

            }
            catch (Exception ex)
            {
                response = await Task.FromResult<bool>(false);
            }

            return response;
        }
    }
}