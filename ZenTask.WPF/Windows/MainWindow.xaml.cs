using Microsoft.Toolkit.Uwp.Notifications;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        private string _currentFilter = "All Tasks";

        public MainWindow()
        {
            InitializeComponent();

            _taskStorage = new SqliteTaskStorage();
            _taskManager = new TaskManager();

            _taskManager.ReminderTriggeredEvent += OnReminderTriggered;

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
                    btnAddTask.ClearValue(Button.BackgroundProperty);
                    btnAddTask.ClearValue(Button.ForegroundProperty);
                    btnAddTask.Style = (Style)Application.Current.Resources["PrimaryPillButton"];
                    btnAddTask.Click += (s, e) =>
                    {
                        var addWindow = new AddTaskWindow(_taskManager, _taskStorage);
                        addWindow.Owner = this;
                        addWindow.ShowDialog();
                        Application.Current.Dispatcher.BeginInvoke(new Action(() => { RefreshTasksList(); }));
                    };
                }

                ScrollViewer filterScroll = new ScrollViewer
                {
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                    Margin = new Thickness(15, 0, 15, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };

                StackPanel filterPanel = new StackPanel 
                { 
                    Orientation = Orientation.Horizontal 
                };

                filterScroll.Content = filterPanel;

                string[] filters = { "All Tasks", "Call", "Focus", "Habit", "List", "Meeting", "Urgent" };
                List<Border> filterButtons = new List<Border>();

                foreach (var f in filters)
                {
                    bool isActive = f == _currentFilter;

                    Border btnBorder = new Border
                    {
                        CornerRadius = new CornerRadius(10),
                        Padding = new Thickness(12, 6, 12, 6),
                        Margin = new Thickness(0, 0, 8, 0),
                        Cursor = Cursors.Hand,

                        Background = (Brush)new BrushConverter().ConvertFromString(isActive ? "#111827" : "#F3F4F6")
                    };

                    TextBlock txt = new TextBlock
                    {
                        Text = f,
                        FontSize = 13,
                        FontWeight = isActive ? FontWeights.Bold : FontWeights.Normal,
                        Foreground = (Brush)new BrushConverter().ConvertFromString(isActive ? "#FFFFFF" : "#4B5563"),
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    btnBorder.Child = txt;

                    btnBorder.MouseEnter += (s, ev) =>
                    {
                        if (_currentFilter != f)
                            btnBorder.Background = (Brush)new BrushConverter().ConvertFromString("#D1D5DB");
                    };

                    btnBorder.MouseLeave += (s, ev) =>
                    {
                        if (_currentFilter != f)
                            btnBorder.Background = (Brush)new BrushConverter().ConvertFromString("#F3F4F6");
                    };

                    btnBorder.MouseLeftButtonDown += (s, ev) =>
                    {
                        _currentFilter = f;

                        foreach (var b in filterButtons)
                        {
                            var t = b.Child as TextBlock;
                            if (t.Text == _currentFilter)
                            {
                                b.Background = (Brush)new BrushConverter().ConvertFromString("#111827");
                                t.Foreground = Brushes.White;
                                t.FontWeight = FontWeights.Bold;
                            }
                            else
                            {
                                b.Background = (Brush)new BrushConverter().ConvertFromString("#F3F4F6");
                                t.Foreground = (Brush)new BrushConverter().ConvertFromString("#4B5563");
                                t.FontWeight = FontWeights.Normal;
                            }
                        }

                        RefreshTasksList();
                    };

                    filterButtons.Add(btnBorder);
                    filterPanel.Children.Add(btnBorder);
                }

                if (btnAddTask != null && btnAddTask.Parent is Panel parentPanel)
                    parentPanel.Children.Insert(parentPanel.Children.IndexOf(btnAddTask), filterScroll);
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
                _taskManager.StartReminderMonitor();
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


            var filteredTasks = tasks.Where(task =>
                _currentFilter == "All Tasks" ||
                (_currentFilter == "Call" && task is CallTask) ||
                (_currentFilter == "Focus" && task is FocusTask) ||
                (_currentFilter == "Habit" && task is HabitTask) ||
                (_currentFilter == "List" && task is ListTask) ||
                (_currentFilter == "Meeting" && task is MeetingTask) ||
                (_currentFilter == "Urgent" && task is UrgentTask)
            ).ToList();

            if (filteredTasks.Count == 0)
            {
                StackPanel emptyStatePanel = new StackPanel
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 60, 0, 0)
                };

                TextBlock iconBlock = new TextBlock
                {
                    Text = "✨",
                    FontSize = 48,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 15)
                };

                TextBlock messageBlock = new TextBlock
                {
                    Text = _currentFilter == "All Tasks"
                        ? "You have no tasks yet. Time to chill or add a new one! ✨"
                        : $"No {_currentFilter} tasks found. You're all caught up! 🎉",
                    FontSize = 14,
                    FontWeight = FontWeights.Medium,
                    Foreground = (Brush)new BrushConverter().ConvertFromString("#9CA3AF"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextAlignment = TextAlignment.Center,
                    TextWrapping = TextWrapping.Wrap
                };

                emptyStatePanel.Children.Add(iconBlock);
                emptyStatePanel.Children.Add(messageBlock);
                _tasksContainer.Children.Add(emptyStatePanel);
            }
            else
            {
                foreach (var task in filteredTasks)
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
                            btnStartPomo.Style = (Style)Application.Current.Resources["PrimaryPillButton"];
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
        }

        private void AnimateExpand(StackPanel panel, Button button, Border parentCard)
        {
            panel.Visibility = Visibility.Visible;
            button.Content = "▲";

            double availableWidth = parentCard.ActualWidth - 30;

            if (availableWidth <= 0) 
                availableWidth = double.PositiveInfinity;

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
                await _taskStorage.SaveTaskAsync(_taskManager.GetTasks());
        }

        private async void OnReminderTriggered(object sender, TaskEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (e.IsOverdue)
                {
                    new ToastContentBuilder()
                        .AddText("🚨 Overdue Task!")
                        .AddText($"Task '{e.Task.Title}' is past due.")
                        .Show();
                }
                else
                {
                    new ToastContentBuilder()
                        .AddText("⏳ Upcoming Task")
                        .AddText($"'{e.Task.Title}' is due in 15 minutes or less.")
                        .Show();
                }
            });

            if (e.IsOverdue)
                await _taskStorage.SaveTaskAsync(_taskManager.GetTasks());
        }
    }
}