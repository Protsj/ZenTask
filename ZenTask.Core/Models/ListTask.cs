using ZenTask.Core.Interfaces;

namespace ZenTask.Core.Models
{
    public class CheckListItem
    {
        public string Name { get; set; }
        public bool IsDone { get; set; }
    }
    public class ListTask : BaseTask, ICompletable
    {
        public List<CheckListItem> Items { get; set; }
        public bool IsCompleted 
        { get { return Items != null && Items.Any() && Items.All(item => item.IsDone); } }
        public ListTask(string title, string description = "") 
            : base(title, description)
        { Items = new List<CheckListItem>(); }
        public void AddItem(string itemName) 
        {
            Items.Add(new CheckListItem { Name = itemName, IsDone = false });
        }
        public void Complete() 
        {
            foreach (var item in Items)
                item.IsDone = true;
        }
        public void UndoComplete() 
        {
            foreach (var item in Items)
                item.IsDone = false;
        }
    }
}
