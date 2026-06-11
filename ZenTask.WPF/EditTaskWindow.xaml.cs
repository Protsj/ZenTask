using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ZenTask.Core.Data;
using ZenTask.Core.Models;
using ZenTask.Core.Services;
using ZenTask.WPF.UIConfig;

namespace ZenTask.WPF
{
    public partial class EditTaskWindow : Window
    {
        private UIBuilder _builder = new UIBuilder();
        private TaskManager _taskManager;
        private SqliteTaskStorage _taskStorage;
        private BaseTask _taskToEdit;
        private Dictionary<string, Control> _fieldsToValidate = new Dictionary<string, Control>();
        private StackPanel _formPanel;
        private StackPanel _listItemsContainer;
        private List<Grid> _listItemRows = new List<Grid>();

        public EditTaskWindow(TaskManager taskManager, SqliteTaskStorage taskStorage, BaseTask taskToEdit)
        {
            _taskManager = taskManager;
            _taskStorage = taskStorage;
            _taskToEdit = taskToEdit;

            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
            this.AllowsTransparency = true;
            this.Background = Brushes.Transparent;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            BuildEditForm();
        }

        private void BuildEditForm()
        {
            this.Width = 420;
            this.Height = 550;

            Border windowBorder = new Border
            {
                BorderBrush = (Brush)new BrushConverter().ConvertFromString("#E5E7EB"),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Background = (Brush)new BrushConverter().ConvertFromString("#F9FAFB")
            };

            Grid mainLayout = new Grid();
            _formPanel = new StackPanel 
            { 
                Margin = new Thickness(25, 25, 25, 20) 
            };

            string typeName = _taskToEdit.GetType().Name.Replace("Task", "");

            _formPanel.Children.Add(new TextBlock
            {
                Text = $"Edit {typeName} Task",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)new BrushConverter().ConvertFromString("#111827"),
                Margin = new Thickness(0, 0, 0, 10)
            });

            _formPanel.Children.Add(CreateLabel("Task title*"));

            var txtTitle = (TextBox)_builder.BuildElement(new ElementConfig 
            { 
                Type = "TextBox", 
                Name = "TxtTitle", 
                Height = 30 
            });

            txtTitle.Text = _taskToEdit.Title;
            _formPanel.Children.Add(txtTitle);

            _formPanel.Children.Add(CreateLabel("Description"));
            
            var txtDesc = (TextBox)_builder.BuildElement(new ElementConfig 
            { 
                Type = "TextBox", 
                Name = "TxtDesc", 
                Height = 45 
            });
            
            txtDesc.Text = _taskToEdit.Description;
            _formPanel.Children.Add(txtDesc);

