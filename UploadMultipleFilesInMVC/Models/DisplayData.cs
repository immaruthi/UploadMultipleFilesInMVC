using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UploadMultipleFilesInMVC.Models
{
    public class DisplayData
    {
        public List<APIData> listApiData { get; set; }
    }


    public class APIData
    {
        public byte[] imageData { get; set; }
        public string imageText { get; set; }
        public string ImageUrl { get; set; }
        public string Error { get; set; }
        public string Remarks { get; set; }
    }

}