using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
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
        private Guid? _expandedTaskId = null;
        private Button _currentlyExpandedButton = null;
        private DispatcherTimer _uiUpdateTimer;

        public MainWindow()
        {
            InitializeComponent();

            _taskStorage = new SqliteTaskStorage();
            _taskManager = new TaskManager();

            LoadDynamicUI();
            LoadDataFromDatabaseAsync();

            _uiUpdateTimer = new DispatcherTimer();
            _uiUpdateTimer.Interval = TimeSpan.FromSeconds(60);
            _uiUpdateTimer.Tick += (s, e) => { RefreshTasksList(); };
            _uiUpdateTimer.Start();
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

                if (_tasksContainer != null && _tasksContainer.Parent is ScrollViewer scroll)
                {
                    scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    scroll.Margin = new Thickness(scroll.Margin.Left, 50, scroll.Margin.Right, 30);
                }

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
                        Application.Current.Dispatcher.BeginInvoke(new Action(() => { RefreshTasksList(); }));
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
                AuditHabits();
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
                if (logicalChild is FrameworkElement fe)
                {
                    var result = FindElementByName(fe, name);
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
                return;
                
            _tasksContainer.Children.Clear();
            _currentlyExpandedButton = null;
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
                    onEditClick: (s, e) =>
                    {
                        var editWindow = new EditTaskWindow(_taskManager, _taskStorage, task);
                        editWindow.Owner = this;
                        editWindow.ShowDialog();
                        Application.Current.Dispatcher.BeginInvoke(new Action(() => { RefreshTasksList(); }));
                    },
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
                    onCompleteClick: async (s, e) =>
                    {
                        if (task is HabitTask habit)
                        {
                            if (habit.IsCompleted)
                                habit.UndoComplete();
                            else
                            {
                                habit.Complete();
                                habit.LastCompletedDate = DateTime.Today;
                            }
                        }
                        else if (task is ICompletable completableTask)
                        {
                            bool newState = !completableTask.IsCompleted;
                            var prop = completableTask.GetType().GetProperty("IsCompleted");
                            if (prop != null && prop.CanWrite)
                                prop.SetValue(completableTask, newState);
                        }

                        if (task is ListTask listTask)
                        {
                            bool isParentCompleted = (task as ICompletable)?.IsCompleted ?? false;

                            foreach (var childItem in listTask.Items)
                            {
                                var childStatusProp = childItem.GetType().GetProperty("IsCompleted") ?? childItem.GetType().GetProperty("IsDone");
                                if (childStatusProp != null && childStatusProp.CanWrite)
                                    childStatusProp.SetValue(childItem, isParentCompleted);
                            }
                        }

                        await _taskStorage.SaveTaskAsync(_taskManager.GetTasks());
                        RefreshTasksList();
                    },
                    onToggleExpand: (cardBorder, btnExpand) =>
                    {
                        var panel = FindElementByName(cardBorder, "DetailsPanel") as StackPanel;
                        
                        if (panel == null) 
                            return;
                        
                        if (_expandedTaskId == task.Id)
                        {
                            AnimateCollapse(panel, btnExpand);
                            _expandedTaskId = null;
                            _currentlyExpandedButton = null;
                        }
                        else
                        {
                            if (_expandedTaskId != null)
                            {
                                string oldCardName = $"Card_{_expandedTaskId.Value.ToString().Replace("-", "_")}";
                                var oldCard = FindElementByName(_tasksContainer, oldCardName) as Border;
                                
                                if (oldCard != null)
                                {
                                    var oldPanel = FindElementByName(oldCard, "DetailsPanel") as StackPanel;
                                    
                                    if (oldPanel != null && _currentlyExpandedButton != null)
                                        AnimateCollapse(oldPanel, _currentlyExpandedButton);
                                }
                            }

                            AnimateExpand(panel, btnExpand, cardBorder);
                            _expandedTaskId = task.Id;
                            _currentlyExpandedButton = btnExpand;
                        }
                    },
                    onSubTaskChanged: async () =>
                    {
                        await _taskStorage.SaveTaskAsync(_taskManager.GetTasks());
                        RefreshTasksList();
                    }

                );
                if (_expandedTaskId == task.Id)
                {
                    if (card is FrameworkElement cardElement)
                    {
                        if (FindElementByName(cardElement, "DetailsPanel") is StackPanel panel)
                        {
                            var btnExpand = FindElementByName(cardElement, "BtnExpand") as Button ?? cardElement.FindName("BtnExpand") as Button;
                            
                            if (btnExpand == null)
                                btnExpand = FindExpandButton(cardElement);

                            panel.Visibility = Visibility.Visible;
                            panel.Height = double.NaN;

                            if (btnExpand != null)
                            {
                                btnExpand.Content = "▲";
                                _currentlyExpandedButton = btnExpand;
                            }
                        }
                    }
                }
                if (task is FocusTask)
                {
                    string btnPomoName = $"BtnStartPomo_{task.Id.ToString().Replace("-", "_")}";
                    Button btnStartPomo = FindElementByName((FrameworkElement)card, btnPomoName) as Button;

                    if (btnStartPomo != null)
                    {
                        btnStartPomo.Click += (s, e) =>
                        {
                            PomodoroTimerWindow timerWindow = new PomodoroTimerWindow((FocusTask)task, async () =>
                            {
                                await _taskStorage.SaveTaskAsync(_taskManager.GetTasks());
                                RefreshTasksList();
                            });

                            timerWindow.Owner = this;
                            timerWindow.ShowDialog();
                        };
                    }
                }
                _tasksContainer.Children.Add(card);
            }
            _tasksContainer.UpdateLayout();
        }

        private void AnimateExpand(StackPanel panel, Button button, Border parentCard)
        {
            panel.Visibility = Visibility.Visible;
            button.Content = "▲";

            double availableWidth = parentCard.ActualWidth - 30;
            if (availableWidth <= 0) availableWidth = double.PositiveInfinity;

            panel.Measure(new Size(availableWidth, double.PositiveInfinity));
            double targetHeight = panel.DesiredSize.Height;

            double startHeight = panel.ActualHeight;

            DoubleAnimation animation = new DoubleAnimation
            {
                From = startHeight,
                To = targetHeight,
                Duration = TimeSpan.FromSeconds(0.25),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            animation.Completed += (s, e) =>
            {
                panel.BeginAnimation(FrameworkElement.HeightProperty, null);
                panel.Height = double.NaN;
            };

            panel.BeginAnimation(FrameworkElement.HeightProperty, animation);
        }

        private void AnimateCollapse(StackPanel panel, Button button)
        {
            button.Content = "▼";

            DoubleAnimation animation = new DoubleAnimation
            {
                From = panel.ActualHeight,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.2),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            animation.Completed += (s, e) =>
            {
                panel.BeginAnimation(FrameworkElement.HeightProperty, null);
                panel.Visibility = Visibility.Collapsed;
                panel.Height = double.NaN;
            };

            panel.BeginAnimation(FrameworkElement.HeightProperty, animation);
        }

        private Button FindExpandButton(FrameworkElement parent)
        {
            if (parent is Button b && (b.Content.ToString() == "▼" || b.Content.ToString() == "▲")) 
                return b;
            
            foreach (object logicalChild in LogicalTreeHelper.GetChildren(parent))
            {
                if (logicalChild is FrameworkElement fe)
                {
                    var res = FindExpandButton(fe);
                    if (res != null) 
                        return res;
                }
            }

            return null;
        }

        private async void AuditHabits()
        {
            bool needsSave = false;
            var today = DateTime.Today;

            foreach (var task in _taskManager.GetTasks().OfType<HabitTask>())
            {
                var lastCompleted = task.LastCompletedDate.Date;
                if (task.IsCompleted && lastCompleted < today)
                {
                    task.ResetCycle();
                    needsSave = true;
                }

                if (!task.IsCompleted && lastCompleted < today.AddDays(-1))
                {
                    if (task.Streak > 0)
                    {
                        task.ResetCycle();
                        needsSave = true;
                    }
                }
            }

            if (needsSave)
            {
                await _taskStorage.SaveTaskAsync(_taskManager.GetTasks());
            }
        }
    }
}