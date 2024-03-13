namespace HaBuddies.Constants
{
    public static class Category {
        public interface ICategoryData {
            public string? IconHtml { get; }
            public string? ColorHex { get; }
        }

        private class CategoryData : ICategoryData {
            public string? IconHtml { get; init; }
            public string? ColorHex { get; init; }
        }

        public static readonly string Meeting = "Meeting";
        public static readonly string Exercise = "Exercise";
        public static readonly string Working = "Working";
        public static readonly string Eating = "Eating";
        public static readonly string Concert = "Concert";
        public static readonly string Karaoke = "Karaoke";
        public static readonly string Party = "Party";
        public static readonly string Tabletop = "Tabletop";

        public static readonly Dictionary<string, ICategoryData> DataDict = new Dictionary<string, ICategoryData>(){
            { 
                Meeting, new CategoryData {
                    IconHtml = "<i class=\"fa-solid fa-heart\"></i>", 
                    ColorHex = "#ff70f0"
                } 
            },
            { 
                Exercise, new CategoryData {
                    IconHtml = "<i class=\"fa-solid fa-dumbbell\"></i>", 
                    ColorHex = "#72c357"
                } 
            },
            { 
                Working, new CategoryData {
                    IconHtml = "<i class=\"fa-solid fa-briefcase\"></i>", 
                    ColorHex = "#f8b800"
                } 
            },
            { 
                Eating, new CategoryData {
                    IconHtml = "<i class=\"fa-solid fa-utensils\"></i>", 
                    ColorHex = "#ff9135"
                } 
            },
            { 
                Concert, new CategoryData {
                    IconHtml = "<i class=\"fa-solid fa-guitar\"></i>", 
                    ColorHex = "#6382ff"
                } 
            },
            { 
                Karaoke, new CategoryData {
                    IconHtml = "<i class=\"fa-solid fa-music\"></i>", 
                    ColorHex = "#6fd9f5"
                } 
            },
            { 
                Party, new CategoryData {
                    IconHtml = "<i class=\"fa-solid fa-champagne-glasses\"></i>", 
                    ColorHex = "#b369ff"
                } 
            },
            { 
                Tabletop, new CategoryData {
                    IconHtml = "<i class=\"fa-solid fa-chess\"></i>", 
                    ColorHex = "#ff5672"
                } 
            },
        };

        public static string? GetIconHtml(string category) {
            if (!DataDict.TryGetValue(category, out ICategoryData? categoryData))
            {
                return "";
            }
            return categoryData.IconHtml;
        }

        public static string? GetColorHex(string category) {
            if (!DataDict.TryGetValue(category, out ICategoryData? categoryData))
            {
                return "";
            }
            return categoryData.ColorHex;
        }
    }
}