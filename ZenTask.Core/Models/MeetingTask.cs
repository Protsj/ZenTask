using ZenTask.Core.Interfaces;

namespace ZenTask.Core.Models
{
    public class MeetingTask : BaseTask, ICompletable, IRemindable
    {
        public string Location { get; set; }
        public DateTime ReminderTime { get; set; }
        public bool IsReminderSent { get; set; }
        public bool IsCompleted { get; private set; }
        public MeetingTask(string title, DateTime reminderTime, string location= "", string description = "") 
            : base(title, description)
        {
            Location = location;
            ReminderTime = reminderTime;
            IsCompleted = false;
            IsReminderSent = false;
        }
        public void Complete() { IsCompleted = true; }
        public void UndoComplete() { IsCompleted = false; }
    }
}
