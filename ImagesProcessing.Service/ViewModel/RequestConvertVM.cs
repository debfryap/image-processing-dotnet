using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageProcessing.Api.ViewModel
{
    public class RequestConvertVM
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public string ResizeMode { get; set; }
    }
}