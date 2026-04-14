using ZenTask.Core.Interfaces;

namespace ZenTask.Core.Models
{
    public class FocusTask : BaseTask, ICompletable
    {
        public TimeSpan EstimatedDuration { get; set; }
        public int PomodoroCount { get; set; }
        public bool IsCompleted { get; private set; }
        public FocusTask(string title, TimeSpan duration, int pomodoroCount = 1, string description = "") 
            : base(title, description)
        {
            EstimatedDuration = duration;
            PomodoroCount = pomodoroCount;
            IsCompleted = false;
        }
        public void Complete() { IsCompleted = true; }
    }
}
