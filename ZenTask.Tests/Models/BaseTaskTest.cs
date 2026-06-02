using ZenTask.Core.Models;

namespace ZenTask.Tests.Models
{
    public class BaseTaskTest
    {
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void BaseTask_Constructor_Should_Throw_Exception_When_Title_Is_Empty_Or_Null_Or_Whitespace(string title)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new HabitTask(title));
        }
        [Fact]
        public void BaseTask_Constructor_Should_Set_Id_Different_For_Each_Instance()
        {
            // Arrange
            var task1 = new HabitTask("Task 1");
            var task2 = new HabitTask("Task 2");
            var task3 = new HabitTask("Task 3");
            // Act & Assert
            Assert.NotEqual(task1.Id, task2.Id);
            Assert.NotEqual(task1.Id, task3.Id);
            Assert.NotEqual(task2.Id, task3.Id);
        }
        [Fact]
        public void BaseTask_Constructor_Should_Set_Title_Correctly()
        {
            // Arrange
            string title = "Test Task";
            var task = new HabitTask(title);
            // Act & Assert
            Assert.Equal(title, task.Title);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        [InlineData("This is a description.")]
        public void BaseTask_Constructor_Should_Set_Description_Correctly(string description)
        {
            // Arrange
            var task = new HabitTask("Test Task", description);
            // Act & Assert
            Assert.Equal(string.IsNullOrWhiteSpace(description) ? string.Empty : description, task.Description);
        }

        [Fact]
        public void BaseTask_Constructor_Should_Set_CreatedAt_To_Current_Time()
        {
            // Arrange
            var beforeCreation = DateTime.UtcNow;
            var task = new HabitTask("Test Task");
            var afterCreation = DateTime.UtcNow;
            // Act & Assert
            Assert.True(task.CreatedAt >= beforeCreation && task.CreatedAt <= afterCreation);
        }
    }
}
