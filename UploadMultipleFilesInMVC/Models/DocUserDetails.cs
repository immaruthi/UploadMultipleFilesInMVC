using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UploadMultipleFilesInMVC.Models
{
    public class DocUserDetails
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName ="username")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName ="otherdetails")]
        public string OtherDetails { get; set; }
    }
}