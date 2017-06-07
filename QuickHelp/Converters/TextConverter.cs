using System;
using System.Text;

namespace QuickHelp.Converters
{
    /// <summary>
    /// Provides methods to convert help topics to plain text.
    /// </summary>
    public class TextConverter
    {
        public static string ConvertTopic(HelpTopic topic)
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
