using Microsoft.EntityFrameworkCore;
using ProjectData.Models;
using System;

namespace ProjectData
{
    public class ProjectContext : DbContext
    {
        public ProjectContext(DbContextOptions options) : base(options) { }

        public DbSet<Assistant> Assistant { get; set; }
        public DbSet<English> English { get; set; }
        public DbSet<Progress> Progress { get; set; }
        public DbSet<Schedule> Schedule { get; set; }
        public DbSet<Models.Type> Type { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Game> Game { get; set; }
    }
}
