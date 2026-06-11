using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using ZenTask.Core.Models;
using ZenTask.WPF.UICustom;

namespace ZenTask.WPF
{
    public partial class PomodoroTimerWindow : Window
    {
        private DispatcherTimer _timer;
        private TimeSpan _timeLeft;
        private FocusTask _focusTask;
        private Action _onSessionCompleted;

        private TextBlock _txtClock;
        private TextBlock _txtTaskTitle;
        public PomodoroTimerWindow(FocusTask focusTask, Action onSessionCompleted)
        {
            InitializeComponent();
            _focusTask = focusTask;
            _onSessionCompleted = onSessionCompleted;

            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
            this.AllowsTransparency = true;
            this.Background = Brushes.Transparent;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.Width = 350;
            this.Height = 280;

            _timeLeft = focusTask.EstimatedDuration;

            BuildLayout();
            StartTimer();
        }

        private void BuildLayout()
        {
            Border mainBorder = new Border
            {
                Background = (Brush)new BrushConverter().ConvertFromString("#1F2937"),
                BorderBrush = (Brush)new BrushConverter().ConvertFromString("#EF4444"),
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(20)
            };

            StackPanel rootPanel = new StackPanel { VerticalAlignment = VerticalAlignment.Center };

            _txtTaskTitle = new TextBlock
            {
                Text = $"Focus session: {_focusTask.Title}",
                FontSize = 14,
                FontWeight = FontWeights.Medium,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 20)
            };

            rootPanel.Children.Add(_txtTaskTitle);

            _txtClock = new TextBlock
            {
                Text = GetTimerDisplay(),
                FontSize = 48,
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)new BrushConverter().ConvertFromString("#EF4444"),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 25)
            };

            rootPanel.Children.Add(_txtClock);

            Button btnStop = new Button
            {
                Content = "Stop session",
                Cursor = System.Windows.Input.Cursors.Hand
            };

            btnStop.Style = (Style)Application.Current.Resources["SecondaryPillButton"];

            btnStop.Click += (s, e) =>
            {
                var result = CustomMessageBox.Show("Are you sure, you want to stop this session? The progress will be lost.", "Stop Pomodoro", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _timer.Stop();
                    this.Close();
                }
            };

            rootPanel.Children.Add(btnStop);

            mainBorder.Child = rootPanel;
            this.Content = mainBorder;
        }

        private void StartTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_timeLeft.TotalSeconds > 0)
            {
                _timeLeft = _timeLeft.Subtract(TimeSpan.FromSeconds(1));
                _txtClock.Text = GetTimerDisplay();
            }
            else
            {
                _timer.Stop();
                HandleTimerFinished();
            }
        }

        private void HandleTimerFinished()
        {
            System.Media.SystemSounds.Exclamation.Play();

            CustomMessageBox.Show($"Congratulations! You've successfully completed a {_focusTask.EstimatedDuration.TotalMinutes}-minute focus session on task '{_focusTask.Title}'!\n\nOne Pomodoro has been added to your streak.", "Time is up! 🍅", MessageBoxButton.OK);

            var pomoProp = _focusTask.GetType().GetProperty("PomodoroCount") ?? _focusTask.GetType().GetProperty("PomodorosDone") ?? _focusTask.GetType().GetProperty("Count");
            if (pomoProp != null && pomoProp.CanWrite)
            {
                int currentCount = (int)pomoProp.GetValue(_focusTask);
                pomoProp.SetValue(_focusTask, currentCount + 1);
            }

            _onSessionCompleted?.Invoke();
            this.Close();
        }

        private string GetTimerDisplay()
        {
            if (_timeLeft.TotalHours >= 1)
                return $"{(int)_timeLeft.TotalHours}:{_timeLeft.Minutes:D2}:{_timeLeft.Seconds:D2}";
            else
                return $"{(int)_timeLeft.TotalMinutes:D2}:{_timeLeft.Seconds:D2}";
        }
    }
}
