using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using ZenTask.Core.Interfaces;
using ZenTask.Core.Models;

namespace ZenTask.WPF.UIConfig
{
    public class UIBuilder
    {
        public WindowConfig LoadMainWindowConfig(string path)
        {
            if (!File.Exists(path)) 
                GenerateDefaultMainWindowConfig(path);
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<WindowConfig>(json);
        }

        public WindowConfig LoadAddWindowConfig(string path)
        {
            if (!File.Exists(path)) 
                GenerateDefaultAddWindowConfig(path);
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<WindowConfig>(json);
        }

        private void GenerateDefaultMainWindowConfig(string filePath)
        {
            var defaultConfig = new WindowConfig
            {
                Title = "ZenTask",
                Width = 920,
                Height = 700,
                Background = "#FFFFFF",
                RootElement = new ElementConfig
                {
                    Type = "Grid",
                    Children = new List<ElementConfig>
            {
                new ElementConfig
                {
                    Type = "Border", 
                    Height = 1, 
                    Background = "#E5E7EB", 
                    VerticalAlignment = "Top"
                },

                // Upper panel with title and add button
                new ElementConfig
                {
                    Type = "Grid",
                    Height = 30,
                    VerticalAlignment = "Top",
                    Margin = "15,5,15,0",
                    Children = new List<ElementConfig>
                    {
                        new ElementConfig
                        {
                            Type = "Button",
                            Name = "BtnAddTask",
                            Content = "+ New Task",
                            Background = "#F3F4F6",
                            Foreground = "#4B5563",
                            FontSize = 13,
                            Width = 100,
                            Height = 30,
                            HorizontalAlignment = "Right"
                        }
                    }
                },
                new ElementConfig
                {
                    Type = "Border", 
                    Height = 1, 
                    Background = "#E5E7EB", 
                    VerticalAlignment = "Top", 
                    Margin = "0,40,0,0"
                },

                // Main panel with task list
                new ElementConfig
                {
                    Type = "ScrollViewer",
                    Name = "TasksScrollViewer",
                    Background = "#F9FAFB",
                    VerticalAlignment = "Stretch",
                    Margin = "0,41,0,17",
                    Children = new List<ElementConfig>
                    {
                        new ElementConfig
                        {
                            Type = "StackPanel",
                            Name = "TasksContainer",
                            Orientation = "Vertical",
                            Margin = "15,10,15,10"
                        }
                    }
                },
                new ElementConfig
                {
                    Type = "Border", 
                    Height = 1, 
                    Background = "#E5E7EB", 
                    VerticalAlignment = "Bottom", 
                    Margin = "0,0,0,25"
                },

                // Bottom panel with task counts
                new ElementConfig
                {
                    Type = "Grid",
                    Height = 15,
                    VerticalAlignment = "Bottom",
                    Margin = "15,0,15,5",
                    Children = new List<ElementConfig>
                    {
                        new ElementConfig
                        {
                            Type = "TextBlock",
                            Name = "TasksTotal",
                            Content = "0 tasks",
                            Foreground = "#9CA3AF",
                            FontSize = 11,
                            HorizontalAlignment = "Left"
                        },
                        new ElementConfig
                        {
                            Type = "TextBlock",
                            Name = "TasksCompleted",
                            Content = "0 completed",
                            Foreground = "#9CA3AF",
                            FontSize = 11,
                            HorizontalAlignment = "Right"
                        }
                    }
                }
            }
                }
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(defaultConfig, options);
            File.WriteAllText(filePath, jsonString);
        }

        private void GenerateDefaultAddWindowConfig(string path)
        {
            var defaultConfig = new WindowConfig
            {
                Title = "Select Task Type",
                Width = 450,
                Height = 520,
                Background = "#F3F4F6",
                RootElement = new ElementConfig
                {
                    Type = "StackPanel",
                    Orientation = "Vertical",
                    Margin = "20,20,20,20",
                    Children = new List<ElementConfig>
                    {
                        new ElementConfig 
                        { 
                            Type = "TextBlock", 
                            Foreground = "#111827", 
                            FontSize = 18, 
                            Margin = "0,0,0,20", 
                            HorizontalAlignment = "Center" 
                        },
                        
                        new ElementConfig
                        {
                            Type = "UniformGrid",
                            Height = 380,
                            Children = new List<ElementConfig>
                            {
                                CreateTypeBlock("TileCall", "📞", "Call Task"),
                                CreateTypeBlock("TileFocus", "⏱️", "Focus Task"),
                                CreateTypeBlock("TileHabit", "🔁", "Habit Task"),
                                CreateTypeBlock("TileList", "📋", "List Task"),
                                CreateTypeBlock("TileMeeting", "👥", "Meeting Task"),
                                CreateTypeBlock("TileUrgent", "🚨", "Urgent Task")
                            }
                        }
                    }
                }
            };
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(path, JsonSerializer.Serialize(defaultConfig, options));
        }

        private ElementConfig CreateTypeBlock(string name, string icon, string title)
        {
            return new ElementConfig
            {
                Type = "Border",
                Name = name,
                Background = "#FFFFFF",
                Margin = "8,8,8,8",
                Children = new List<ElementConfig>
                {
                    new ElementConfig
                    {
                        Type = "StackPanel",
                        Orientation = "Vertical",
                        VerticalAlignment = "Center",
                        Children = new List<ElementConfig>
                        {
                            new ElementConfig 
                            { 
                                Type = "TextBlock", 
                                Content = icon, 
                                FontSize = 32, 
                                HorizontalAlignment = "Center", 
                                Margin = "0,0,0,8" 
                            },

                            new ElementConfig 
                            { 
                                Type = "TextBlock", 
                                Content = title, 
                                FontSize = 13, 
                                Foreground = "#4B5563", 
                                HorizontalAlignment = "Center" 
                            }
                        }
                    }
                }
            };
        }

        public UIElement BuildElement(ElementConfig config)
        {
            FrameworkElement element = null;

            switch (config.Type)
            {
                case "StackPanel":
                    var panel = new StackPanel();

                    if (config.Orientation == "Horizontal") 
                        panel.Orientation = Orientation.Horizontal;
                    
                    if (!string.IsNullOrEmpty(config.Background))
                        panel.Background = (Brush)new BrushConverter().ConvertFromString(config.Background);

                    foreach (var child in config.Children)
                        panel.Children.Add(BuildElement(child));

                    element = panel;
                    break;

                case "ScrollViewer":
                    var scroll = new ScrollViewer();
                    scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

                    if (config.Height > 0) 
                        scroll.Height = config.Height;

                    foreach (var child in config.Children)
                        scroll.Content = BuildElement(child);

                    element = scroll;
                    break;

                case "Border":
                    var border = new Border();
                    border.CornerRadius = new CornerRadius(8);
                    
                    if (!string.IsNullOrEmpty(config.Background))
                        border.Background = (Brush)new BrushConverter().ConvertFromString(config.Background);

                    foreach (var child in config.Children)
                        border.Child = BuildElement(child);

                    element = border;
                    break;

                case "TextBlock":
                    var txt = new TextBlock 
                    { 
                        Text = config.Content, 
                        VerticalAlignment = VerticalAlignment.Center 
                    };

                    if (config.FontSize > 0) 
                        txt.FontSize = config.FontSize;

                    if (!string.IsNullOrEmpty(config.Foreground))
                        txt.Foreground = (Brush)new BrushConverter().ConvertFromString(config.Foreground);

                    element = txt;
                    break;
                case "TextBox":
                    var textBox = new TextBox
                    {
                        VerticalContentAlignment = VerticalAlignment.Center,
                        Padding = new Thickness(5),
                        BorderThickness = new Thickness(1),
                        BorderBrush = (Brush)new BrushConverter().ConvertFromString("#E5E7EB")
                    };

                    if (!string.IsNullOrEmpty(config.Background)) 
                        textBox.Background = (Brush)new BrushConverter().ConvertFromString(config.Background);

                    if (!string.IsNullOrEmpty(config.Foreground)) 
                        textBox.Foreground = (Brush)new BrushConverter().ConvertFromString(config.Foreground);

                    element = textBox;
                    break;
                case "Button":
                    var btn = new Button 
                    { 
                        Content = config.Content 
                    };

                    if (config.FontSize > 0) 
                        btn.FontSize = config.FontSize;

                    if (!string.IsNullOrEmpty(config.Background))
                        btn.Background = (Brush)new BrushConverter().ConvertFromString(config.Background);

                    if (!string.IsNullOrEmpty(config.Foreground))
                        btn.Foreground = (Brush)new BrushConverter().ConvertFromString(config.Foreground);

                    element = btn;
                    break;
                case "Grid":
                    var grid = new Grid();

                    if (!string.IsNullOrEmpty(config.Background))
                        grid.Background = (Brush)new BrushConverter().ConvertFromString(config.Background);

                    foreach (var child in config.Children)
                        grid.Children.Add(BuildElement(child));

                    element = grid;
                    break;
                case "UniformGrid":
                    var uGrid = new UniformGrid 
                    { 
                        Columns = 2, 
                        Rows = 3 
                    };

                    foreach (var child in config.Children) 
                        uGrid.Children.Add(BuildElement(child));

                    element = uGrid;
                    break;
                case "DatePicker":
                    var picker = new DatePicker 
                    { 
                        VerticalContentAlignment = VerticalAlignment.Center 
                    };

                    picker.SelectedDate = DateTime.Today;
                    element = picker;
                    break;

                case "ComboBox":
                    var comboBox = new ComboBox 
                    { 
                        VerticalContentAlignment = VerticalAlignment.Center 
                    };

                    foreach (var child in config.Children)
                        if (child.Type == "ComboBoxItem")
                            comboBox.Items.Add(new ComboBoxItem { Content = child.Content, Padding = new Thickness(5) });
                    
                    if (comboBox.Items.Count > 0) 
                        comboBox.SelectedIndex = 0;

                    element = comboBox;
                    break;
            }

            if (element != null)
            {
                if (!string.IsNullOrEmpty(config.Name)) 
                    element.Name = config.Name;

                if (config.Width > 0) 
                    element.Width = config.Width;

                if (config.Height > 0) 
                    element.Height = config.Height;

                if (!string.IsNullOrEmpty(config.Margin))
                {
                    var m = config.Margin.Split(',');
                    if (m.Length == 4)
                        element.Margin = new Thickness(double.Parse(m[0]), double.Parse(m[1]), double.Parse(m[2]), double.Parse(m[3]));
                }

                if (!string.IsNullOrEmpty(config.HorizontalAlignment))
                    if (Enum.TryParse(config.HorizontalAlignment, out HorizontalAlignment align))
                        element.HorizontalAlignment = align;

                if (!string.IsNullOrEmpty(config.VerticalAlignment))
                    if (Enum.TryParse(config.VerticalAlignment, out VerticalAlignment vAlign))
                        element.VerticalAlignment = vAlign;
            }

            return element;
        }

        public static UIElement CreateTaskCard(BaseTask task, RoutedEventHandler onEditClick, RoutedEventHandler onDeleteClick, RoutedEventHandler onCompleteClick, Action<Border, Button> onToggleExpand, Func<Task> onSubTaskChanged)
        {
            bool isCompleted = task is ICompletable completable && completable.IsCompleted;

            Border cardBorder = new Border
            {
                Name = $"Card_{task.Id.ToString().Replace("-", "_")}",
                Background = Brushes.White,
                BorderBrush = (Brush)new BrushConverter().ConvertFromString("#E5E7EB"),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Margin = new Thickness(0, 0, 0, 10),
                Padding = new Thickness(15),
                Opacity = isCompleted ? 0.6 : 1.0
            };

            StackPanel cardLayout = new StackPanel();

            Grid topGrid = new Grid();
            topGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(35) });
            topGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            topGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(110) });

            string icon = "📝";
            string typeName = "Task";

            if (task is HabitTask) 
            { 
                icon = "🔁"; 
                typeName = "Habit"; 
            }
            else if (task is UrgentTask) 
            { 
                icon = "🚨"; 
                typeName = "Urgent"; 
            }
            else if (task is MeetingTask) 
            { 
                icon = "👥"; 
                typeName = "Meeting"; 
            }
            else if (task is FocusTask) 
            { 
                icon = "⏱️"; 
                typeName = "Focus"; 
            }
            else if (task is CallTask) 
            { 
                icon = "📞"; 
                typeName = "Call"; 
            }
            else if (task is ListTask) 
            { 
                icon = "📋"; 
                typeName = "List"; 
            }

            TextBlock txtIcon = new TextBlock { Text = icon, FontSize = 20, VerticalAlignment = VerticalAlignment.Center };
            Grid.SetColumn(txtIcon, 0);
            topGrid.Children.Add(txtIcon);

            StackPanel textPanel = new StackPanel { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(5, 0, 0, 0) };
            TextBlock txtTitle = new TextBlock
            {
                Text = task.Title,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = (Brush)new BrushConverter().ConvertFromString("#111827")
            };

            if (isCompleted)
            {
                txtTitle.TextDecorations = TextDecorations.Strikethrough;
                txtTitle.Foreground = (Brush)new BrushConverter().ConvertFromString("#9CA3AF");
            }

            textPanel.Children.Add(txtTitle);

            StackPanel typeAndFeaturePanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 2, 0, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBlock txtType = new TextBlock
            {
                Text = typeName,
                FontSize = 11,
                Foreground = (Brush)new BrushConverter().ConvertFromString("#9CA3AF"),
                VerticalAlignment = VerticalAlignment.Center
            };

            typeAndFeaturePanel.Children.Add(txtType);

            TextBlock txtFeature = new TextBlock
            {
                FontSize = 11,
                FontWeight = FontWeights.Medium,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(6, 0, 0, 0)
            };

            bool hasFeature = false;

            if (task is HabitTask habitTaskFeature)
            {
                var streakProp = habitTaskFeature.GetType().GetProperty("Streak") ?? habitTaskFeature.GetType().GetProperty("CurrentStreak");
                string streak = streakProp?.GetValue(habitTaskFeature)?.ToString() ?? "0";

                txtFeature.Text = $"🔥 {streak} days streak";
                txtFeature.Foreground = (Brush)new BrushConverter().ConvertFromString("#F97316");
                hasFeature = true;
            }
            else if (task is ListTask listTaskFeature)
            {
                int total = listTaskFeature.Items.Count;
                int completedItems = 0;

                foreach (var item in listTaskFeature.Items)
                {
                    var isCompProp = item.GetType().GetProperty("IsCompleted") ?? item.GetType().GetProperty("IsDone");
                    
                    if (isCompProp != null && isCompProp.PropertyType == typeof(bool) && (bool)isCompProp.GetValue(item))
                        completedItems++;
                }

                Brush listColor = (completedItems == total && total > 0) ? (Brush)new BrushConverter().ConvertFromString("#10B981") : (Brush)new BrushConverter().ConvertFromString("#6B7280");

                txtFeature.Text = $"📋 {completedItems}/{total} complete";
                txtFeature.Foreground = listColor;
                hasFeature = true;
            }
            else if (task is UrgentTask urgentTaskFeature)
            {
                TimeSpan timeLeft = urgentTaskFeature.Deadline - DateTime.Now;

                if (timeLeft.TotalSeconds < 0)
                {
                    txtFeature.Text = "🚨 Overdue";
                    txtFeature.Foreground = Brushes.Red;
                    txtFeature.FontWeight = FontWeights.Bold;
                }
                else
                {
                    int hours = (int)timeLeft.TotalHours;
                    int minutes = timeLeft.Minutes;
                    txtFeature.Text = $"⏳ {hours}h {minutes}m left";
                    txtFeature.Foreground = (Brush)new BrushConverter().ConvertFromString("#EF4444");
                }
                hasFeature = true;
            }

            if (hasFeature)
                typeAndFeaturePanel.Children.Add(txtFeature);

            textPanel.Children.Add(typeAndFeaturePanel);

            Grid.SetColumn(textPanel, 1);
            topGrid.Children.Add(textPanel);

            StackPanel actionsPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center };

            Button btnComplete = new Button
            {
                Content = isCompleted ? "☑" : "☐",
                FontSize = 12, 
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Width = 28,
                Height = 28
            };

            btnComplete.Click += onCompleteClick;
            actionsPanel.Children.Add(btnComplete);

            Button btnEdit = new Button 
            { 
                Content = "✏️", FontSize = 12, 
                Background = Brushes.Transparent, 
                BorderThickness = new Thickness(0), 
                Cursor = System.Windows.Input.Cursors.Hand, 
                Width = 28, 
                Height = 28 
            };

            btnEdit.Click += onEditClick;
            actionsPanel.Children.Add(btnEdit);

            Button btnExpand = new Button
            {
                Content = "▼",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Width = 28,
                Height = 28,
                Foreground = (Brush)new BrushConverter().ConvertFromString("#4B5563")
            };

            btnExpand.Click += (s, e) => onToggleExpand(cardBorder, btnExpand);
            actionsPanel.Children.Add(btnExpand);

            Button btnDelete = new Button
            {
                Content = "🗑️",
                FontSize = 12,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Width = 28,
                Height = 28
            };

            btnDelete.Click += onDeleteClick;
            actionsPanel.Children.Add(btnDelete);

            Grid.SetColumn(actionsPanel, 2);
            topGrid.Children.Add(actionsPanel);

            cardLayout.Children.Add(topGrid);

            StackPanel detailsPanel = new StackPanel { Name = "DetailsPanel", Margin = new Thickness(40, 10, 0, 0), Visibility = Visibility.Collapsed };

            if (!string.IsNullOrWhiteSpace(task.Description))
            {
                detailsPanel.Children.Add(new TextBlock
                {
                    Text = $"📄 Description: {task.Description}",
                    FontSize = 12,
                    FontStyle = FontStyles.Italic,
                    Foreground = (Brush)new BrushConverter().ConvertFromString("#4B5563"),
                    Margin = new Thickness(0, 0, 0, 8),
                    TextWrapping = TextWrapping.Wrap
                });
            }

            if (task is UrgentTask urgentTaskDetails)
            {
                detailsPanel.Children.Add(CreateDetailLine("⏰ Deadline:", urgentTaskDetails.Deadline.ToString("yyyy-MM-dd HH:mm")));
            }
            else if (task is MeetingTask meetingTaskDetails)
            {
                detailsPanel.Children.Add(CreateDetailLine("⏰ Reminder:", meetingTaskDetails.ReminderTime.ToString("yyyy-MM-dd HH:mm")));

                if (!string.IsNullOrEmpty(meetingTaskDetails.Location))
                    detailsPanel.Children.Add(CreateDetailLine("📍 Location:", meetingTaskDetails.Location));
            }
            else if (task is CallTask callTaskDetails)
            {
                detailsPanel.Children.Add(CreateDetailLine("👤 Contact:", callTaskDetails.ContactName));
                detailsPanel.Children.Add(CreateDetailLine("📞 Phone:", callTaskDetails.PhoneNumber));
                detailsPanel.Children.Add(CreateDetailLine("🌐 Platform:", callTaskDetails.Platform));
                detailsPanel.Children.Add(CreateDetailLine("⏰ Reminder:", callTaskDetails.ReminderTime.ToString("yyyy-MM-dd HH:mm")));
            }
            else if (task is FocusTask focusTaskDetails)
            {
                detailsPanel.Children.Add(CreateDetailLine("⏱️ Duration:", $"{focusTaskDetails.EstimatedDuration.TotalMinutes} minutes"));
            }
            else if (task is ListTask listTaskDetails)
            {
                foreach (var item in listTaskDetails.Items)
                {
                    string itemText = item.ToString();
                    var textProp = item.GetType().GetProperty("Text") ?? item.GetType().GetProperty("Name") ?? item.GetType().GetProperty("Title") ?? item.GetType().GetProperty("Content");
                    
                    if (textProp != null) 
                        itemText = textProp.GetValue(item)?.ToString() ?? itemText;

                    var isCompletedProp = item.GetType().GetProperty("IsCompleted") ?? item.GetType().GetProperty("IsDone");
                    bool isItemCompleted = false;
                    
                    if (isCompletedProp != null && isCompletedProp.PropertyType == typeof(bool))
                        isItemCompleted = (bool)isCompletedProp.GetValue(item);

                    CheckBox chkItem = new CheckBox
                    {
                        IsChecked = isItemCompleted,
                        Margin = new Thickness(0, 3, 0, 3),
                        FontSize = 12,
                        Cursor = System.Windows.Input.Cursors.Hand,
                        Foreground = isItemCompleted ? Brushes.Gray : Brushes.Black,
                        Content = isItemCompleted ? new TextBlock { Text = itemText, TextDecorations = TextDecorations.Strikethrough } : (object)itemText
                    };

                    chkItem.Click += (s, e) =>
                    {
                        bool isChecked = chkItem.IsChecked == true;

                        if (isCompletedProp != null && isCompletedProp.CanWrite)
                            isCompletedProp.SetValue(item, isChecked);

                        bool allCompleted = true;
                        foreach (var childItem in listTaskDetails.Items)
                        {
                            var childStatus = childItem.GetType().GetProperty("IsCompleted") ?? childItem.GetType().GetProperty("IsDone");
                            if (childStatus != null && !(bool)childStatus.GetValue(childItem))
                                allCompleted = false; 
                            break;
                        }

                        if (task is ICompletable comp)
                        {
                            var parentProp = comp.GetType().GetProperty("IsCompleted");
                            if (parentProp != null && parentProp.CanWrite)
                                parentProp.SetValue(comp, allCompleted);
                        }

                        onSubTaskChanged?.Invoke();
                    };
                    detailsPanel.Children.Add(chkItem);
                }
            }
            else if (task is HabitTask habitTaskDetails)
            {
                var streakProp = habitTaskDetails.GetType().GetProperty("Streak") ?? habitTaskDetails.GetType().GetProperty("CurrentStreak");
                string streak = streakProp?.GetValue(habitTaskDetails)?.ToString() ?? "0";
                detailsPanel.Children.Add(CreateDetailLine("🔥 Current streak:", $"{streak} days"));
            }

            detailsPanel.Children.Add(new TextBlock 
            { 
                Text = $"Created: {task.CreatedAt.ToString("yyyy-MM-dd HH:mm")}", 
                FontSize = 10, 
                Foreground = Brushes.LightGray, 
                Margin = new Thickness(0, 5, 0, 0) 
            });

            cardLayout.Children.Add(detailsPanel);
            cardBorder.Child = cardLayout;

            return cardBorder;
        }

        private static StackPanel CreateDetailLine(string label, string value)
        {
            StackPanel line = new StackPanel 
            { 
                Orientation = Orientation.Horizontal, 
                Margin = new Thickness(0, 2, 0, 2) 
            };
            line.Children.Add(new TextBlock 
            { 
                Text = label + " ", 
                FontWeight = FontWeights.Medium, 
                FontSize = 12, Foreground = (Brush)new BrushConverter().ConvertFromString("#374151") 
            });
            line.Children.Add(new TextBlock 
            { 
                Text = value, 
                FontSize = 12, 
                Foreground = (Brush)new BrushConverter().ConvertFromString("#4B5563") 
            });
            return line;
        }
    }
}
