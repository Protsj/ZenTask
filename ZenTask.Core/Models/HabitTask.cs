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
            if (!IsCompleted)
            {
                IsCompleted = true;
                Streak++;
            }
        }
        public void UndoComplete() 
        {
            if (IsCompleted)
            {
                IsCompleted = false;
                Streak = Math.Max(0, Streak - 1);
            }
        }
        public void ResetCycle()
        {
            if (!IsCompleted)
                Streak = 0;
            IsCompleted = false;
        }
    }
}
