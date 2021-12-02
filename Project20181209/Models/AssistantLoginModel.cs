using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project20181209.Models
{
    public class AssistantLoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ValidationCode { get; set; }
        public string Message { get; set; }
        public bool IsValidated { get; set; }
    }
}
