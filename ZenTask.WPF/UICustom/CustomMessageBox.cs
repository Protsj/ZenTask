using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ZenTask.WPF.UICustom
{
    public class CustomMessageBox : Window
    {
        public MessageBoxResult Result { get; private set; } = MessageBoxResult.None;

        private CustomMessageBox(string message, string title, MessageBoxButton button)
        {
            this.Title = title;
            this.Width = 380;
            this.SizeToContent = SizeToContent.Height;
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.AllowsTransparency = true;
            this.Background = Brushes.Transparent;
            this.Topmost = true;

            BuildUI(message, title, button);
        }

        private void BuildUI(string message, string title, MessageBoxButton button)
        {
            Border mainBorder = new Border
            {
                Background = (Brush)new BrushConverter().ConvertFromString("#1F2937"), // Темний фон, як у Pomodoro
                BorderBrush = (Brush)new BrushConverter().ConvertFromString("#374151"),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(20)
            };

            StackPanel rootPanel = new StackPanel();

            TextBlock txtTitle = new TextBlock
            {
                Text = title,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Margin = new Thickness(0, 0, 0, 15)
            };
            rootPanel.Children.Add(txtTitle);

            TextBlock txtMessage = new TextBlock
            {
                Text = message,
                FontSize = 13,
                Foreground = (Brush)new BrushConverter().ConvertFromString("#D1D5DB"),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 25)
            };
            rootPanel.Children.Add(txtMessage);

            StackPanel buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            if (button == MessageBoxButton.YesNo)
            {
                Button btnNo = CreateButton("No", "SecondaryPillButton");
                btnNo.Click += (s, e) => { Result = MessageBoxResult.No; this.Close(); };
                buttonPanel.Children.Add(btnNo);

                Button btnYes = CreateButton("Yes", "PrimaryPillButton");
                btnYes.Click += (s, e) => { Result = MessageBoxResult.Yes; this.Close(); };
                buttonPanel.Children.Add(btnYes);
            }
            else
            {
                Button btnOk = CreateButton("OK", "PrimaryPillButton");
                btnOk.Click += (s, e) => { Result = MessageBoxResult.OK; this.Close(); };
                buttonPanel.Children.Add(btnOk);
            }

            rootPanel.Children.Add(buttonPanel);
            mainBorder.Child = rootPanel;
            this.Content = mainBorder;
        }

        private Button CreateButton(string text, string styleName)
        {
            Button btn = new Button
            {
                Content = text,
                Width = 90,
                Height = 32,
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(10, 0, 0, 0)
            };
            btn.Style = (Style)Application.Current.Resources[styleName];
            return btn;
        }

        public static MessageBoxResult Show(string message, string title, MessageBoxButton button = MessageBoxButton.OK)
        {
            var msgBox = new CustomMessageBox(message, title, button);
            msgBox.ShowDialog();
            return msgBox.Result;
        }
    }
}
