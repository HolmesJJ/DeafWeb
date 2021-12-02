using ProjectData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project20181209.Models
{
    public class HomeModel
    {
        public IEnumerable<User> User { get; set; }
        public string AssistantName { get; set; }
    }
}
