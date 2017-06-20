using System.IO;
using System.Reflection;

namespace QuickHelp.Formatters
{
    /// <summary>
    /// Provides methods to format a help topic as HTML suitable for embedding
    /// in a WebBrowser control.
    /// </summary>
    public class EmbeddedHtmlFormatter : HtmlFormatter
    {
        private static string s_styleSheet = LoadStyleSheet();

        private static string LoadStyleSheet()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "QuickHelp.Formatters.Default.css";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        protected override string FormatUri(HelpTopic topic, HelpUri uri)
        {
            return "?" + Escape(uri.ToString());
        }

        protected override string GetStyleSheet()
        {
            return string.Format("    <style>\n{0}\n    </style>\n", s_styleSheet);
        }
    }
}
