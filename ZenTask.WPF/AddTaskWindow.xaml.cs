using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ZenTask.WPF.UIConfig;

namespace ZenTask.WPF
{
    public partial class AddTaskWindow : Window
    {
        private UIBuilder _builder = new UIBuilder();
        private StackPanel _listItemsContainer;
        private List<Grid> _listItemRows = new List<Grid>();

        public AddTaskWindow()
        {
            InitializeComponent();

            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
            this.AllowsTransparency = true;
            this.Background = Brushes.Transparent;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            ShowTypeSelectionScreen();
        }

        private Button CreateFloatingCloseButton()
        {
            Button btnClose = new Button
            {
                Content = "✕",
                FontSize = 14,
                Foreground = (Brush)new BrushConverter().ConvertFromString("#9CA3AF"),
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 10, 10, 0),
                Cursor = Cursors.Hand,
                Width = 25,
                Height = 25
            };
            btnClose.Click += (s, e) => this.Close();
            return btnClose;
        }

        private TextBlock CreateLabel(string text)
        {
            return new TextBlock { Text = text, Foreground = (Brush)new BrushConverter().ConvertFromString("#4B5563"), FontSize = 12, FontWeight = FontWeights.Medium, Margin = new Thickness(0, 4, 0, 2) };
        }

        private TextBlock CreateErrorBlock(string name, string errorMessage)
        {
            return new TextBlock { Name = name, Text = errorMessage, Foreground = Brushes.Red, FontSize = 10, Margin = new Thickness(2, 0, 0, 2), Visibility = Visibility.Collapsed };
        }

        private StackPanel CreateDateTimePicker(string namePrefix)
        {
            StackPanel timePanel = new StackPanel { Orientation = Orientation.Horizontal };

            var datePicker = (DatePicker)_builder.BuildElement(new ElementConfig { Type = "DatePicker", Name = $"{namePrefix}Date", Width = 230, Height = 30 });
            timePanel.Children.Add(datePicker);

            var cboHours = new ComboBox { Name = $"{namePrefix}Hours", Width = 60, Height = 30, Margin = new Thickness(10, 0, 0, 0), VerticalContentAlignment = VerticalAlignment.Center };
            for (int i = 0; i < 24; i++) cboHours.Items.Add(i.ToString("D2"));
            cboHours.SelectedIndex = 9;
            timePanel.Children.Add(cboHours);

            var cboMinutes = new ComboBox { Name = $"{namePrefix}Minutes", Width = 60, Height = 30, Margin = new Thickness(5, 0, 0, 0), VerticalContentAlignment = VerticalAlignment.Center };
            for (int i = 0; i < 60; i += 5) cboMinutes.Items.Add(i.ToString("D2"));
            cboMinutes.SelectedIndex = 0;
            timePanel.Children.Add(cboMinutes);

            return timePanel;
        }

