namespace ZenTask.Core.Interfaces
{
    public interface IRemindable
    {
        DateTime ReminderTime { get; set; }
        bool IsReminderSent { get; set; }
    }
}
