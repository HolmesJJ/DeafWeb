using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ProjectData.Models
{
    public class Type
    {
        [Key]
        [MaxLength(10)]
        public int Id { get; set; }
        
        [Column(TypeName = "varchar(255)")]
        public string Name { get; set; }
    }
}
