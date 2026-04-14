using ZenTask.Core.Interfaces;

namespace ZenTask.Core.Models
{
    public class HabitTask : BaseTask, ICompletable
    {
        public int Streak { get; private set; }
        public bool IsCompleted { get; private set; }
        public HabitTask(string title, string description = "") 
            : base(title, description)
        {
            Streak = 0;
            IsCompleted = false;
        }
        public void Complete() 
        {
            IsCompleted = true;
            Streak++;
        }
    }
}
