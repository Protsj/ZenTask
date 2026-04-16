using ZenTask.Core.Models;

namespace ZenTask.Tests.Models
{
    public class BaseTaskTest
    {
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void BaseTask_Contructor_When_Title_Is_Empty_Or_Null_Or_Whitespace_Should_Throw_Exception(string title)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new HabitTask(title));
        }
        [Fact]
        public void BaseTask_Constructor_Should_Set_Properties_Correctly()
        {
            // Arrange
            string title = "Test Task";
            string description = "This is a test task.";
            // Act
            var task = new HabitTask(title, description);
            // Assert
            Assert.Equal(title, task.Title);
            Assert.Equal(description, task.Description);
            Assert.NotEqual(Guid.Empty, task.Id); // Id should be set to a new Guid
            Assert.True((DateTime.UtcNow - task.CreatedAt).TotalSeconds < 5); // CreatedAt should be recent
        }
    }
}
