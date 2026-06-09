using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ZenTask.WPF.UIConfig;

namespace ZenTask.WPF
{
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            LoadDynamicUI();
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
                var rootUI = builder.BuildElement(windowConfig.RootElement);
                this.Content = rootUI;
                var btnAddTask = LogicalTreeHelper.FindLogicalNode(rootUI, "BtnAddTask") as Button;

                if (btnAddTask != null)
                {
                    btnAddTask.Click += (s, e) =>
                    {
                        var addWindow = new AddTaskWindow();
                        addWindow.Owner = this;
                        addWindow.ShowDialog();
                    };
                }
            }
        }
    }
}