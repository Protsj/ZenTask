using ZenTask.Core.Interfaces;

namespace ZenTask.Core.Models
{
    public class UrgentTask : BaseTask, ICompletable, IRemindable
    {
        public DateTime ReminderTime { get; set; }
        public bool IsCompleted { get; private set; }
        public bool IsReminderSent { get; set; }
        public UrgentTask(string title, DateTime reminderTime, string description = "") 
            : base(title, description)
        {
            ReminderTime = reminderTime;
            IsCompleted = false;
            IsReminderSent = false;
        }
        public void Complete() { IsCompleted = true; }
        public void UndoComplete() { IsCompleted = false; }
    }
}
