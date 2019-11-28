using System.Text.RegularExpressions;

namespace server {
    public class Utils {
        public static string SanitizeString(string str) =>
            Regex.Replace(str, "[^a-zA-Z0-9]+", "", RegexOptions.Compiled);
    }
}