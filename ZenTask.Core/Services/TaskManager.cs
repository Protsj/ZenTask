using ZenTask.Core.Interfaces;
using ZenTask.Core.Models;

namespace ZenTask.Core.Services
{
    public class TaskEventArgs : EventArgs
    {
        public BaseTask Task { get; }
        public TaskEventArgs(BaseTask task) => Task = task;
    }

    public class TaskManager
    {
        private readonly List<BaseTask> _tasks;
        public event EventHandler<TaskEventArgs> TaskCompletedEvents;
        public TaskManager() => _tasks = new List<BaseTask>();
        public void AddTask(BaseTask task)
        {
            if(task == null) throw new ArgumentNullException(nameof(task));
            _tasks.Add(task);
        }
        public void RemoveTask(Guid id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task != null) 
                _tasks.Remove(task);
        }
        public List<BaseTask> GetTasks(Predicate<BaseTask> filter = null)
        {
            if(filter == null) 
                return _tasks.ToList();
            return _tasks.FindAll(filter);
        }
        public void CompleteTask(Guid id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task != null && task is ICompletable complitableTask)
            {
                if(!complitableTask.IsCompleted)
                {
                    complitableTask.Complete();
                    TaskCompletedEvents?.Invoke(this, new TaskEventArgs(task));
                }
            }
        }
    }
 
}
