using ZenTask.Core.Models;

namespace ZenTask.Tests.Models
{
    public class CallTaskTest
    {
        [Fact]
        public void CallTask_Constructor_Should_Set_Title_Correctly()
        {
            // Arrange
            string title = "Test Call Task";
            var callTask = new CallTask(title, "John Doe", "123-456-7890", DateTime.Now);
            // Act & Assert
            Assert.Equal(title, callTask.Title);
        }

        [Fact]
        public void CallTask_Constructor_Should_Set_ContactName_Correctly()
        {
            // Arrange
            string contactName = "John Doe";
            var callTask = new CallTask("Test Call Task", contactName, "123-456-7890", DateTime.Now);
            // Act & Assert
            Assert.Equal(contactName, callTask.ContactName);
        }

        [Fact]
        public void CallTask_Contructor_Should_Set_PhoneNumber_Correctly()
        {
            // Arrange
            string phoneNumber = "123-456-7890";
            var callTask = new CallTask("Test Call Task", "John Doe", phoneNumber, DateTime.Now);
            // Act & Assert
            Assert.Equal(phoneNumber, callTask.PhoneNumber);
        }

        [Fact]
        public void CallTask_Constructor_Should_Set_ReminderTime_Correctly()
        {
            // Arrange
            DateTime reminderTime = DateTime.Now.AddHours(1);
            var callTask = new CallTask("Test Call Task", "John Doe", "123-456-7890", reminderTime);
            // Act & Assert
            Assert.Equal(reminderTime, callTask.ReminderTime);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        [InlineData("Zoom")]
        public void CallTask_Constructor_Should_Set_Platform_Correctly(string platform)
        {
            // Arrange
            var callTask = new CallTask("Test Call Task", "John Doe", "123-456-7890", DateTime.Now, platform);
            // Act & Assert
            string expectedPlatform = string.IsNullOrWhiteSpace(platform) ? "Phone" : platform;
            Assert.Equal(expectedPlatform, callTask.Platform);
        }

        [Fact]
        public void CallTask_Constructor_Should_Set_IsReminderSent_To_False()
        {
            // Arrange
            var callTask = new CallTask("Test Call Task", "John Doe", "123-456-7890", DateTime.Now);
            // Act & Assert
            Assert.False(callTask.IsReminderSent);
        }

        [Fact]
        public void CallTask_Constructor_Should_Set_IsCompleted_To_False()
        {
            // Arrange
            var callTask = new CallTask("Test Call Task", "John Doe", "123-456-7890", DateTime.Now);
            // Act & Assert
            Assert.False(callTask.IsCompleted);
        }

        [Fact]
        public void CallTask_Complete_Should_Set_IsCompleted_To_True()
        {
            // Arrange
            var callTask = new CallTask("Test Call Task", "John Doe", "123-456-7890", DateTime.Now);
            // Act
            callTask.Complete();
            // Assert
            Assert.True(callTask.IsCompleted);
        }

        [Fact]
        public void CallTask_UndoComplete_Should_Set_IsCompleted_To_False()
        {
            // Arrange
            var callTask = new CallTask("Test Call Task", "John Doe", "123-456-7890", DateTime.Now);
            callTask.Complete();
            // Act
            callTask.UndoComplete();
            // Assert
            Assert.False(callTask.IsCompleted);
        }
    }
}
