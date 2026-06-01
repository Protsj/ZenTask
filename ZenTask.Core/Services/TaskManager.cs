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
        private bool _isMonitoring = false;
        public event EventHandler<TaskEventArgs> TaskCompletedEvents;
        public event EventHandler<TaskEventArgs> ReminderTriggeredEvent;
        public TaskManager() => _tasks = new List<BaseTask>();
        public void AddTask(BaseTask task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
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
            if (filter == null)
                return _tasks.ToList();
            return _tasks.FindAll(filter);
        }
        public void CompleteTask(Guid id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task != null && task is ICompletable complitableTask)
            {
                if (!complitableTask.IsCompleted)
                {
                    complitableTask.Complete();
                    TaskCompletedEvents?.Invoke(this, new TaskEventArgs(task));
                }
            }
        }

        public void StartReminderMonitor()
        {
            if (_isMonitoring) return;
            _isMonitoring = true;
            Task.Run(async () =>
            {
                while (_isMonitoring)
                {
                    var currentTasks = GetTasks().ToList();
                    foreach (var task in currentTasks)
                    {
                        if (task is IRemindable remindableTask
                        && !remindableTask.IsReminderSent
                        && remindableTask.ReminderTime <= DateTime.Now)
                        {
                            remindableTask.IsReminderSent = true;
                            ReminderTriggeredEvent?.Invoke(this, new TaskEventArgs(task));
                        }
                    }
                    await Task.Delay(5000);
                }
            });
        }

        public void StopReminderMonitor()
        {
            _isMonitoring = false;
        }
    }
}
