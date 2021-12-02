using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project20181209.Models
{
    public class AddUserModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public IFormFile Profile { set; get; }
        public string Gender { set; get; }
        public string Age { get; set; }
        public string Hearing { get; set; }
        public string AssistantId { get; set; }
        public string Message { get; set; }
    }
}
