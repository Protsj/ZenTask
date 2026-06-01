namespace ZenTask.Core.Interfaces
{
    public interface ICompletable
    {
        bool IsCompleted { get; }
        void Complete();
        void UndoComplete();
    }
}
