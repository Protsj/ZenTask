using System.Threading.Tasks;
using ZenTask.Core.Data;
using ZenTask.Core.Models;
using ZenTask.Core.Services;

var storage = new SqliteTaskStorage();
var manager = new TaskManager();

manager.TaskCompletedEvents += ( sender, e) => { Console.WriteLine($"Task completed: {e.Task.Title}"); };

Console.WriteLine("Loading tasks...");
var loadedTasks = await storage.LoadTasksAsync();
foreach(var task in loadedTasks)
    manager.AddTask(task);
Console.WriteLine($"Loaded tasks: {loadedTasks.Count}\n");

bool exit = false;
while (!exit)
{
    Console.WriteLine("\nZenTask Menu");
    Console.WriteLine("1. Add Task");
    Console.WriteLine("2. Show Tasks");
    Console.WriteLine("3. Complete Task");
    Console.WriteLine("4. Delete Task");
    Console.WriteLine("5. Exit");
    Console.Write("Enter your choice: ");
    string choice = Console.ReadLine();
    switch (choice)
    {
        case "1": AddTaskFlow(manager); break;
        case "2": ShowTasks(manager); break;
        case "3": CompleteTaskFlow(manager); break;
        case "4": DeleteTaskFlow(manager); break;
        case "5": exit = true; break;
        default: Console.WriteLine("Invalid choice!\n"); break;
    }
}

static void AddTaskFlow(TaskManager manager)
{
    Console.WriteLine("\nSelect task type to add:");
    Console.WriteLine("1. Call Task");
    Console.WriteLine("2. Focus Task");
    Console.WriteLine("3. Habit Task");
    Console.WriteLine("4. List Task");
    Console.WriteLine("5. Meeting Task");
    Console.WriteLine("6. Urgent Task");
    Console.Write("Enter your choice: ");
    string choice = Console.ReadLine();
    switch (choice)
    {
        case "1": AddCallTask(manager); break;
        case "2": AddFocusTask(manager); break;
        case "3": AddHabitTask(manager); break;
        case "4": AddListTask(manager); break;
        case "5": AddMeetingTask(manager); break;
        case "6": AddUrgentTask(manager); break;
        default: Console.WriteLine("Invalid choice!\n"); break;
    }
}

static void ShowTasks(TaskManager manager)
{
    Console.WriteLine("\nTask List");
    var tasks = manager.GetTasks();
    if (tasks.Count == 0)
        Console.WriteLine("No tasks available.");
    else
    {
        for (int i = 0; i < tasks.Count; i++)
        {
            var t = tasks[i];
            string status = t is ZenTask.Core.Interfaces.ICompletable c && c.IsCompleted ? "[x]" : "[ ]";
            string extraInfo = t is HabitTask h ? $"(Streak: {h.Streak} days)" : "";
            Console.WriteLine($"{i + 1}. {status} {t.Title} {extraInfo}");
        }
    }
    Console.WriteLine("----------------------\n");
}

static void AddCallTask(TaskManager manager)
{
    Console.Write("\nEnter the call task name: ");
    string title = Console.ReadLine();
    Console.Write("Enter contact name: ");
    string contactName = Console.ReadLine();
    Console.Write("Enter phone number: ");
    string phoneNumber = Console.ReadLine();
    Console.Write("Enter platform: ");
    string platform = Console.ReadLine();
    Console.Write("Enter reminder time (yyyy-MM-dd HH:mm): ");
    DateTime reminderTime;
    if (!DateTime.TryParse(Console.ReadLine(), out reminderTime))
    {
        Console.WriteLine("Invalid reminder time! Using current time.");
        reminderTime = DateTime.Now;
    }
    var callTask = new CallTask(title, contactName, reminderTime, phoneNumber, platform);
    manager.AddTask(callTask);
    Console.WriteLine("Call task successfully added!\n");
}
static void AddFocusTask(TaskManager manager)
{
    Console.Write("\nEnter the focus task name: ");
    string title = Console.ReadLine();
    Console.Write("Enter estimated duration in minutes: ");
    if (int.TryParse(Console.ReadLine(), out int minutes) && minutes > 0)
    {
        var focusTask = new FocusTask(title, TimeSpan.FromMinutes(minutes));
        manager.AddTask(focusTask);
        Console.WriteLine("Focus task successfully added!\n");
    }
    else
    {
        Console.WriteLine("Invalid duration!\n");
    }
}
static void AddHabitTask(TaskManager manager)
{
    Console.Write("\nEnter the habit name: ");
    string title = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(title))
    {
        var habit = new HabitTask(title);
        manager.AddTask(habit);
        Console.WriteLine("Habit successfully added!\n");
    }
    else
    {
        Console.WriteLine("Name cannot be empty!\n");
    }
}

static void AddListTask(TaskManager manager)
{
    Console.Write("\nEnter the list task name: ");
    string title = Console.ReadLine();
    var listTask = new ListTask(title);
    while (true)
    {
        Console.Write("Add an item to the list (or leave empty to finish): ");
        string item = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(item))
            break;
        listTask.AddItem(item);
    }
    manager.AddTask(listTask);
    Console.WriteLine("List task successfully added!\n");
}

static void AddMeetingTask(TaskManager manager)
{
    Console.Write("\nEnter the meeting task name: ");
    string title = Console.ReadLine();
    Console.Write("Enter meeting time (yyyy-MM-dd HH:mm): ");
    if (DateTime.TryParse(Console.ReadLine(), out DateTime meetingTime) && meetingTime > DateTime.Now)
    {
        var meetingTask = new MeetingTask(title, meetingTime);
        manager.AddTask(meetingTask);
        Console.WriteLine("Meeting task successfully added!\n");
    }
    else
    {
        Console.WriteLine("Invalid meeting time!\n");
    }
}

static void AddUrgentTask(TaskManager manager)
{
    Console.Write("\nEnter the urgent task name: ");
    string title = Console.ReadLine();
    Console.Write("Enter deadline (yyyy-MM-dd HH:mm): ");
    if (DateTime.TryParse(Console.ReadLine(), out DateTime deadline) && deadline > DateTime.Now)
    {
        var urgentTask = new UrgentTask(title, deadline);
        manager.AddTask(urgentTask);
        Console.WriteLine("Urgent task successfully added!\n");
    }
    else
    {
        Console.WriteLine("Invalid deadline!\n");
    }
}

static void CompleteTaskFlow(TaskManager manager)
{
    var tasks = manager.GetTasks();
    if (tasks.Count == 0)
    {
        Console.WriteLine("\nNo tasks available to complete.\n");
        return;
    }

    ShowTasks(manager);
    Console.Write("Enter the number of the task you want to complete: ");

    if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= tasks.Count)
    {
        var taskToComplete = tasks[index - 1];
        manager.CompleteTask(taskToComplete.Id);
    }
    else
        Console.WriteLine("Invalid number.\n");
}

static async Task DeleteTaskFlow(TaskManager manager)
{
    var tasks = manager.GetTasks();
    if (tasks.Count == 0)
    {
        Console.WriteLine("\nNo tasks available to delete.\n");
        return;
    }
    ShowTasks(manager);
    Console.Write("Enter the number of the task you want to delete: ");
    if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= tasks.Count)
    {
        var taskToDelete = tasks[index - 1];
        manager.RemoveTask(taskToDelete.Id);
        await new SqliteTaskStorage().DeleteTaskAsync(taskToDelete.Id);
        Console.WriteLine("Task deleted successfully!\n");
    }
    else
        Console.WriteLine("Invalid number.\n");
}
