using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ImageProcessing.Api.ViewModel
{
    public class RequestConvertVM
    {
        [Required]
        public int Height { get; set; }
        [Required]
        public int Width { get; set; }
        [Required]
        public string ResizeMode { get; set; }
        [Required]
        public string ContentStream { get; set; }
    }
}