using ProjectData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project20181209.Models
{
    public class UserDetailModel
    {
        public User User { get; set; }
        public IEnumerable<Schedule> Schedule { get; set; }
        public IEnumerable<Progress> Progress { get; set; }
        public IEnumerable<ProjectData.Models.Type> Type { get; set; }
        public IEnumerable<English> English { get; set; }
        
        public int NewScheduleUser { get; set; }
        public int NewScheduleEnglish { get; set; }
        public DateTime NewScheduleStartDate { get; set; }
        public DateTime NewScheduleEndDate { get; set; }
        public int UserId { get; set; }
    }

    public class ChartModel
    {
        public User User { get; set; }
        public English English { get; set; }
        public IEnumerable<Progress> Progress { get; set; }
    }

    public class ScheduleModel
    {
        public Schedule Schedule { get; set; }
        public English English { get; set; }
    }

    public class ProgressModel
    {
        // public Progress Progress { get; set; }
        public English English { get; set; }
        public int Count { get; set; }
    }

    public class TypeModel
    {
        public ProjectData.Models.Type Type { get; set; }
        public IEnumerable<English> English { get; set; }
    }
}