            if (_taskToEdit is UrgentTask urgent)
            {
                _formPanel.Children.Add(CreateLabel("Deadline*"));
                _formPanel.Children.Add(CreateDateTimePicker("Urgent"));
                SetDateTimePickerValue("Urgent", urgent.ReminderTime);
            }
            else if (_taskToEdit is MeetingTask meeting)
            {
                _formPanel.Children.Add(CreateLabel("Location"));

                var txtLocation = (TextBox)_builder.BuildElement(new ElementConfig 
                { 
                    Type = "TextBox", 
                    Name = "TxtLocation", 
                    Height = 30 
                });

                txtLocation.Text = meeting.Location;
                _formPanel.Children.Add(txtLocation);

                _formPanel.Children.Add(CreateLabel("Scheduled time*"));
                _formPanel.Children.Add(CreateDateTimePicker("Meeting"));
                SetDateTimePickerValue("Meeting", meeting.ReminderTime);
            }
            else if (_taskToEdit is FocusTask focus)
            {
                _formPanel.Children.Add(CreateLabel("Estimated duration* in minutes"));

                var txtDuration = (TextBox)_builder.BuildElement(new ElementConfig 
                { 
                    Type = "TextBox", 
                    Name = "TxtDuration", 
                    Height = 30 
                });

                txtDuration.Text = focus.EstimatedDuration.TotalMinutes.ToString();
                _formPanel.Children.Add(txtDuration);
                _fieldsToValidate.Add("Duration", txtDuration);
            }
            else if (_taskToEdit is CallTask call)
            {
                _formPanel.Children.Add(CreateLabel("Contact name*"));

                var txtContact = (TextBox)_builder.BuildElement(new ElementConfig 
                { 
                    Type = "TextBox", 
                    Name = "TxtContact", 
                    Height = 30 
                });

                txtContact.Text = call.ContactName;
                _formPanel.Children.Add(txtContact);

                _formPanel.Children.Add(CreateLabel("Phone number*"));

                var txtPhone = (TextBox)_builder.BuildElement(new ElementConfig 
                { 
                    Type = "TextBox", 
                    Name = "TxtPhone", 
                    Height = 30 
                });

                txtPhone.Text = call.PhoneNumber;
                _formPanel.Children.Add(txtPhone);

                _formPanel.Children.Add(CreateLabel("Scheduled time*"));
                _formPanel.Children.Add(CreateDateTimePicker("Call"));
                SetDateTimePickerValue("Call", call.ReminderTime);
            }
            else if (_taskToEdit is ListTask listTask)
            {
                _formPanel.Children.Add(CreateLabel("Items list*"));

                ScrollViewer listScrollViewer = new ScrollViewer 
                { 
                    Height = 160, 
                    VerticalScrollBarVisibility = ScrollBarVisibility.Hidden, 
                    Margin = new Thickness(0, 2, 0, 5) 
                };

                _listItemsContainer = new StackPanel();
                _listItemRows.Clear();

                foreach (var item in listTask.Items)
                {
                    string text = item.ToString();
                    var textProp = item.GetType().GetProperty("Text") ?? item.GetType().GetProperty("Name") ?? item.GetType().GetProperty("Title") ?? item.GetType().GetProperty("Content");
                    
                    if (textProp != null) 
                        text = textProp.GetValue(item)?.ToString() ?? text;

                    AddListItemRow(text);
                }

                if (_listItemRows.Count == 0) 
                    AddListItemRow("");

                listScrollViewer.Content = _listItemsContainer;
                _formPanel.Children.Add(listScrollViewer);

                Button btnAddSubTask = new Button 
                { 
                    Content = "+ Add item", 
                    Background = (Brush)new BrushConverter().ConvertFromString("#F3F4F6"), 
                    Foreground = (Brush)new BrushConverter().ConvertFromString("#4B5563"), 
                    FontSize = 13, 
                    FontWeight = FontWeights.Medium, 
                    Width = 80, 
                    Height = 32, 
                    HorizontalAlignment = HorizontalAlignment.Left, 
                    Margin = new Thickness(0, 8, 0, 10), 
                    Cursor = Cursors.Hand 
                };

                btnAddSubTask.Click += (s, e) => { AddListItemRow(""); listScrollViewer.ScrollToEnd(); };
                _formPanel.Children.Add(btnAddSubTask);
            }

            Grid buttonsGrid = new Grid 
            { 
                Margin = new Thickness(0, 15, 0, 0), 
                Height = 35 
            };

            var btnCancel = (Button)_builder.BuildElement(new ElementConfig 
            { 
                Type = "Button", 
                Name = "BtnCancel", 
                Content = "Cancel", 
                Background = "Transparent", 
                Foreground = "#6B7280", 
                Width = 80, 
                HorizontalAlignment = "Left" 
            });

            btnCancel.Click += (s, ev) => this.Close();

            var btnSave = (Button)_builder.BuildElement(new ElementConfig 
            { 
                Type = "Button", 
                Name = "BtnSave", 
                Content = "Save Changes", 
                Background = "#3B82F6", 
                Foreground = "#FFFFFF", 
                Width = 130, 
                HorizontalAlignment = "Right" 
            });

            btnSave.Click += async (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(txtTitle.Text))
                {
                    MessageBox.Show("Title is required!"); 
                    return;
                }

                _taskToEdit.Title = txtTitle.Text.Trim();
                _taskToEdit.Description = txtDesc.Text.Trim();

