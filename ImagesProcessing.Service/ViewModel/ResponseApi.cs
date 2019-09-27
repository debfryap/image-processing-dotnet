using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageProcessing.Api.ViewModel
{
    public class ResponseAPI<T>
    {
        public HeaderOutput header { get; set; }
        public T data { get; set; }
    }

    public class HeaderOutput
    {
        public long process_time { get; set; }
        public List<ErrorOutput> errors { get; set; }
    }

    public class ErrorOutput
    {
        public string message { get; set; }
        public string cause { get; set; }
        public string code { get; set; }
    }
}