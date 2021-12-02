using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ProjectData.Models
{
    public class English
    {
        [Key]
        [MaxLength(10)]
        public int Id { get; set; }
        
        [Column(TypeName = "nvarchar(255)")]
        public string Content { get; set; }

        public Type Type { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string Detail { get; set; }
    }
}
