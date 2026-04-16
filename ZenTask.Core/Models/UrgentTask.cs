using ZenTask.Core.Interfaces;

namespace ZenTask.Core.Models
{
    public class UrgentTask : BaseTask, ICompletable
    {
        public DateTime Deadline { get; set; }
        public bool IsCompleted { get; private set; }
        public UrgentTask(string title, DateTime deadline, string description = "") 
            : base(title, description)
        {
            Deadline = deadline;
            IsCompleted = false;
        }
        public void Complete() { IsCompleted = true; }
        public void UndoComplete() { IsCompleted = false; }
    }
}
