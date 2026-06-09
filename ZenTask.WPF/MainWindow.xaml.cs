using System.IO;
using System.Windows;
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
            var windowConfig = builder.LoadConfig(jsonPath);

            this.Title = windowConfig.Title;
            this.Width = windowConfig.Width;
            this.Height = windowConfig.Height;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.ResizeMode = ResizeMode.NoResize;
            if (!string.IsNullOrEmpty(windowConfig.Background))
                this.Background = (Brush)new BrushConverter().ConvertFromString(windowConfig.Background);

            if (windowConfig.RootElement != null)
                this.Content = builder.BuildElement(windowConfig.RootElement);
        }
    }
}