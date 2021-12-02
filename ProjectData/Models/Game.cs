using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProjectData.Models
{
    public class Game
    {
        [Key]
        [MaxLength(10)]
        public int Id { get; set; }

        [MaxLength(1)]
        public int IsStart { get; set; }
    }
}
