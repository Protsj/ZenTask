using Microsoft.EntityFrameworkCore;
using ZenTask.Core.Models;

namespace ZenTask.Core.Data
{
    public class SqliteTaskStorage
    {
        public async Task SaveTaskAsync(IEnumerable<BaseTask> tasks)
        {
            if(tasks == null)
                throw new ArgumentNullException(nameof(tasks), "Tasks collection cannot be null.");

            using var context = new TaskDbContext();

            foreach (var task in tasks)
            {
                bool exists = await context.Tasks.AnyAsync(t => t.Id == task.Id);
                if (exists)
                    context.Tasks.Update(task);
                else
                    await context.Tasks.AddAsync(task);
            }
            await context.SaveChangesAsync();
        }

        public async Task<List<BaseTask>> LoadTasksAsync()
        {
            using var context = new TaskDbContext();
            return await context.Tasks.ToListAsync();
        }
        public async Task DeleteTaskAsync(Guid id)
        {
            using var context = new TaskDbContext();
            var taskToDelete = await context.Tasks.FindAsync(id);
            if (taskToDelete != null)
            {
                context.Tasks.Remove(taskToDelete);
                await context.SaveChangesAsync();
            }
        }
    }
}
