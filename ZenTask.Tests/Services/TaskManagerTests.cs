using ZenTask.Core.Models;
using ZenTask.Core.Services;

namespace ZenTask.Tests.Services
{
    public class TaskManagerTests
    {
        [Fact]
        public void TaskManager_Constructor_Should_Initialize_Task_List()
        {
            // Arrange & Act
            var manager = new TaskManager();
            // Assert
            Assert.NotNull(manager.GetTasks());
            Assert.Empty(manager.GetTasks());
        }

        [Fact]
        public void TaskManager_AddTask_Should_Add_Task_To_Manager()
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
        public void TaskManager_AddTask_Should_Throw_Exception_When_Task_Is_Null()
        {
            // Arrange
            var manager = new TaskManager();
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => manager.AddTask(null));
        }

        [Fact]
        public void TaskManager_RemoveTask_Should_Remove_Task_From_Manager_When_Task_Exists()
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
        public void TaskManager_GetTasks_With_Predicate_Should_Return_Only_Matching_Tasks()
        {
            // Arrange
            var manager = new TaskManager();
            manager.AddTask(new HabitTask("Habit 1"));
            manager.AddTask(new CallTask("Call Task", "John Doe", "123-456-7890", DateTime.Now.AddHours(1)));
            manager.AddTask(new HabitTask("Habit 2"));
            // Act
            var habitTasks = manager.GetTasks(t => t is HabitTask);
            // Assert
            Assert.Equal(2, habitTasks.Count);
            Assert.All(habitTasks, t => Assert.IsType<HabitTask>(t));
        }

        [Fact]
        public void TaskManager_GetTasks_Should_Return_All_Tasks_When_No_Filter_Is_Provided()
        {
            // Arrange
            var manager = new TaskManager();
            manager.AddTask(new HabitTask("Habit 1"));
            manager.AddTask(new CallTask("Call Task", "John Doe", "123-456-7890", DateTime.Now.AddHours(1)));
            // Act
            var tasks = manager.GetTasks();
            // Assert
            Assert.Equal(2, tasks.Count);
        }

        [Fact]
        public void TaskManager_CompleteTask_Should_Mark_Task_As_Completed_And_Raise_Event()
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
        public void TaskManager_CompleteTask_Should_Do_Nothing_When_Task_Is_Already_Completed()
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

        [Fact]
        public void TaskManager_CompleteTask_Should_Do_Nothing_When_Task_Does_Not_Exist()
        {
            // Arrange
            var manager = new TaskManager();
            bool eventRaised = false;
            manager.TaskCompletedEvents += (sender, args) => eventRaised = true;
            // Act
            manager.CompleteTask(Guid.NewGuid()); // Attempt to complete non-existent task
            // Assert
            Assert.False(eventRaised); // Event should not be raised
        }

        [Fact]
        public void TaskManager_StartReminderMonitor_Should_Trigger_Reminder_Event_When_Reminder_Time_Is_Reached()
        {
            // Arrange
            var manager = new TaskManager();
            var task = new CallTask("Call Task", "John Doe", "123-456-7890", DateTime.Now.AddSeconds(-1));
            manager.AddTask(task);
            bool eventRaised = false;
            Guid raisedTaskId = Guid.Empty;
            manager.ReminderTriggeredEvent += (sender, args) =>
            {
                eventRaised = true;
                raisedTaskId = args.Task.Id;
            };
            try
            {
                // Act
                manager.StartReminderMonitor();
                Thread.Sleep(100); // Wait for reminder to trigger
                // Assert
                Assert.True(eventRaised);
                Assert.Equal(task.Id, raisedTaskId);
            }
            finally
            {
                manager.StopReminderMonitor(); // Ensure monitor is stopped after test
            }
        }

