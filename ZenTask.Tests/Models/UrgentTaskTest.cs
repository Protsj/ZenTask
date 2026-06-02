using ZenTask.Core.Models;

namespace ZenTask.Tests.Models
{
    public class UrgentTaskTest
    {
        [Fact]
        public void UrgentTask_Constructor_Should_Set_Title_Correctly()
        {
            // Arrange
            string title = "Test Urgent Task";
            // Act
            var urgentTask = new UrgentTask(title, DateTime.Now.AddHours(1));
            // Assert
            Assert.Equal(title, urgentTask.Title);
        }

        [Fact]
        public void UrgentTask_Constructor_Should_Set_Deadline_Correctly()
        {
            // Arrange
            DateTime deadline = DateTime.Now.AddHours(1);
            // Act
            var urgentTask = new UrgentTask("Test Urgent Task", deadline);
            // Assert
            Assert.Equal(deadline, urgentTask.Deadline);
        }

        [Fact]
        public void UrgentTask_Constructor_Should_Set_IsCompleted_To_False()
        {
            // Act
            var urgentTask = new UrgentTask("Test Urgent Task", DateTime.Now.AddHours(1));
            // Assert
            Assert.False(urgentTask.IsCompleted);
        }

        [Fact]
        public void UrgentTask_Complete_Should_Set_IsCompleted_To_True()
        {
            // Arrange
            var urgentTask = new UrgentTask("Test Urgent Task", DateTime.Now.AddHours(1));
            // Act
            urgentTask.Complete();
            // Assert
            Assert.True(urgentTask.IsCompleted);
        }

        [Fact]
        public void UrgentTask_UndoComplete_Should_Set_IsCompleted_To_False()
        {
            // Arrange
            var urgentTask = new UrgentTask("Test Urgent Task", DateTime.Now.AddHours(1));
            urgentTask.Complete();
            // Act
            urgentTask.UndoComplete();
            // Assert
            Assert.False(urgentTask.IsCompleted);
        }
    }
}
