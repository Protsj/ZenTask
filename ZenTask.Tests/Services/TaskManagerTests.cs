using ZenTask.Core.Models;
using ZenTask.Core.Services;

namespace ZenTask.Tests.Services
{
    public class TaskManagerTests
    {
        [Fact]
        public void AddTask_Should_Throw_Exception_When_Task_Is_Null()
        {
            // Arrange
            var manager = new TaskManager();
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => manager.AddTask(null));
        }
        [Fact]
        public void AddTask_Should_Add_Task_To_Manager()
        {
            // Arrange
            var manager = new TaskManager();
            var task = new UrgentTask("Test Task", DateTime.Now.AddDays(1));
            // Act
            manager.AddTask(task);
            var tasks = manager.GetTasks();
            // Assert
            Assert.Single(tasks);
            Assert.Equal("Test Task", tasks[0].Title);
        }

        [Fact]
        public void RemoveTask_Should_Remove_Task_From_Manager_When_Task_Exists()
        {
            // Arrange
            var manager = new TaskManager();
            var task = new UrgentTask("Test Task", DateTime.Now.AddDays(1));
            manager.AddTask(task);
            // Act
            manager.RemoveTask(task.Id);
            var tasks = manager.GetTasks();
            // Assert
            Assert.Empty(tasks);
        }
        [Fact]
        public void RemoveTask_Should_Do_Nothing_When_Task_Does_Not_Exist()
        {
            // Arrange
            var manager = new TaskManager();
            var task = new UrgentTask("Test Task", DateTime.Now.AddDays(1));
            manager.AddTask(task);
            // Act
            manager.RemoveTask(Guid.NewGuid());
            var tasks = manager.GetTasks();
            // Assert
            Assert.Single(tasks);
        }
        [Fact]
        public void GetTasks_With_Predicate_Should_Return_Only_Matching_Tasks()
        {
            // Arrange
            var manager = new TaskManager();
            manager.AddTask(new HabitTask("Habit 1"));
            manager.AddTask(new CallTask("Call 1", "Ivan", DateTime.Now));
            manager.AddTask(new HabitTask("Habit 2"));
            // Act
            var habitTasks = manager.GetTasks(t => t is HabitTask);
            // Assert
            Assert.Equal(2, habitTasks.Count);
            Assert.All(habitTasks, t => Assert.IsType<HabitTask>(t));
        }
        [Fact]
        public void CompleteTask_Should_Mark_Task_As_Completed_And_Raise_Event()
        {
            // Arrange
            var manager = new TaskManager();
            var task = new HabitTask("Test Habit Task");
            manager.AddTask(task);
            bool eventRaised = false;
            Guid raisedTaskId = Guid.Empty;
            manager.TaskCompletedEvents += (sender, args) =>
            {
                eventRaised = true;
                raisedTaskId = args.Task.Id;
            };
            // Act
            manager.CompleteTask(task.Id);
            // Assert
            Assert.True(task.IsCompleted);
            Assert.True(eventRaised);
            Assert.Equal(task.Id, raisedTaskId);
        }
        [Fact]
        public void CompleteTask_Should_Do_Nothing_When_Task_Is_Already_Completed()
        {
            // Arrange
            var manager = new TaskManager();
            var task = new HabitTask("Test Habit Task");
            manager.AddTask(task);
            manager.CompleteTask(task.Id); // First completion
            bool eventRaised = false;
            manager.TaskCompletedEvents += (sender, args) => eventRaised = true;
            // Act
            manager.CompleteTask(task.Id); // Attempt to complete again
            // Assert
            Assert.True(task.IsCompleted);
            Assert.False(eventRaised); // Event should not be raised again
        }
    }
}
