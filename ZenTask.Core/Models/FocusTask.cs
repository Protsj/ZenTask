using ZenTask.Core.Interfaces;

namespace ZenTask.Core.Models
{
    public class FocusTask : BaseTask, ICompletable
    {
        public TimeSpan EstimatedDuration { get; set; }
        public int PomodoroCount { get; set; }
        public bool IsCompleted { get; private set; }
        public FocusTask(string title, TimeSpan estimatedDuration, int pomodoroCount = 0, string description = "") 
            : base(title, description)
        {
            EstimatedDuration = estimatedDuration;
            PomodoroCount = pomodoroCount;
            IsCompleted = false;
        }
        public void Complete() { IsCompleted = true; }
        public void UndoComplete() { IsCompleted = false; }
    }
}
