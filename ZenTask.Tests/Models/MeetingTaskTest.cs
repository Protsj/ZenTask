using ZenTask.Core.Models;

namespace ZenTask.Tests.Models
{
    public class MeetingTaskTest
    {
        [Fact]
        public void MeetingTask_Constructor_Should_Set_Title_Correctly()
        {
            // Arrange
            string title = "Test Meeting Task";
            var task = new MeetingTask(title, DateTime.UtcNow.AddHours(1));
            // Act & Assert
            Assert.Equal(title, task.Title);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        [InlineData("Office")]
        public void MeetingTask_Constructor_Should_Set_Location_Correctly(string location)
        {
            // Arrange
            var task = new MeetingTask("Test Meeting Task", DateTime.UtcNow.AddHours(1), location);
            // Act & Assert
            string expectedLocation = string.IsNullOrWhiteSpace(location) ? string.Empty : location;
            Assert.Equal(expectedLocation, task.Location);
        }

        [Fact]
        public void MeetingTask_Constructor_Should_Set_ReminderTime_Correctly()
        {
            // Arrange
            DateTime reminderTime = DateTime.UtcNow.AddHours(1);
            var task = new MeetingTask("Test Meeting Task", reminderTime);
            // Act & Assert
            Assert.Equal(reminderTime, task.ReminderTime);
        }

        [Fact]
        public void MeetingTask_Constructor_Should_Set_IsReminderSent_To_False()
        {
            // Arrange
            var task = new MeetingTask("Test Meeting Task", DateTime.UtcNow.AddHours(1));
            // Act & Assert
            Assert.False(task.IsReminderSent);
        }

        [Fact]
        public void MeetingTask_Constructor_Should_Set_IsCompleted_To_False()
        {
            // Arrange
            var task = new MeetingTask("Test Meeting Task", DateTime.UtcNow.AddHours(1));
            // Act & Assert
            Assert.False(task.IsCompleted);
        }

        [Fact]
        public void MeetingTask_Complete_Should_Set_IsCompleted_To_True()
        {
            // Arrange
            var task = new MeetingTask("Test Meeting Task", DateTime.UtcNow.AddHours(1));
            // Act
            task.Complete();
            // Assert
            Assert.True(task.IsCompleted);
        }

        [Fact]
        public void MeetingTask_UndoComplete_Should_Set_IsCompleted_To_False()
        {
            // Arrange
            var task = new MeetingTask("Test Meeting Task", DateTime.UtcNow.AddHours(1));
            task.Complete();
            // Act
            task.UndoComplete();
            // Assert
            Assert.False(task.IsCompleted);
        }
    }
}
