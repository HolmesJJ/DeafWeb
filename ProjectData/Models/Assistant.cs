using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ProjectData.Models
{
    public class Assistant
    {
        [Key]
        [MaxLength(10)]
        public int Id { get; set; }
        
        [Column(TypeName = "varchar(100)")]
        public string Username { get; set; }
        
        [Column(TypeName = "varchar(100)")]
        public string Password { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string UserStatus { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string Email { get; set; }

        public int LoginTimes { get; set; }
    }
}
