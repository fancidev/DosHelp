using System;
using System.Collections.Generic;
using System.Text;

namespace QuickHelp.Converters
{
    /// <summary>
    /// Provides methods to convert help topics into HTML format.
    /// </summary>
    public class HtmlConverter
    {
        public static readonly HtmlConverter Default = new HtmlConverter
        {
            AutoFixHyperlinks = true,
        };

        /// <summary>
        /// Gets or sets a flag indicating whether to automatically fix
        /// hyperlinks according to one of the following rules:
        /// - If the hyperlink ends with ►, does not start with ◄, and is not
        ///   a single ►, the ending ► is excluded from the hyperlink.
        /// </summary>
        public bool AutoFixHyperlinks { get; set; }

        /// <summary>
        /// Converts the given help topic into HTML format.
        /// </summary>
        /// <param name="topic">The help topic to convert.</param>
        /// <returns>The HTML source for the topic.</returns>
        public string ConvertTopic(HelpTopic topic)
        {
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            StringBuilder html = new StringBuilder();
            html.AppendFormat("<html><head><title>{0}</title></head>\r\n",
                              Escape(topic.Title));
            html.AppendLine("<body><pre>");

            foreach (HelpLine line in topic.Lines)
            {
                FormatLine(html, topic, line);
            }

            html.AppendLine("</pre></body>");
            html.AppendLine("</html>");
            return html.ToString();
        }

        /// <summary>
        /// Converts a help line into HTML format.
        /// </summary>
        /// <returns>HTML representation of the line.</returns>
        /// <remarks>
        /// This method does not guarantee that the generated html is XML
        /// conformant. Consider the following example:
        ///   ...<b>...<i>...</b>...</i>...
        /// If XML conformance is required, this would have to be changed
        /// into
        ///   ...<b>...<i>...</i></b><i>...</i>...
        /// While this could be done, it adds complexity and increases output
        /// size with no added value, since every web browser can handle the
        /// previous markup as expected.
        /// 
        /// For a formal discussion about this issue, see the HTML 5 section
        /// http://www.w3.org/html/wg/drafts/html/master/syntax.html#an-introduction-to-error-handling-and-strange-cases-in-the-parser
        /// </remarks>
        private void FormatLine(StringBuilder html, HelpTopic topic, HelpLine line)
        {
            TextAttribute oldAttrs = TextAttribute.Default;

            for (int i = 0; i < line.Text.Length; i++)
            {
                TextAttribute newAttrs = line.Attributes[i];
                TextStyle addedStyles = newAttrs.Style & ~oldAttrs.Style;
                TextStyle removedStyles = oldAttrs.Style & ~newAttrs.Style;

                if (removedStyles != TextStyle.None)
                    FormatRemovedStyles(html, removedStyles);
                if (addedStyles != TextStyle.None)
                    FormatAddedStyles(html, addedStyles);

                if (AutoFixHyperlinks)
                {
                    if (line.Text[i] == '►' &&
                        newAttrs.Link != null &&
                        (i == line.Length - 1 || line.Attributes[i + 1].Link == null))
                    {
                        newAttrs = new TextAttribute(newAttrs.Style, null);
                    }
                }

                if (newAttrs.Link != oldAttrs.Link)
                {
                    if (oldAttrs.Link != null)
                        html.Append("</a>");
                    if (newAttrs.Link != null)
                    {
                        html.AppendFormat("<a href=\"{0}\">",
                                          ConvertUri(topic, newAttrs.Link));
                    }
                }
                html.Append(Escape("" + line.Text[i]));
                oldAttrs = line.Attributes[i];
            }

            // Reset styles and links at end of line.
            if (oldAttrs.Link != null)
                html.Append("</a>");
            if (oldAttrs.Style != TextStyle.None)
                FormatRemovedStyles(html, oldAttrs.Style);

            html.AppendLine();
        }

        protected virtual string ConvertUri(HelpTopic topic, HelpUri uri)
        {
            return "?" + Escape(uri.ToString());
        }

        private static void FormatAddedStyles(
            StringBuilder html, TextStyle change)
        {
            if ((change & TextStyle.Bold) != 0)
                html.Append("<b>");
            if ((change & TextStyle.Italic) != 0)
                html.Append("<i>");
            if ((change & TextStyle.Underline) != 0)
                html.Append("<u>");
        }

        private static void FormatRemovedStyles(
            StringBuilder html, TextStyle change)
        {
            if ((change & TextStyle.Bold) != 0)
                html.Append("</b>");
            if ((change & TextStyle.Italic) != 0)
                html.Append("</i>");
            if ((change & TextStyle.Underline) != 0)
                html.Append("</u>");
        }

        public static string Escape(string s)
        {
            return System.Security.SecurityElement.Escape(s);
        }
    }
}
