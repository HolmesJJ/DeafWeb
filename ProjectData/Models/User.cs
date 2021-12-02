using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ProjectData.Models
{
    public class User
    {
        [Key]
        [MaxLength(10)]
        public int Id { get; set; }
        
        [Column(TypeName = "varchar(100)")]
        public string Username { get; set; }
        
        [Column(TypeName = "varchar(100)")]
        public string Password { get; set; }
        
        [Column(TypeName = "varchar(100)")]
        public string Profile { get; set; }

        [Column(TypeName = "char(1)")]
        public string Gender { get; set; }
        
        [Column(TypeName = "varchar(2)")]
        public string Age { get; set; }
        
        [Column(TypeName = "varchar(100)")]
        public string Hearing { get; set; }

        public int JumpForFunScore { get; set; }

        public int CubeHubScore { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string UserStatus { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string Email { get; set; }

        public int LoginTimes { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string DeviceId { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string ValidationCode { get; set; }

        public int ExpiredTime { get; set; }

        public Assistant Assistant { get; set; }
    }
}
