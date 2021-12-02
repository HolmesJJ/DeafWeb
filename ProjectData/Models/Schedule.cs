using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ProjectData.Models
{
    public class Schedule
    {
        [Key]
        [MaxLength(10)]
        public int Id { get; set; }

        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }

        public English English { get; set; }

        public User User { get; set; }
    }
}
