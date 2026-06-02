using ZenTask.Core.Models;

namespace ZenTask.Tests.Models
{
    public class ListTaskTest
    {
        [Fact]
        public void ListTask_Constructor_Should_Set_Title_Correctly()
        {
            // Arrange
            string title = "Test List Task";
            // Act
            var task = new ListTask(title);
            // Assert
            Assert.Equal(title, task.Title);
        }

        [Fact]
        public void ListTask_Should_Have_Empty_Items_List_On_Creation()
        {
            // Arrange
            var task = new ListTask("Test List Task");
            // Act & Assert
            Assert.NotNull(task.Items);
            Assert.Empty(task.Items);
        }

        [Fact]
        public void ListTask_Complete_Should_Set_IsDone_To_True()
        {
            // Arrange
            var task = new ListTask("Test List Task");
            task.AddItem("Item 1");
            // Act
            task.Complete();
            // Assert
            Assert.True(task.IsCompleted);
        }

        [Fact]
        public void ListTask_Complete_Should_Mark_All_Items_IsDone_To_True()
        {
            // Arrange
            var task = new ListTask("Test List Task");
            task.AddItem("Item 1");
            task.AddItem("Item 2");
            // Act
            task.Complete();
            // Assert
            Assert.All(task.Items, item => Assert.True(item.IsDone));
        }

        [Fact]
        public void ListTask_UndoComplete_Should_Set_IsCompleted_To_False()
        {
            // Arrange
            var task = new ListTask("Test List Task");
            task.AddItem("Item 1");
            task.Complete();
            // Act
            task.UndoComplete();
            // Assert
            Assert.False(task.IsCompleted);
        }

        [Fact]
        public void ListTask_UndoComplete_Should_Mark_All_Items_IsDone_To_False()
        {
            // Arrange
            var task = new ListTask("Test List Task");
            task.AddItem("Item 1");
            task.AddItem("Item 2");
            task.Complete();
            // Act
            task.UndoComplete();
            // Assert
            Assert.All(task.Items, item => Assert.False(item.IsDone));
        }

        [Fact]
        public void ListTask_Should_Not_Be_Complete_When_No_Items()
        {
            // Arrange 
            var task = new ListTask("Test List Task");
            // Act & Assert
            Assert.False(task.IsCompleted);
        }

        [Fact]
        public void ListTask_Should_Not_Be_Complete_When_Items_Not_Done()
        {
            // Arrange & Act
            var task = new ListTask("Test List Task");
            task.AddItem("Item 1");
            task.AddItem("Item 2");
            // Assert
            Assert.False(task.IsCompleted);
        }

        [Fact]
        public void ListTask_Should_Not_Be_Complete_When_Some_Items_Not_Done()
        {
            // Arrange
            var task = new ListTask("Test List Task");
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
            var task = new ListTask("Test List Task");
            task.AddItem("Item 1");
            task.AddItem("Item 2");
            // Act
            task.Items[0].IsDone = true;
            task.Items[1].IsDone = true;
            // Assert
            Assert.True(task.IsCompleted);
        }
    }
}
