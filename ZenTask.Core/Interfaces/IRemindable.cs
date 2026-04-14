namespace ZenTask.Core.Interfaces
{
    public interface IRemindable //Interface for tasks that can have reminders
    {
        DateTime ReminderTime { get; set; }
        bool IsReminderSent { get; set; }
    }
}
