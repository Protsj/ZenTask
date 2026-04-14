namespace ZenTask.Core.Models
{
    public abstract class BaseTask //Abstract base class for all tasks, containing common properties and logic
    {
        public Guid Id { get; protected set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; protected set; }
        protected BaseTask(string title, string description = "")
        {
            if(string.IsNullOrWhiteSpace(title))
                throw new ArgumentException($"Title cannot be empty. {nameof(title)}");
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            CreatedAt = DateTime.UtcNow;
        }

    }
}
