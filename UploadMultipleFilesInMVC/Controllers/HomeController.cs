using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UploadMultipleFilesInMVC.DocumentService;
using UploadMultipleFilesInMVC.Models;

namespace UploadMultipleFilesInMVC.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public async Task<ActionResult> UploadFiles()
        {
            
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UploadFiles(HttpPostedFileBase[] files)
        {
            List<APIData> apidata = new List<APIData>();
            try
            {
                HttpClient client = new HttpClient();

                string subscriptionKey = "908a6575a1da43a9aa736f8bf8dd5124";
                string uriBase = "https://westcentralus.api.cognitive.microsoft.com/vision/v2.0/recognizeText";

                //var storageAccountConnectionString_File = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=cognitivedata;AccountKey=c7mMSC8rWc/qAw3Wq2gIZ29qmJlisdrdsROn0bH1+4Egl6OtkAympYRuHjZqA3yesY948O+RbHmHaC0CH2ukJw==;EndpointSuffix=core.windows.net");

                var storageAccountConnectionString_File = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=ocrextracthandwrittentx;AccountKey=pOHbwrr3hGc7Q0msNk7rgEIDtuKq54YofeoJeOOicm05TLYtnNPvnaxnU+ruC+oZesmwp2qoYzge+pvUq1ihvA==;EndpointSuffix=core.windows.net");

                CloudFileClient cloudFileClient = storageAccountConnectionString_File.CreateCloudFileClient();

                // CloudFileShare cloudFileShare = cloudFileClient.GetShareReference("ocrextracthandwrittentx");

                CloudFileShare cloudFileShare = cloudFileClient.GetShareReference("ocrextracthandwrittentx");

                //Ensure model state is valid
                if (ModelState.IsValid)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (files[i].FileName.EndsWith(".png") || files[i].FileName.EndsWith(".jpg"))
                        {
                            try
                            {
                                if (cloudFileShare.Exists())
                                {
                                    CloudFileDirectory rootDir = cloudFileShare.GetRootDirectoryReference();
                                    CloudFile fileSas = rootDir.GetFileReference(Path.GetFileName(files[i].FileName));

                                    byte[] fileData = null;
                                    using (var binaryReader = new BinaryReader(Request.Files[i].InputStream))
                                    {
                                        fileData = binaryReader.ReadBytes(Request.Files[i].ContentLength);
                                    }

                                    fileSas.UploadFromStream(new MemoryStream(fileData));

                                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                                    string requestParameters = "mode=Handwritten";
                                    string uri = uriBase + "?" + requestParameters;
                                    HttpResponseMessage response;

                                    string operationLocation;

                                    using (ByteArrayContent content = new ByteArrayContent(fileData))
                                    {

                                        content.Headers.ContentType =
                                            new MediaTypeHeaderValue("application/octet-stream");

                                        response = await client.PostAsync(uri, content);
                                    }

                                    if (response.IsSuccessStatusCode)
                                    {
                                        operationLocation = response.Headers.GetValues("Operation-Location").FirstOrDefault();

                                        string contentString;
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

                                        var rootobject = JsonConvert.DeserializeObject<RootObject>(contentString);

                                        apidata.Add(new APIData()
                                        {
                                            imageData = fileData,
                                            imageText = rootobject.recognitionResult.lines[0].text,
                                            ImageUrl = fileSas.Uri.AbsoluteUri.ToString(),
                                            Error = "NO",
                                            Remarks = "No Errors Found"
                                        });

                                        var jsonResponse = JToken.Parse(contentString).ToString();

                                        //LocalResource localResource = RoleEnvironment.GetLocalResource("HandwrittenText");

                                        //Define the file name and path
                                        //string[] paths = { localResource.RootPath, "handtext.txt" };
                                        String filePath = Server.MapPath("~/HandwrittenText/handtext.txt");

                                        //if (System.IO.File.Exists(filePath))
                                        //{
                                        //    using (StreamWriter sw = System.IO.File.AppendText(filePath))
                                        //    {
                                        //        sw.WriteLine("===========================" + Path.GetFileName(files[i].FileName) + "================");
                                        //        sw.WriteLine(jsonResponse.ToString());
                                        //        sw.WriteLine("===========================================");
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    using (StreamWriter sw = System.IO.File.CreateText(filePath))
                                        //    {
                                        //        sw.WriteLine("===========================" + Path.GetFileName(files[i].FileName) + "================");
                                        //        sw.WriteLine(jsonResponse.ToString());
                                        //        sw.WriteLine("===========================================");
                                        //    }
                                        //}
                                    }
                                    else
                                    {
                                        string errorString = await response.Content.ReadAsStringAsync();
                                        var errorObject = JsonConvert.DeserializeObject<VisionAPIErrorMessage>(errorString);

                                        apidata.Add(new APIData()
                                        {
                                            imageData = fileData,
                                            ImageUrl = fileSas.Uri.AbsoluteUri.ToString(),
                                            imageText = errorObject.error.message.ToString(),
                                            Error = "YES",
                                            Remarks = errorObject.error.message.ToString()
                                        });
                                        //return null;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                //return RedirectToAction("DisplayImages");
                return RedirectToAction("GetErrorPage");
            }
            TempData["ApiData"] = apidata;
            return RedirectToAction("DisplayImages"); //View();
        }

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }

        [HttpGet]
        public async Task<ActionResult> DisplayImages()
        {
            List<APIData> apidatas = TempData["ApiData"] as List<APIData>;

            CosmosAPIService cosmosAPIService = new CosmosAPIService();

            var responseAPIService = await cosmosAPIService.CreateDocumentCollection(apidatas);

            return View(getDisplayData(apidatas));
        }

        [HttpPost]
        public async Task<ActionResult> GetErrorPage()
        {
            return View();
        }

        public IEnumerable<DisplayData> getDisplayData(List<APIData> apidatas)
        {
            DisplayData display = new DisplayData();

            //List<APIData> apidata = new List<APIData>();

            //apidata.Add(new APIData()
            //{
            //    imageData = GetImageAsByteArray(@"C:\Users\Bhargavi\Desktop\birds\sample.jpg"),
            //    imageText = "Hi This is sample text"
            //});

            //apidata.Add(new APIData()
            //{
            //    imageData = GetImageAsByteArray(@"C:\Users\Bhargavi\Desktop\birds\sample.jpg"),
            //    imageText = "Hi This is sample text"
            //});

            //apidata.Add(new APIData()
            //{
            //    imageData = GetImageAsByteArray(@"C:\Users\Bhargavi\Desktop\birds\sample.jpg"),
            //    imageText = "Hi This is sample text"
            //});

            display.listApiData = apidatas;

            List<DisplayData> displayDatas = new List<DisplayData>();

            displayDatas.Add(display);

            return displayDatas;
        }

    }
}
