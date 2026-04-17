using ZenTask.Core.Data;
using ZenTask.Core.Models;

namespace ZenTask.Tests.Data
{
    public class FileTaskStorageTests
    {
        [Fact]
        public async Task Save_And_Load_Async_Should_Preserve_Polymorphism()
        {
            // Arrange
            var testFile = $"{Guid.NewGuid()}.json";
            var storage = new FileTaskStorage(testFile);
            var originalTasks = new List<BaseTask>
            {
                new HabitTask("Morning Exercise"),
                new ListTask("Grocery Shopping")
            };
            try
            {
                // Act
                await storage.SaveAsync(originalTasks);
                var loadedTasks = await storage.LoadAsync();
                // Assert
                Assert.Equal(originalTasks.Count, loadedTasks.Count);
                Assert.IsType<HabitTask>(loadedTasks[0]);
                Assert.IsType<ListTask>(loadedTasks[1]);
                Assert.Equal(originalTasks[0].Title, loadedTasks[0].Title);
                Assert.Equal(originalTasks[1].Title, loadedTasks[1].Title);
            }
            finally
            {
                // Clean up test file
                if (File.Exists(testFile))
                    File.Delete(testFile);
            }
        }
        [Fact]
        public async Task Load_Async_When_File_Does_Not_Exist_Should_Return_Empty_List()
        {
            // Arrange
            var testFile = $"{Guid.NewGuid()}.json";
            var storage = new FileTaskStorage(testFile);
            // Act
            var loadedTasks = await storage.LoadAsync();
            // Assert
            Assert.NotNull(loadedTasks);
            Assert.Empty(loadedTasks);
        }
        [Fact]
        public async Task Load_Async_When_File_Is_Corrupted_Should_Return_Empty_List()
        {
            //Arrange
            var testFile = $"{Guid.NewGuid()}.json";
            var storage = new FileTaskStorage(testFile);
            await File.WriteAllTextAsync(testFile, "This is not valid JSON");
            try
            {
                // Act
                var loadedTasks = await storage.LoadAsync();
                // Assert
                Assert.NotNull(loadedTasks);
                Assert.Empty(loadedTasks);
            }
            finally
            {
                // Clean up test file
                if (File.Exists(testFile))
                    File.Delete(testFile);
            }
        }
    }
}
