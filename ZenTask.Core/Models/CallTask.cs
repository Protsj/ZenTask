using ZenTask.Core.Interfaces;

namespace ZenTask.Core.Models
{
    public class CallTask : BaseTask, ICompletable, IRemindable
    {
        public string ContactName { get; set; }
        public string PhoneNumber { get; set; }
        public string Platform { get; set; } //"Phone", "Zoom", "Teams"
        public DateTime ReminderTime { get; set; }
        public bool IsReminderSent { get; set; }
        public bool IsCompleted { get; private set; }
        public CallTask(string title, string contactName, DateTime reminderTime, string platform = "Phone", string description = "") 
            : base(title, description)
        {
            ContactName = contactName;
            Platform = platform;
            ReminderTime = reminderTime;
            IsCompleted = false;
            IsReminderSent = false;
        }
        public void Complete() { IsCompleted = true; }
    }
}