        [Fact]
        public void TaskManager_StartReminderMonitor_Should_Not_Trigger_Reminder_Event_For_Tasks_With_Future_Reminders()
        {
            // Arrange
            var manager = new TaskManager();
            var task = new CallTask("Call Task", "John Doe", "123-456-7890", DateTime.Now.AddMinutes(5));
            manager.AddTask(task);
            bool eventRaised = false;
            manager.ReminderTriggeredEvent += (sender, args) => eventRaised = true;
            try
            {
                // Act
                manager.StartReminderMonitor();
                Thread.Sleep(100); // Wait to see if any reminder triggers
                // Assert
                Assert.False(eventRaised); // No reminder should be triggered yet
            }
            finally
            {
                manager.StopReminderMonitor(); // Ensure monitor is stopped after test
            }
        }

        [Fact]
        public void TaskManager_StartReminderMonitor_Should_Not_Trigger_Reminder_Event_For_Completed_Tasks()
        {
            // Arrange
            var manager = new TaskManager();
            var task = new CallTask("Call Task", "John Doe", "123-456-7890", DateTime.Now.AddSeconds(1));
            manager.AddTask(task);
            manager.CompleteTask(task.Id); // Mark task as completed
            bool eventRaised = false;
            manager.ReminderTriggeredEvent += (sender, args) => eventRaised = true;
            try
            {
                // Act
                manager.StartReminderMonitor();
                Thread.Sleep(100); // Wait to see if any reminder triggers
                // Assert
                Assert.False(eventRaised); // No reminder should be triggered for completed tasks
            }
            finally
            {
                manager.StopReminderMonitor(); // Ensure monitor is stopped after test
            }
        }

        [Fact]
        public void TaskManager_StartReminderMonitor_Should_Not_Trigger_Reminder_Event_For_Tasks_Without_Reminders()
        {
            // Arrange
            var manager = new TaskManager();
            var task = new HabitTask("Test Habit Task");
            manager.AddTask(task);
            bool eventRaised = false;
            manager.ReminderTriggeredEvent += (sender, args) => eventRaised = true;
            try
            {
                // Act
                manager.StartReminderMonitor();
                Thread.Sleep(100); // Wait to see if any reminder triggers
                // Assert
                Assert.False(eventRaised); // No reminder should be triggered
            }
            finally
            {
                manager.StopReminderMonitor(); // Ensure monitor is stopped after test
            }
        }

        [Fact]
        public void TaskManager_StartReminderMonitor_Should_Not_Start_Multiple_Monitors()
        {
            // Arrange
            var manager = new TaskManager();
            var task = new CallTask("Call Task", "John Doe", "123-456-7890", DateTime.Now.AddSeconds(-1));
            manager.AddTask(task);
            int eventTriggerCount = 0;
            manager.ReminderTriggeredEvent += (sender, args) => eventTriggerCount++;
            try
            {
                // Act
                manager.StartReminderMonitor();
                manager.StartReminderMonitor(); // Attempt to start monitor again
                Thread.Sleep(100); // Wait for reminder to trigger
                // Assert
                Assert.Equal(1, eventTriggerCount); // Reminder should only trigger once
            }
            finally
            {
                manager.StopReminderMonitor(); // Ensure monitor is stopped after test
            }
        }

        [Fact]
        public void TaskManager_StopReminderMonitor_Should_Stop_Monitor_Successfully()
        {
            // Arrange
            var manager = new TaskManager();
            var task = new CallTask("Call Task", "John Doe", "123-456-7890", DateTime.Now.AddSeconds(1));
            manager.AddTask(task);
            bool eventRaised = false;
            manager.ReminderTriggeredEvent += (sender, args) => eventRaised = true;
            manager.StartReminderMonitor();
            // Act
            manager.StopReminderMonitor();
            Thread.Sleep(100); // Wait to see if any reminder triggers
            // Assert
            Assert.False(eventRaised); // No reminder should be triggered after monitor is stopped
        }

        [Fact]
        public void TaskManager_StopReminderMonitor_Should_Not_Stop_Monitor_If_Not_Started()
        {
            // Arrange
            var manager = new TaskManager();
            // Act
            manager.StopReminderMonitor(); // Attempt to stop monitor when it's not started
            // Assert
            Assert.True(true); // No exception should be thrown
        }
    }
}
