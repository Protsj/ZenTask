using Microsoft.EntityFrameworkCore;
using ZenTask.Core.Models;

namespace ZenTask.Core.Data
{
    public class TaskDbContext : DbContext
    {
        public DbSet<BaseTask> Tasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Tasks.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BaseTask>().ToTable("Task");
            modelBuilder.Entity<UrgentTask>().ToTable("UrgentTask");
            modelBuilder.Entity<MeetingTask>().ToTable("MeetingTask");
            modelBuilder.Entity<HabitTask>().ToTable("HabitTask");
            modelBuilder.Entity<FocusTask>().ToTable("FocusTask");
            modelBuilder.Entity<ListTask>().ToTable("ListTask");
            modelBuilder.Entity<CallTask>().ToTable("CallTask");
        }

    }
}
