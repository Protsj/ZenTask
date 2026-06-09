using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ZenTask.Core.Data;
using ZenTask.Core.Interfaces;
using ZenTask.Core.Models;
using ZenTask.Core.Services;
using ZenTask.WPF.UIConfig;

namespace ZenTask.WPF
{
    public partial class MainWindow : Window
    {
        private TaskManager _taskManager;
        private SqliteTaskStorage _taskStorage;
        private StackPanel _tasksContainer;
        private TextBlock _tasksTotal;
        private TextBlock _tasksCompleted;
        public MainWindow()
        {
            InitializeComponent();

            _taskStorage = new SqliteTaskStorage();
            _taskManager = new TaskManager();

            LoadDynamicUI();

            LoadDataFromDatabaseAsync();
        }

        private void LoadDynamicUI()
        {
            var builder = new UIBuilder();
            string jsonPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "main_window.json");
            var windowConfig = builder.LoadMainWindowConfig(jsonPath);

            this.Title = windowConfig.Title;
            this.Width = windowConfig.Width;
            this.Height = windowConfig.Height;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.ResizeMode = ResizeMode.NoResize;
            if (!string.IsNullOrEmpty(windowConfig.Background))
                this.Background = (Brush)new BrushConverter().ConvertFromString(windowConfig.Background);

            if (windowConfig.RootElement != null)
            {
                var rootUI = builder.BuildElement(windowConfig.RootElement) as FrameworkElement;
                this.Content = rootUI;

                _tasksContainer = FindElementByName(rootUI, "TasksContainer") as StackPanel;
                _tasksTotal = FindElementByName(rootUI, "TasksTotal") as TextBlock;
                _tasksCompleted = FindElementByName(rootUI, "TasksCompleted") as TextBlock;

                var btnAddTask = FindElementByName(rootUI, "BtnAddTask") as Button;

                if (btnAddTask != null)
                {
                    btnAddTask.Click += (s, e) =>
                    {
                        var addWindow = new AddTaskWindow(_taskManager, _taskStorage);
                        addWindow.Owner = this;
                        addWindow.ShowDialog();
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            RefreshTasksList();
                        }));
                    };
                }
            }
        }

        private async void LoadDataFromDatabaseAsync()
        {
            try
            {
                var tasksFromDb = await _taskStorage.LoadTasksAsync();
                foreach (var task in tasksFromDb)
                    _taskManager.AddTask(task);
                RefreshTasksList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading from database: {ex.Message}");
            }
        }

        private FrameworkElement FindElementByName(FrameworkElement parent, string name)
        {
            if (parent == null)
                return null;
            if (parent.Name == name)
                return parent;

            foreach (object logicalChild in LogicalTreeHelper.GetChildren(parent))
            {
                if (logicalChild is FrameworkElement child)
                {
                    var result = FindElementByName(child, name);
                    if (result != null) 
                        return result;
                }
            }
            if (parent is ContentControl cc && cc.Content is FrameworkElement ccChild)
            {
                var result = FindElementByName(ccChild, name);
                if (result != null) 
                    return result;
            }
            if (parent is Border b && b.Child is FrameworkElement bChild)
            {
                var result = FindElementByName(bChild, name);
                if (result != null) 
                    return result;
            }
            if (parent is Panel p)
            {
                foreach (UIElement child in p.Children)
                {
                    if (child is FrameworkElement fe)
                    {
                        var result = FindElementByName(fe, name);
                        if (result != null) 
                            return result;
                    }
                }
            }
            return null;
        }

        private void RefreshTasksList()
        {
            if (_tasksContainer == null)
            {
                MessageBox.Show("System error: Container 'TasksContainer' is not found!\nPlease, delete main_window.json in the Debug folder and reopen the application.", "UI Error");
                return;
            }
                
            _tasksContainer.Children.Clear();

            var tasks = _taskManager.GetTasks();

            int totalCount = tasks.Count;
            int completedCount = tasks.Count(t => t is ICompletable c && c.IsCompleted);

            if (_tasksTotal != null) 
                _tasksTotal.Text = $"{totalCount} tasks";
            if (_tasksCompleted != null) 
                _tasksCompleted.Text = $"{completedCount} completed";

            foreach (var task in tasks)
            {
                var card = UIBuilder.CreateTaskCard(task,
                    onDeleteClick: async (s, e) =>
                    {
                        var result = MessageBox.Show($"Are you sure you want to delete '{task.Title}'?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                _taskManager.RemoveTask(task.Id);
                                await _taskStorage.DeleteTaskAsync(task.Id);
                                RefreshTasksList();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error deleting task: {ex.Message}");
                            }
                        }
                    },
                    onExpandClick: (s, e) =>
                    {
                        MessageBox.Show($"Details:\n\nTitle: {task.Title}\nCreated: {task.CreatedAt}", "Task Details");
                    }
                );

                _tasksContainer.Children.Add(card);
            }
            _tasksContainer.UpdateLayout();
        }
    }
}