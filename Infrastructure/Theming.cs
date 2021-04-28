using Client.Models;
using DiffPlex.Model;
using System.Collections.Generic;
using System.Linq;

namespace Client.Infrastructure
{
    public class Themes
    {
        public Theme Dark { get; set; }
        public Theme Light { get; set; }
    }

    public class Theme
    {
        public SideBarOptions SideBarOptions { get; set; }
        public BackgroundOptions BackgroundOptions { get; set; }
        public ColorOptions ColorOptions { get; set; }
    }

    public class SideBarOptions
    {
        public string Color { get; set; }
        public string BackgroundColor { get; set; }
        public string HoverColor { get; set; }
        public string HoverBackgroundColor { get; set; }
        public string ActiveColor { get; set; }
        public string ActiveBackgroundColor { get; set; }
    }

    public class BackgroundOptions
    {
        public string Body { get; set; }
        public string Modal { get; set; }
    }

    public class ColorOptions
    {
        public string Body { get; set; }
        public string Modal { get; set; }
    }

    public enum ThemeType
    {
        Dark,
        Light
    }
}