                if (_taskToEdit is UrgentTask u)
                {
                    DateTime newDeadline = ExtractDateTime("Urgent");
                    if (u.ReminderTime != newDeadline)
                    {
                        u.ReminderTime = newDeadline;
                        u.IsReminderSent = false;
                    }
                }
                else if (_taskToEdit is MeetingTask m)
                {
                    var locNode = LogicalTreeHelper.FindLogicalNode(_formPanel, "TxtLocation") as TextBox;
                    m.Location = locNode?.Text;

                    DateTime newTime = ExtractDateTime("Meeting");
                    if (m.ReminderTime != newTime)
                    {
                        m.ReminderTime = newTime;
                        m.IsReminderSent = false;
                    }
                }
                else if (_taskToEdit is FocusTask f)
                {
                    var durNode = LogicalTreeHelper.FindLogicalNode(_formPanel, "TxtDuration") as TextBox;

                    if (int.TryParse(durNode?.Text, out int mins))
                        f.EstimatedDuration = TimeSpan.FromMinutes(mins);
                }
                else if (_taskToEdit is CallTask c)
                {
                    var contactNode = LogicalTreeHelper.FindLogicalNode(_formPanel, "TxtContact") as TextBox;
                    var phoneNode = LogicalTreeHelper.FindLogicalNode(_formPanel, "TxtPhone") as TextBox;
                    c.ContactName = contactNode?.Text;
                    c.PhoneNumber = phoneNode?.Text;

                    DateTime newTime = ExtractDateTime("Call");
                    if (c.ReminderTime != newTime)
                    {
                        c.ReminderTime = newTime;
                        c.IsReminderSent = false;
                    }
                }
                else if (_taskToEdit is ListTask l)
                {
                    var fields = l.GetType().GetFields(System.Reflection.BindingFlags.NonPublic |
                                                       System.Reflection.BindingFlags.Instance |
                                                       System.Reflection.BindingFlags.Public);

                    foreach (var field in fields)
                    {
                        var val = field.GetValue(l);

                        if (val is System.Collections.IList list && !val.GetType().Name.Contains("ReadOnly"))
                            list.Clear();
                    }

                    foreach (var row in _listItemRows)
                    {
                        var input = LogicalTreeHelper.FindLogicalNode(row, "TxtItemInput") as TextBox;

                        if (input != null && !string.IsNullOrWhiteSpace(input.Text))
                            l.AddItem(input.Text.Trim());
                    }
                }

                try
                {
                    await _taskStorage.DeleteTaskAsync(_taskToEdit.Id);
                    await _taskStorage.SaveTaskAsync(_taskManager.GetTasks());
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating database: {ex.Message}");
                }
            };

            buttonsGrid.Children.Add(btnCancel);
            buttonsGrid.Children.Add(btnSave);
            _formPanel.Children.Add(buttonsGrid);

            mainLayout.Children.Add(_formPanel);

            Button btnClose = new Button 
            { 
                Content = "✕", 
                FontSize = 14, 
                Foreground = Brushes.Gray, 
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
            mainLayout.Children.Add(btnClose);

            windowBorder.Child = mainLayout;
            this.Content = windowBorder;
        }

        private TextBlock CreateLabel(string text) => new TextBlock 
        { 
            Text = text, 
            Foreground = (Brush)new BrushConverter().ConvertFromString("#4B5563"), 
            FontSize = 12, 
            FontWeight = FontWeights.Medium, 
            Margin = new Thickness(0, 10, 0, 4) 
        };

