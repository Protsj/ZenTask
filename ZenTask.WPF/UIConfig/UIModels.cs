namespace ZenTask.WPF.UIConfig
{
    public class WindowConfig
    {
        public string Title { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Background { get; set; }
        public ElementConfig RootElement { get; set; }
    }

    public class ElementConfig
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Background { get; set; }
        public string Foreground { get; set; }
        public double FontSize { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Margin { get; set; }
        public string Orientation { get; set; }
        public string HorizontalAlignment { get; set; }
        public string VerticalAlignment { get; set; }
        public List<ElementConfig> Children { get; set; } = new List<ElementConfig>();
    }
}
