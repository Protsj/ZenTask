using System.Text.Json;
using ZenTask.Core.Interfaces;
using ZenTask.Core.Models;

namespace ZenTask.Core.Data
{
    public class FileTaskStorage : ITaskStorage
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public FileTaskStorage(string filePath = "tasks.json")
        {
            _filePath = filePath;
            _jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        }

        public async Task SaveAsync(IEnumerable<BaseTask> tasks)
        {
            var json = JsonSerializer.Serialize(tasks, _jsonOptions);
            await File.WriteAllTextAsync(_filePath, json);
        }

        public async Task<List<BaseTask>> LoadAsync()
        {
            if (!File.Exists(_filePath))
                return new List<BaseTask>();
            try
            {
                var json = await File.ReadAllTextAsync(_filePath);
                return JsonSerializer.Deserialize<List<BaseTask>>(json) ?? new List<BaseTask>();
            }
            catch
            {
                Console.WriteLine($"\nError loading tasks from {_filePath}.");
                return new List<BaseTask>();
            }
        }
    }
}