using System;

namespace Project20181209.Models
{
    public class ErrorViewModel
    {
        public int StatusCode { get; set; }

        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}