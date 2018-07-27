using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UploadMultipleFilesInMVC.Models
{
    //public class VisionAPIErrorMessage
    //{
    //}

    public class Error
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class VisionAPIErrorMessage
    {
        public Error error { get; set; }
    }

    //{"error":{"code":"InvalidImageDimension","message":"Image dimension (4160*3120) is out of range. Image dimensions should be in the range of 40 x 40 and 3200 x 3200."}}
}