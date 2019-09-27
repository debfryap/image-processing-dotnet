using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageProcessing.Api.ViewModel
{
    public class ResultUploadVM
    {
        public string filename { get; set; }
        public string contenttype { get; set; }
        public string real_size { get; set; }
        public string convert_size { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string data_image { get; set; }
        
    }
}