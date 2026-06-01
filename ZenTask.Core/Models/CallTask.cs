using ZenTask.Core.Interfaces;

namespace ZenTask.Core.Models
{
    public class CallTask : BaseTask, ICompletable, IRemindable
    {
        public string ContactName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime ReminderTime { get; set; }
        public string Platform { get; set; }
        public bool IsReminderSent { get; set; }
        public bool IsCompleted { get; private set; }
        public CallTask(string title, string contactName, string phoneNumber, DateTime reminderTime, string platform = "Phone", string description = "") 
            : base(title, description)
        {
            ContactName = contactName;
            PhoneNumber = phoneNumber;
            ReminderTime = reminderTime;
            Platform = string.IsNullOrWhiteSpace(description) ? "Phone" :platform;
            IsReminderSent = false;
            IsCompleted = false;
        }
        public void Complete() { IsCompleted = true; }
        public void UndoComplete() { IsCompleted = false; }
    }
}
