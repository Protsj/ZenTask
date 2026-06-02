using ZenTask.Core.Models;

namespace ZenTask.Tests.Models
{
    public class FocusTaskTest
    {
        [Fact]
        public void FocusTask_Constructor_Should_Set_Title_Correctly()
        {
            // Arrange
            string title = "Test Focus Task";
            // Act
            var task = new FocusTask(title, TimeSpan.FromMinutes(25));
            // Assert
            Assert.Equal(title, task.Title);
        }

        [Fact]
        public void FocusTask_Constructor_Should_Set_EstimatedDuration_Correctly()
        {
            // Arrange
            TimeSpan estimatedDuration = TimeSpan.FromMinutes(25);
            // Act
            var task = new FocusTask("Test Focus Task", estimatedDuration);
            // Assert
            Assert.Equal(estimatedDuration, task.EstimatedDuration);
        }

        [Fact]
        public void FocusTask_Constructor_Should_Set_PomodoroCount_Correctly()
        {
            // Arrange
            int pomodoroCount = 4;
            // Act
            var task = new FocusTask("Test Focus Task", TimeSpan.FromMinutes(25), pomodoroCount);
            // Assert
            Assert.Equal(pomodoroCount, task.PomodoroCount);
        }

        [Fact]
        public void FocusTask_Constructor_Should_Set_IsCompleted_To_False()
        {
            // Arrange
            var task = new FocusTask("Test Focus Task", TimeSpan.FromMinutes(25));
            // Act & Assert
            Assert.False(task.IsCompleted);
        }

        [Fact]
        public void FocusTask_Complete_Should_Set_IsCompleted_To_True()
        {
            // Arrange
            var task = new FocusTask("Test Focus Task", TimeSpan.FromMinutes(25));
            // Act
            task.Complete();
            // Assert
            Assert.True(task.IsCompleted);
        }

        [Fact]
        public void FocusTask_UndoComplete_Should_Set_IsCompleted_To_False()
        {
            // Arrange
            var task = new FocusTask("Test Focus Task", TimeSpan.FromMinutes(25));
            task.Complete();
            // Act
            task.UndoComplete();
            // Assert
            Assert.False(task.IsCompleted);
        }
    }
}
