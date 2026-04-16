using ZenTask.Core.Models;

namespace ZenTask.Tests.Models
{
    public class HabitTaskTest
    {
        [Fact]
        public void HabitTask_Should_Increase_Streak_On_Complete()
        {
            // Arrange
            var task = new HabitTask("Test Habit Task");
            int initialStreak = task.Streak;
            // Act
            task.Complete();
            // Assert
            Assert.True(task.IsCompleted);
            Assert.Equal(initialStreak + 1, task.Streak);
        }
        [Fact]
        public void HabitTask_When_Completed_Multiple_Times_Should_Increase_Streak_Each_Time()
        {
            // Arrange
            var task = new HabitTask("Test Habit Task");
            int initialStreak = task.Streak;
            // Act
            task.Complete(); // First completion
            task.ResetCycle(); // Reset cycle to allow for next completion
            task.Complete(); // Second completion
            // Assert
            Assert.True(task.IsCompleted);
            Assert.Equal(initialStreak + 2, task.Streak); // Streak should increase by 2
        }
        [Fact]
        public void HabitTask_Should_Reset_Streak_On_ResetCycle()
        {
            // Arrange
            var task = new HabitTask("Test Habit Task");
            task.Complete(); // Increase streak
            // Act
            task.ResetCycle();
            task.ResetCycle();
            // Assert
            Assert.False(task.IsCompleted);
            Assert.Equal(0, task.Streak); // Streak should reset to 0
        }

        [Fact]
        public void HabitTask_Should_Not_Increase_Streak_When_Already_Completed()
        {
            // Arrange
            var task = new HabitTask("Test Habit Task");
            task.Complete(); // First completion
            int streakAfterFirstComplete = task.Streak;
            // Act
            task.Complete(); // Attempt to complete again
            // Assert
            Assert.True(task.IsCompleted);
            Assert.Equal(streakAfterFirstComplete, task.Streak); // Streak should not increase
        }

    }
}
