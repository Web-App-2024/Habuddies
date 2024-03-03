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

        public static readonly Dictionary<string, ICategoryData> DataDict = new Dictionary<string, ICategoryData>(){
            { 
                Meeting, new CategoryData {
                    IconHtml = "<i class=\"fa-solid fa-heart\"></i>", 
                    ColorHex = "#f6678b"
                } 
            },
            { 
                Exercise, new CategoryData {
                    IconHtml = "<i class=\"fa-solid fa-dumbbell\"></i>", 
                    ColorHex = "#72c357"
                } 
            }
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