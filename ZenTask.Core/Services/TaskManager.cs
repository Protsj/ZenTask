using ZenTask.Core.Interfaces;
using ZenTask.Core.Models;

namespace ZenTask.Core.Services
{
    public class TaskEventArgs : EventArgs
    {
        public BaseTask Task { get; }
        public bool IsOverdue { get; set; }
        public TaskEventArgs(BaseTask task, bool isOverdue = false)
        {
            Task = task;
            IsOverdue = isOverdue;
        }
    }

    public class TaskManager
    {
        private readonly List<BaseTask> _tasks;
        private bool _isMonitoring = false;
        public event EventHandler<TaskEventArgs> TaskCompletedEvents;
        public event EventHandler<TaskEventArgs> ReminderTriggeredEvent;
        private readonly Dictionary<Guid, int> _reminderCounters = new Dictionary<Guid, int>();
        
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
                    var now = DateTime.Now;
                    foreach (var task in GetTasks())
                    {
                        if (task is IRemindable r && task is ICompletable c && !c.IsCompleted)
                        {
                            if (r.ReminderTime <= now && !r.IsReminderSent)
                            {
                                r.IsReminderSent = true;
                                ReminderTriggeredEvent?.Invoke(this, new TaskEventArgs(task, true));
                            }
                            else if (r.ReminderTime > now && r.ReminderTime <= now.AddMinutes(15) && !r.IsReminderSent)
                            {
                                if (!_reminderCounters.ContainsKey(task.Id)) 
                                    _reminderCounters[task.Id] = 0;

                                if (_reminderCounters[task.Id] < 3)
                                {
                                    _reminderCounters[task.Id]++;
                                    ReminderTriggeredEvent?.Invoke(this, new TaskEventArgs(task, false));
                                }
                            }
                        }
                    }
                    await Task.Delay(60000);
                }
            });
        }

        public void StopReminderMonitor()
        {
            _isMonitoring = false;
        }
    }
}
