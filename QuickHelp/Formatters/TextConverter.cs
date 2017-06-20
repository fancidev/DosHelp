using System;
using System.Text;

namespace QuickHelp.Formatters
{
    /// <summary>
    /// Provides methods to format a help topic as plain text.
    /// </summary>
    public class TextFormatter
    {
        public static string FormatTopic(HelpTopic topic)
        {
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            StringBuilder sb = new StringBuilder();
            foreach (HelpLine line in topic.Lines)
            {
                sb.AppendLine(line.Text);
            }
            return sb.ToString();
        }
    }
}
