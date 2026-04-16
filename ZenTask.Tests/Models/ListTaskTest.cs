using ZenTask.Core.Models;

namespace ZenTask.Tests.Models
{
    public class ListTaskTest
    {
        [Fact]
        public void ListTask_Should_Not_Be_Complete_When_No_Items()
        {
            // Arrange & Act
            var task = new ListTask("Test Task");
            // Assert
            Assert.False(task.IsCompleted);
        }

        [Fact]
        public void ListTask_Should_Not_Be_Complete_When_Items_Not_Done()
        {
            // Arrange & Act
            var task = new ListTask("Test Task");
            task.AddItem("Item 1");
            task.AddItem("Item 2");
            // Assert
            Assert.False(task.IsCompleted);
        }
        [Fact]
        public void ListTask_Should_Not_Be_Complete_When_Some_Items_Not_Done()
        {
            // Arrange
            var task = new ListTask("Test Task");
            task.AddItem("Item 1");
            task.AddItem("Item 2");
            // Act
            task.Items[0].IsDone = true;
            // Assert
            Assert.False(task.IsCompleted);
        }

        [Fact]
        public void ListTask_Should_Be_Complete_When_All_Items_Done()
        {
            // Arrange
            var task = new ListTask("Test Task");
            task.AddItem("Item 1");
            task.AddItem("Item 2");
            // Act
            task.Items[0].IsDone = true;
            task.Items[1].IsDone = true;
            // Assert
            Assert.True(task.IsCompleted);
        }

        [Fact]
        public void ListTask_Complete_Method_Should_Mark_All_Items_As_Done()
        {
            // Arrange
            var task = new ListTask("Test Task");
            task.AddItem("Item 1");
            task.AddItem("Item 2");
            // Act
            task.Complete();
            // Assert
            Assert.True(task.Items.All(item => item.IsDone));
            Assert.True(task.IsCompleted);
        }
    }
}
