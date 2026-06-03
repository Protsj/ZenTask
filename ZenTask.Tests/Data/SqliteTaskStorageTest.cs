using ZenTask.Core.Data;
using ZenTask.Core.Models;

namespace ZenTask.Tests.Data
{
    public class SqliteTaskStorageTest
    {
        [Fact]
        public async Task SqliteTaskStorage_SaveTaskAsync_Should_Save_Tasks()
        {
            using (var context = new TaskDbContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
            // Arrange
            var storage = new SqliteTaskStorage();
            var tasks = new List<BaseTask>
            {
                new CallTask("Call John", "John Doe", "1234567890", DateTime.Now.AddHours(1)),
                new CallTask("Call Jane", "Jane Doe", "0987654321", DateTime.Now.AddHours(2))
            };
            // Act
            await storage.SaveTaskAsync(tasks);
            var loadedTasks = await storage.LoadTasksAsync();
            // Assert
            Assert.Equal(2, loadedTasks.Count);
            Assert.Contains(loadedTasks, t => t.Title == "Call John");
            Assert.Contains(loadedTasks, t => t.Title == "Call Jane");
        }

        [Fact]
        public async Task SqliteTaskStorage_SaveTaskAsync_Should_Update_Existing_Task()
        {
            using (var context = new TaskDbContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
            // Arrange
            var storage = new SqliteTaskStorage();
            var task = new CallTask("Call John", "John Doe", "1234567890", DateTime.Now.AddHours(1));
            await storage.SaveTaskAsync(new List<BaseTask> { task });
            // Act
            task.Title = "Call John Updated";
            await storage.SaveTaskAsync(new List<BaseTask> { task });
            var loadedTasks = await storage.LoadTasksAsync();
            // Assert
            Assert.Single(loadedTasks);
            Assert.Equal("Call John Updated", loadedTasks[0].Title);
        }

        [Fact]
        public async Task SqliteTaskStorage_SaveTaskAsync_Should_Handle_Null_Tasks()
        {
            using (var context = new TaskDbContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
            // Arrange
            var storage = new SqliteTaskStorage();
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => storage.SaveTaskAsync(null));
        }

        [Fact]
        public async Task SqliteTaskStorage_LoadTasksAsync_Should_Load_Tasks()
        {
            using (var context = new TaskDbContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
            // Arrange
            var storage = new SqliteTaskStorage();
            var tasks = new List<BaseTask>
            {
                new CallTask("Call John", "John Doe", "1234567890", DateTime.Now.AddHours(1)),
                new CallTask("Call Jane", "Jane Doe", "0987654321", DateTime.Now.AddHours(2))
            };
            await storage.SaveTaskAsync(tasks);
            // Act
            var loadedTasks = await storage.LoadTasksAsync();
            // Assert
            Assert.Equal(2, loadedTasks.Count);
            Assert.Contains(loadedTasks, t => t.Title == "Call John");
            Assert.Contains(loadedTasks, t => t.Title == "Call Jane");
        }

        [Fact]
        public async Task SqliteTaskStorage_DeleteTaskAsync_Should_Delete_Task()
        {
            using (var context = new TaskDbContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
            // Arrange
            var storage = new SqliteTaskStorage();
            var task = new CallTask("Call John", "John Doe", "1234567890", DateTime.Now.AddHours(1));
            await storage.SaveTaskAsync(new List<BaseTask> { task });
            // Act
            await storage.DeleteTaskAsync(task.Id);
            var loadedTasks = await storage.LoadTasksAsync();
            // Assert
            Assert.Empty(loadedTasks);
        }

        [Fact]
        public async Task SqliteTaskStorage_DeleteTaskAsync_Should_Handle_NonExistent_Task()
        {
            using (var context = new TaskDbContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
            // Arrange
            var storage = new SqliteTaskStorage();
            var nonExistentId = Guid.NewGuid();
            // Act & Assert
            await storage.DeleteTaskAsync(nonExistentId);
        }

        [Fact]
        public async Task SqliteTaskStorage_LoadTasksAsync_Should_Return_Empty_List_When_No_Tasks()
        {
            using (var context = new TaskDbContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
            // Arrange
            var storage = new SqliteTaskStorage();
            // Act
            var loadedTasks = await storage.LoadTasksAsync();
            // Assert
            Assert.Empty(loadedTasks);
        }
    }
}