using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using ZenTask.Core.Models;

namespace ZenTask.WPF.UIConfig
{
    public class UIBuilder
    {
        public WindowConfig LoadMainWindowConfig(string path)
        {
            if (!File.Exists(path)) GenerateDefaultMainWindowConfig(path);
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<WindowConfig>(json);
        }

        public WindowConfig LoadAddWindowConfig(string path)
        {
            if (!File.Exists(path)) GenerateDefaultAddWindowConfig(path);
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
                    Type = "Border", Height = 1, Background = "#E5E7EB", VerticalAlignment = "Top"
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
                    Type = "Border", Height = 1, Background = "#E5E7EB", VerticalAlignment = "Top", Margin = "0,40,0,0"
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
                    Type = "Border", Height = 1, Background = "#E5E7EB", VerticalAlignment = "Bottom", Margin = "0,0,0,25"
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
                        new ElementConfig { Type = "TextBlock", Foreground = "#111827", FontSize = 18, Margin = "0,0,0,20", HorizontalAlignment = "Center" },
                        
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
                            new ElementConfig { Type = "TextBlock", Content = icon, FontSize = 32, HorizontalAlignment = "Center", Margin = "0,0,0,8" },
                            new ElementConfig { Type = "TextBlock", Content = title, FontSize = 13, Foreground = "#4B5563", HorizontalAlignment = "Center" }
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
                    if (config.Orientation == "Horizontal") panel.Orientation = Orientation.Horizontal;
                    if (!string.IsNullOrEmpty(config.Background))
                        panel.Background = (Brush)new BrushConverter().ConvertFromString(config.Background);

                    foreach (var child in config.Children)
                        panel.Children.Add(BuildElement(child));
                    element = panel;
                    break;

                case "ScrollViewer":
                    var scroll = new ScrollViewer();
                    scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    if (config.Height > 0) scroll.Height = config.Height;

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
                    var txt = new TextBlock { Text = config.Content, VerticalAlignment = VerticalAlignment.Center };
                    if (config.FontSize > 0) txt.FontSize = config.FontSize;
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
                    var btn = new Button { Content = config.Content };
                    if (config.FontSize > 0) btn.FontSize = config.FontSize;
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
                    var uGrid = new UniformGrid { Columns = 2, Rows = 3 };
                    foreach (var child in config.Children) uGrid.Children.Add(BuildElement(child));
                    element = uGrid;
                    break;
                case "DatePicker":
                    var picker = new DatePicker { VerticalContentAlignment = VerticalAlignment.Center };
                    picker.SelectedDate = DateTime.Today;
                    element = picker;
                    break;

                case "ComboBox":
                    var comboBox = new ComboBox { VerticalContentAlignment = VerticalAlignment.Center };
                    foreach (var child in config.Children)
                    {
                        if (child.Type == "ComboBoxItem")
                            comboBox.Items.Add(new ComboBoxItem { Content = child.Content, Padding = new Thickness(5) });
                    }
                    if (comboBox.Items.Count > 0) comboBox.SelectedIndex = 0;
                    element = comboBox;
                    break;

            }

            if (element != null)
            {
                if (!string.IsNullOrEmpty(config.Name)) element.Name = config.Name;
                if (config.Width > 0) element.Width = config.Width;
                if (config.Height > 0) element.Height = config.Height;
                if (!string.IsNullOrEmpty(config.Margin))
                {
                    var m = config.Margin.Split(',');
                    if (m.Length == 4)
                        element.Margin = new Thickness(double.Parse(m[0]), double.Parse(m[1]), double.Parse(m[2]), double.Parse(m[3]));
                }
                if (!string.IsNullOrEmpty(config.HorizontalAlignment))
                {
                    if (Enum.TryParse(config.HorizontalAlignment, out HorizontalAlignment align))
                        element.HorizontalAlignment = align;
                }
                if (!string.IsNullOrEmpty(config.VerticalAlignment))
                {
                    if (Enum.TryParse(config.VerticalAlignment, out VerticalAlignment vAlign))
                        element.VerticalAlignment = vAlign;
                }
            }

            return element;
        }

        public static UIElement CreateTaskCard(BaseTask task, RoutedEventHandler onDeleteClick, RoutedEventHandler onExpandClick)
        {
            Border card = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(0, 0, 0, 10),
                Padding = new Thickness(12),
                BorderBrush = (Brush)new BrushConverter().ConvertFromString("#E5E7EB"),
                BorderThickness = new Thickness(1)
            };

            Grid cardGrid = new Grid();
            cardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
            cardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            cardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
            cardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(65) });

            string icon = "📌";
            string typeName = task.GetType().Name;
            if (task is HabitTask) icon = "🔁";
            else if (task is UrgentTask) icon = "🚨";
            else if (task is FocusTask) icon = "⏱️";
            else if (task is ListTask) icon = "📋";
            else if (task is CallTask) icon = "📞";
            else if (task is MeetingTask) icon = "👥";

            TextBlock txtIcon = new TextBlock { Text = icon, FontSize = 22, VerticalAlignment = VerticalAlignment.Center };
            Grid.SetColumn(txtIcon, 0);
            cardGrid.Children.Add(txtIcon);

            StackPanel textPanel = new StackPanel { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(5, 0, 0, 0) };
            TextBlock txtTitle = new TextBlock { Text = task.Title, FontSize = 15, FontWeight = FontWeights.SemiBold, Foreground = (Brush)new BrushConverter().ConvertFromString("#111827") };
            TextBlock txtType = new TextBlock { Text = typeName, FontSize = 11, Foreground = (Brush)new BrushConverter().ConvertFromString("#6B7280"), Margin = new Thickness(0, 2, 0, 0) };
            textPanel.Children.Add(txtTitle);
            textPanel.Children.Add(txtType);
            Grid.SetColumn(textPanel, 1);
            cardGrid.Children.Add(textPanel);

            string infoText = "";
            if (task is HabitTask habit)
                infoText = $"🔥 {habit.Streak} days";
            else if (task is ListTask listTask)
                infoText = $"✔ {listTask.Items?.Count(i => i.IsDone)}/{listTask.Items?.Count} items";
            else if (task is UrgentTask urgent)
                infoText = $"⏳ {urgent.Deadline:dd.MM}";

            TextBlock txtInfo = new TextBlock { Text = infoText, FontSize = 12, FontWeight = FontWeights.Medium, Foreground = (Brush)new BrushConverter().ConvertFromString("#4B5563"), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 0, 10, 0) };
            Grid.SetColumn(txtInfo, 2);
            cardGrid.Children.Add(txtInfo);

            StackPanel actionPanel = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };

            Button btnExpand = new Button { Content = "👁️", Width = 28, Height = 28, Margin = new Thickness(0, 0, 5, 0), Background = Brushes.Transparent, BorderThickness = new Thickness(0), Cursor = System.Windows.Input.Cursors.Hand };
            btnExpand.Click += onExpandClick;

            Button btnDelete = new Button { Content = "🗑️", Width = 28, Height = 28, Background = Brushes.Transparent, BorderThickness = new Thickness(0), Cursor = System.Windows.Input.Cursors.Hand };
            btnDelete.Click += onDeleteClick;

            actionPanel.Children.Add(btnExpand);
            actionPanel.Children.Add(btnDelete);
            Grid.SetColumn(actionPanel, 3);
            cardGrid.Children.Add(actionPanel);

            card.Child = cardGrid;
            return card;
        }
    }
}