        private void Tile_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border tile)
            {
                string tileName = tile.Name;
                Dispatcher.BeginInvoke(new Action(() => ShowInputFormScreen(tileName)));
            }
        }

        private void ShowTypeSelectionScreen()
        {
            string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "add_task_window.json");
            var windowConfig = _builder.LoadAddWindowConfig(jsonPath);

            this.Width = windowConfig.Width;
            this.Height = windowConfig.Height;

            Border windowBorder = new Border
            {
                BorderBrush = (Brush)new BrushConverter().ConvertFromString("#E5E7EB"),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Background = (Brush)new BrushConverter().ConvertFromString(windowConfig.Background)
            };

            Grid mainLayout = new Grid();
            var rootUi = _builder.BuildElement(windowConfig.RootElement);

            mainLayout.Children.Add(rootUi);
            mainLayout.Children.Add(CreateFloatingCloseButton());

            windowBorder.Child = mainLayout;
            this.Content = windowBorder;

            string[] tileNames = { "TileCall", "TileFocus", "TileList", "TileHabit", "TileMeeting", "TileUrgent" };
            foreach (var name in tileNames)
            {
                var tile = LogicalTreeHelper.FindLogicalNode(rootUi, name) as Border;
                if (tile != null)
                {
                    tile.Cursor = Cursors.Hand;
                    tile.MouseDown += Tile_MouseDown;
                }
            }
        }

        private void ShowInputFormScreen(string taskTypeTileName)
        {
            string cleanTypeName = taskTypeTileName.Replace("Tile", "");

            this.Width = 420;
            this.Height = 550;

            Border windowBorder = new Border
            {
                BorderBrush = (Brush)new BrushConverter().ConvertFromString("#E5E7EB"),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Background = Brushes.White
            };

            Grid mainLayout = new Grid();
            StackPanel formPanel = new StackPanel { Margin = new Thickness(25, 25, 25, 20) };

            formPanel.Children.Add(new TextBlock
            {
                Text = $"New {cleanTypeName} Task",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)new BrushConverter().ConvertFromString("#111827"),
                Margin = new Thickness(0, 0, 0, 10)
            });

            formPanel.Children.Add(CreateLabel("Task title*"));
            var txtTitle = (TextBox)_builder.BuildElement(new ElementConfig { Type = "TextBox", Name = "TxtTitle", Height = 30 });
            formPanel.Children.Add(txtTitle);
            var errTitle = CreateErrorBlock("TxtTitleError", "This field is required!");
            formPanel.Children.Add(errTitle);

            txtTitle.TextChanged += (s, e) => {
                if (!string.IsNullOrWhiteSpace(txtTitle.Text))
                    errTitle.Visibility = Visibility.Collapsed;
            };

            formPanel.Children.Add(CreateLabel("Task description"));
            var txtDesc = (TextBox)_builder.BuildElement(new ElementConfig { Type = "TextBox", Name = "TxtDesc", Height = 45 });
            formPanel.Children.Add(txtDesc);

            var fieldsToValidate = new Dictionary<string, Control>();
            var dynamicErrorBlocks = new Dictionary<string, TextBlock>();

            // Specific fields based on task type
            switch (taskTypeTileName)
            {
                case "TileCall":
                    formPanel.Children.Add(CreateLabel("Contact name*"));
                    var txtContact = (TextBox)_builder.BuildElement(new ElementConfig { Type = "TextBox", Name = "TxtContact", Height = 30 });
                    formPanel.Children.Add(txtContact);
                    var errContact = CreateErrorBlock("TxtContactError", "Enter contact name!");
                    formPanel.Children.Add(errContact);
                    fieldsToValidate.Add("Contact", txtContact);
                    dynamicErrorBlocks.Add("Contact", errContact);

                    txtContact.TextChanged += (s, e) => {
                        if (!string.IsNullOrWhiteSpace(txtContact.Text))
                            errContact.Visibility = Visibility.Collapsed;
                    };

                    formPanel.Children.Add(CreateLabel("Phone number*"));
                    var txtPhone = (TextBox)_builder.BuildElement(new ElementConfig { Type = "TextBox", Name = "TxtPhone", Height = 30 });
                    formPanel.Children.Add(txtPhone);
                    var errPhone = CreateErrorBlock("TxtPhoneError", "Enter phone number!");
                    formPanel.Children.Add(errPhone);
                    fieldsToValidate.Add("Phone", txtPhone);
                    dynamicErrorBlocks.Add("Phone", errPhone);

                    txtPhone.TextChanged += (s, e) => {
                        if (!string.IsNullOrWhiteSpace(txtPhone.Text))
                            errPhone.Visibility = Visibility.Collapsed;
                    };

                    formPanel.Children.Add(CreateLabel("Platform (by default: Phone)"));
                    var txtPlatform = (TextBox)_builder.BuildElement(new ElementConfig { Type = "TextBox", Name = "TxtPlatform", Height = 30 });
                    formPanel.Children.Add(txtPlatform);

                    formPanel.Children.Add(CreateLabel("Scheduled time*"));
                    formPanel.Children.Add(CreateDateTimePicker("Call"));
                    break;

                case "TileFocus":
                    formPanel.Children.Add(CreateLabel("Estimated duration* in minutes"));
                    var txtDuration = (TextBox)_builder.BuildElement(new ElementConfig { Type = "TextBox", Name = "TxtDuration", Height = 30 });
                    formPanel.Children.Add(txtDuration);
                    var errDuration = CreateErrorBlock("TxtDurationError", "Enter estimated duration!");
                    formPanel.Children.Add(errDuration);
                    fieldsToValidate.Add("Duration", txtDuration);
                    dynamicErrorBlocks.Add("Duration", errDuration);

                    txtDuration.TextChanged += (s, e) => {
                        if (!string.IsNullOrWhiteSpace(txtDuration.Text) && int.TryParse(txtDuration.Text, out int m) && m > 0)
                            errDuration.Visibility = Visibility.Collapsed;
                    };
                    break;

                case "TileHabit":
                    break;

                case "TileList":
                    formPanel.Children.Add(CreateLabel("Items list*"));
                    ScrollViewer listScrollViewer = new ScrollViewer { Height = 160, VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Margin = new Thickness(0, 2, 0, 5) };

                    _listItemsContainer = new StackPanel { };
                    _listItemRows.Clear();

                    AddListItemRow();

                    listScrollViewer.Content = _listItemsContainer;
                    formPanel.Children.Add(listScrollViewer);

                    Button btnAddSubTask = new Button
                    {
                        Content = "Add item",
                        Background = (Brush)new BrushConverter().ConvertFromString("#F3F4F6"),
                        Foreground = (Brush)new BrushConverter().ConvertFromString("#4B5563"),
                        FontSize = 13,
                        Width = 60,
                        Height = 32,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(0, 8, 0, 10),
                        Cursor = Cursors.Hand
                    };
                    btnAddSubTask.Click += (s, e) => {
                        AddListItemRow();
                        listScrollViewer.ScrollToEnd();
                    };
                    formPanel.Children.Add(btnAddSubTask);
                    break;

                case "TileMeeting":
                    formPanel.Children.Add(CreateLabel("Location"));
                    var txtLocation = (TextBox)_builder.BuildElement(new ElementConfig { Type = "TextBox", Name = "TxtLocation", Height = 30 });
                    formPanel.Children.Add(txtLocation);
                    formPanel.Children.Add(CreateLabel("Scheduled time*"));
                    formPanel.Children.Add(CreateDateTimePicker("Meeting"));
                    break;

                case "TileUrgent":
                    formPanel.Children.Add(CreateLabel("Deadline*"));
                    formPanel.Children.Add(CreateDateTimePicker("Deadline"));
                    break;
            }
            // Action buttons
            Grid buttonsGrid = new Grid { Margin = new Thickness(0, 15, 0, 0), Height = 35 };
            var btnBack = (Button)_builder.BuildElement(new ElementConfig { Type = "Button", Name = "BtnCancel", Content = "Back", Background = "Transparent", Foreground = "#6B7280", Width = 80, HorizontalAlignment = "Left" });
            btnBack.Click += (s, ev) => ShowTypeSelectionScreen();
  
            var btnSave = (Button)_builder.BuildElement(new ElementConfig { Type = "Button", Name = "BtnSave", Content = "Add Task", Background = "#F3F4F6", Foreground = "#111827", Width = 130, HorizontalAlignment = "Right" });

            btnSave.Click += (s, ev) =>
            {
                errTitle.Visibility = Visibility.Collapsed;
                bool hasError = false;

                if (string.IsNullOrWhiteSpace(txtTitle.Text))
                {
                    errTitle.Visibility = Visibility.Visible;
                    hasError = true;
                }

                foreach (var pair in fieldsToValidate)
                {
                    if (pair.Value is TextBox tb)
                    {
                        bool isFieldInvalid = false;

                        if (string.IsNullOrWhiteSpace(tb.Text))
                            isFieldInvalid = true;
                        if (pair.Key == "Duration" && !isFieldInvalid)
                            if (!int.TryParse(tb.Text, out int m) || m <= 0)
                                isFieldInvalid = true;

                        if (isFieldInvalid)
                        {
                            if (dynamicErrorBlocks.TryGetValue(pair.Key, out TextBlock errorBlock))
                            {
                                errorBlock.Visibility = Visibility.Visible;
                                hasError = true;
                            }
                        }
                    }
                }

                if (taskTypeTileName == "TileList" && _listItemRows.Count > 0)
                {
                    var firstInput = LogicalTreeHelper.FindLogicalNode(_listItemRows[0], "TxtItemInput") as TextBox;
                    if (firstInput != null && string.IsNullOrWhiteSpace(firstInput.Text))
                        hasError = true;
                }
                if (hasError) return;
                this.Close();
            };

            buttonsGrid.Children.Add(btnBack);
            buttonsGrid.Children.Add(btnSave);
            formPanel.Children.Add(buttonsGrid);

            mainLayout.Children.Add(formPanel);
            mainLayout.Children.Add(CreateFloatingCloseButton());

            windowBorder.Child = mainLayout;
            this.Content = windowBorder;
        }

        private void AddListItemRow()
        {
            Grid rowGrid = new Grid { Margin = new Thickness(0, 0, 0, 6) };
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(35) });

            TextBox itemInput = new TextBox
            {
                Name = "TxtItemInput",
                Height = 28,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(5, 0, 5, 0),
                Background = (Brush)new BrushConverter().ConvertFromString("#F3F4F6"),
                BorderBrush = (Brush)new BrushConverter().ConvertFromString("#E5E7EB"),
                BorderThickness = new Thickness(1)
            };
            Grid.SetColumn(itemInput, 0);
            rowGrid.Children.Add(itemInput);

            Button btnDelete = new Button
            {
                Name = "BtnDeleteItem",
                Content = "🗑",
                Width = 28,
                Height = 28,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            btnDelete.Click += (s, e) => {
                _listItemsContainer.Children.Remove(rowGrid);
                _listItemRows.Remove(rowGrid);
                UpdateTrashButtonsVisibility();
            };

            Grid.SetColumn(btnDelete, 1);
            rowGrid.Children.Add(btnDelete);

            _listItemsContainer.Children.Add(rowGrid);
            _listItemRows.Add(rowGrid);

            UpdateTrashButtonsVisibility();
        }

        private void UpdateTrashButtonsVisibility()
        {
            if (_listItemRows.Count <= 1)
            {
                foreach (var row in _listItemRows)
                {
                    var trashBtn = LogicalTreeHelper.FindLogicalNode(row, "BtnDeleteItem") as Button;
                    if (trashBtn != null) trashBtn.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                foreach (var row in _listItemRows)
                {
                    var trashBtn = LogicalTreeHelper.FindLogicalNode(row, "BtnDeleteItem") as Button;
                    if (trashBtn != null) trashBtn.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
