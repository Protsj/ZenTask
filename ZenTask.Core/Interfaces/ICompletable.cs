namespace ZenTask.Core.Interfaces
{
    public interface ICompletable //Interface for tasks that can be marked as completeds
    {
        bool IsCompleted { get; }
        void Complete();
        void UndoComplete();
    }
}