        private StackPanel CreateDateTimePicker(string namePrefix)
        {
            StackPanel timePanel = new StackPanel 
            { 
                Orientation = Orientation.Horizontal 
            };

            var datePicker = (DatePicker)_builder.BuildElement(new ElementConfig 
            { 
                Type = "DatePicker", 
                Name = $"{namePrefix}Date", 
                Width = 230, Height = 30 
            });

            timePanel.Children.Add(datePicker);

            var cboHours = new ComboBox 
            { 
                Name = $"{namePrefix}Hours", 
                Width = 60, 
                Height = 30, 
                Margin = new Thickness(10, 0, 0, 0), 
                VerticalContentAlignment = VerticalAlignment.Center 
            };

            for (int i = 0; i <= 23; i++) 
                cboHours.Items.Add(i.ToString("D2"));
            
            timePanel.Children.Add(cboHours);

            var cboMinutes = new ComboBox 
            { 
                Name = $"{namePrefix}Minutes", 
                Width = 60, 
                Height = 30, 
                Margin = new Thickness(5, 0, 0, 0), 
                VerticalContentAlignment = VerticalAlignment.Center 
            };
            
            for (int i = 0; i <= 59; i++) 
                cboMinutes.Items.Add(i.ToString("D2"));

            timePanel.Children.Add(cboMinutes);

            return timePanel;
        }

        private void SetDateTimePickerValue(string prefix, DateTime time)
        {
            var dp = LogicalTreeHelper.FindLogicalNode(_formPanel, $"{prefix}Date") as DatePicker;
            var cbHours = LogicalTreeHelper.FindLogicalNode(_formPanel, $"{prefix}Hours") as ComboBox;
            var cbMinutes = LogicalTreeHelper.FindLogicalNode(_formPanel, $"{prefix}Minutes") as ComboBox;

            if (dp != null) 
                dp.SelectedDate = time.Date;

            if (cbHours != null) 
                cbHours.SelectedItem = time.Hour.ToString("D2");

            int min = (time.Minute / 5) * 5;

            if (cbMinutes != null) 
                cbMinutes.SelectedItem = min.ToString("D2");
        }

        private DateTime ExtractDateTime(string prefix)
        {
            var dp = LogicalTreeHelper.FindLogicalNode(_formPanel, $"{prefix}Date") as DatePicker;
            var cbHours = LogicalTreeHelper.FindLogicalNode(_formPanel, $"{prefix}Hours") as ComboBox;
            var cbMinutes = LogicalTreeHelper.FindLogicalNode(_formPanel, $"{prefix}Minutes") as ComboBox;

            DateTime date = dp?.SelectedDate ?? DateTime.Today;
            int hours = cbHours?.SelectedItem != null ? int.Parse(cbHours.SelectedItem.ToString()) : 9;
            int minutes = cbMinutes?.SelectedItem != null ? int.Parse(cbMinutes.SelectedItem.ToString()) : 0;

            return new DateTime(date.Year, date.Month, date.Day, hours, minutes, 0);
        }
        private void AddListItemRow(string initialText = "")
        {
            Grid rowGrid = new Grid 
            {
                Margin = new Thickness(0, 0, 0, 6) 
            };

            rowGrid.ColumnDefinitions.Add(new ColumnDefinition 
            { 
                Width = new GridLength(1, GridUnitType.Star) 
            });

            rowGrid.ColumnDefinitions.Add(new ColumnDefinition 
            { 
                Width = new GridLength(35) 
            });

            TextBox itemInput = new TextBox 
            { 
                Name = "TxtItemInput", 
                Text = initialText, 
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
                Content = "🗑️", 
                Width = 28, 
                Height = 28, 
                Background = Brushes.Transparent, 
                BorderThickness = new Thickness(0), 
                Cursor = Cursors.Hand, 
                HorizontalAlignment = HorizontalAlignment.Right 
            };
            
            btnDelete.Click += (s, e) => { _listItemsContainer.Children.Remove(rowGrid); _listItemRows.Remove(rowGrid); UpdateTrashButtonsVisibility(); };
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
                    
                    if (trashBtn != null) 
                        trashBtn.Visibility = Visibility.Collapsed; 
                }
            }
            else
            {
                foreach (var row in _listItemRows) 
                { 
                    var trashBtn = LogicalTreeHelper.FindLogicalNode(row, "BtnDeleteItem") as Button; 
                    
                    if (trashBtn != null) 
                        trashBtn.Visibility = Visibility.Visible; 
                }
            }
        }
    }
}
