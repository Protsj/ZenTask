using ZenTask.Core.Models;

namespace ZenTask.Core.Interfaces
{
    public interface ITaskStorage
    {
        Task SaveAsync(IEnumerable<BaseTask> tasks);
        Task<List<BaseTask>> LoadAsync();
    }
}
