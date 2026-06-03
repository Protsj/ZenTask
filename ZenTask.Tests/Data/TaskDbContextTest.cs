using Microsoft.EntityFrameworkCore;
using ZenTask.Core.Data;

namespace ZenTask.Tests.Data
{
    public class TaskDbContextTest
    {
        [Fact]
        public void TaskDbContext_Should_Have_Tasks_DbSet()
        {
            using var context = new TaskDbContext();
            Assert.NotNull(context.Tasks);
        }

        [Fact]
        public void TaskDbContext_OnConfiguring_ShouldUseSqlite()
        {
            using var context = new TaskDbContext();
            var options = context.Database.GetDbConnection().ConnectionString;
            Assert.Contains("Data Source=Tasks.db", options);
        }

        [Fact]
        public void TaskDbContext_OnConfiguring_Data_Source_Should_Be_TasksDb()
        {
            using var context = new TaskDbContext();
            var options = context.Database.GetDbConnection().ConnectionString;
            Assert.Contains("Data Source=Tasks.db", options);
        }

        [Fact]
        public void TaskDbContext_OnModelCreating_Should_Configure_Entity_Mappings()
        {
            using var context = new TaskDbContext();
            var model = context.Model;
            Assert.NotNull(model.FindEntityType(typeof(ZenTask.Core.Models.BaseTask)));
            Assert.NotNull(model.FindEntityType(typeof(ZenTask.Core.Models.UrgentTask)));
            Assert.NotNull(model.FindEntityType(typeof(ZenTask.Core.Models.MeetingTask)));
            Assert.NotNull(model.FindEntityType(typeof(ZenTask.Core.Models.HabitTask)));
            Assert.NotNull(model.FindEntityType(typeof(ZenTask.Core.Models.FocusTask)));
            Assert.NotNull(model.FindEntityType(typeof(ZenTask.Core.Models.ListTask)));
            Assert.NotNull(model.FindEntityType(typeof(ZenTask.Core.Models.CallTask)));
        }

        [Fact]
        public void TaskDbContext_OnModelCreating_Should_Map_BaseTask_To_Task_Table()
        {
            using var context = new TaskDbContext();
            var model = context.Model;
            var baseTaskEntity = model.FindEntityType(typeof(ZenTask.Core.Models.BaseTask));
            Assert.NotNull(baseTaskEntity);
            Assert.Equal("Task", baseTaskEntity.GetTableName());
        }

        [Fact]
        public void TaskDbContext_OnModelCreating_Should_Map_CallTask_To_CallTask_Table()
        {
            using var context = new TaskDbContext();
            var model = context.Model;
            var callTaskEntity = model.FindEntityType(typeof(ZenTask.Core.Models.CallTask));
            Assert.NotNull(callTaskEntity);
            Assert.Equal("CallTask", callTaskEntity.GetTableName());
        }

        [Fact]
        public void TaskDbContext_OnModelCreating_Should_Map_FocusTask_To_FocusTask_Table()
        {
            using var context = new TaskDbContext();
            var model = context.Model;
            var focusTaskEntity = model.FindEntityType(typeof(ZenTask.Core.Models.FocusTask));
            Assert.NotNull(focusTaskEntity);
            Assert.Equal("FocusTask", focusTaskEntity.GetTableName());
        }

        [Fact]
        public void TaskDbContext_OnModelCreating_Should_Map_HabitTask_To_HabitTask_Table()
        {
            using var context = new TaskDbContext();
            var model = context.Model;
            var habitTaskEntity = model.FindEntityType(typeof(ZenTask.Core.Models.HabitTask));
            Assert.NotNull(habitTaskEntity);
            Assert.Equal("HabitTask", habitTaskEntity.GetTableName());
        }

        [Fact]
        public void TaskDbContext_OnModelCreating_Should_Map_ListTask_To_ListTask_Table()
        {
            using var context = new TaskDbContext();
            var model = context.Model;
            var listTaskEntity = model.FindEntityType(typeof(ZenTask.Core.Models.ListTask));
            Assert.NotNull(listTaskEntity);
            Assert.Equal("ListTask", listTaskEntity.GetTableName());
        }

        [Fact]
        public void TaskDbContext_OnModelCreating_Should_Map_MeetingTask_To_MeetingTask_Table()
        {
            using var context = new TaskDbContext();
            var model = context.Model;
            var meetingTaskEntity = model.FindEntityType(typeof(ZenTask.Core.Models.MeetingTask));
            Assert.NotNull(meetingTaskEntity);
            Assert.Equal("MeetingTask", meetingTaskEntity.GetTableName());
        }

        [Fact]
        public void TaskDbContext_OnModelCreating_Should_Map_UrgentTask_To_UrgentTask_Table()
        {
            using var context = new TaskDbContext();
            var model = context.Model;
            var urgentTaskEntity = model.FindEntityType(typeof(ZenTask.Core.Models.UrgentTask));
            Assert.NotNull(urgentTaskEntity);
            Assert.Equal("UrgentTask", urgentTaskEntity.GetTableName());
        }
    }
}
