namespace eHub.Android.Models
{
    public class MenuItem
    {
        public string Tag { get; }
        public string Label { get; }
        public int ImageResource { get; }
        public MenuType MenuType { get; }

        public MenuItem(string label, int imageResource, MenuType menuType, string tag)
        {
            Label = label;
            ImageResource = imageResource;
            MenuType = menuType;
            Tag = tag;
        }
    }
}