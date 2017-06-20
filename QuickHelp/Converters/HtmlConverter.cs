using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;

namespace QuickHelp.Converters
{
    /// <summary>
    /// Provides methods to render help topics as HTML.
    /// </summary>
    public class HtmlConverter
    {
        public static readonly HtmlConverter Default = new HtmlConverter
        {
            FixLinks = true,
        };

        /// <summary>
        /// Gets or sets a flag that controls whether to fix links in the
        /// output.
        /// </summary>
        /// <remarks>
        /// If this property is set to <c>true</c>, the renderer excludes
        /// the enclosing pair of ◄ and ► from the link.
        /// </remarks>
        public bool FixLinks { get; set; }

        /// <summary>
        /// Renders the given help topic as HTML and returns the HTML source.
        /// </summary>
        public string ConvertTopic(HelpTopic topic)
        {
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            StringBuilder html = new StringBuilder();
            html.AppendLine("<html>");
            html.AppendLine("  <head>");
            html.AppendLine(string.Format("    <title>{0}</title>", Escape(topic.Title)));
            html.AppendLine("    <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            html.Append(GetStyleSheet());
            html.AppendLine("  </head>");
            html.AppendLine("  <body>");

            html.Append("    <pre class=\"help-content\">");
            for (int i = 0; i < topic.Lines.Count; i++)
            {
                FormatLine(html, topic, topic.Lines[i]);
                if (i < topic.Lines.Count - 1)
                    html.AppendLine();
            }
            html.AppendLine("</pre>");

            html.AppendLine("  </body>");
            html.AppendLine("</html>");
            return html.ToString();
        }

        /// <summary>
        /// Renders a help line as HTML and returns the HTML source.
        /// </summary>
        /// <remarks>
        /// This method produces properly structured HTML. That is, it avoids
        /// markup such as 
        /// 
        ///   ...<b>...<i>...</b>...</i>...
        /// 
        /// The generated HTML is not the most compact possible, but is quite
        /// compact in practice.
        /// 
        /// For a formal discussion about unpaired tags, see
        /// http://www.w3.org/html/wg/drafts/html/master/syntax.html#an-introduction-to-error-handling-and-strange-cases-in-the-parser
        /// </remarks>
        private void FormatLine(StringBuilder html, HelpTopic topic, HelpLine line)
        {
            if (this.FixLinks)
            {
                line = FixLine(line);
            }
            for (int index = 0; index < line.Length;)
            {
                index = FormatLineSegment(html, topic, line, index);
            }
        }

        private static HelpLine FixLine(HelpLine line)
        {
            TextAttribute[] attributes = new TextAttribute[line.Length];
            for (int i = 0; i < line.Length; i++)
            {
                if (line.Text[i] == '►' &&
                    line.Attributes[i].Link != null &&
                    (i == line.Length - 1 || line.Attributes[i + 1].Link == null))
                {
                    attributes[i] = new TextAttribute(line.Attributes[i].Style, null);
                }
                else
                {
                    attributes[i] = line.Attributes[i];
                }
            }
            return new HelpLine(line.Text, attributes);
        }

        private int FormatLineSegment(StringBuilder html, HelpTopic topic, HelpLine line, int startIndex)
        {
            HelpUri link = line.Attributes[startIndex].Link;
            if (link != null)
            {
                html.AppendFormat("<a href=\"{0}\">", ConvertUri(topic, link));
            }

            Stack<TextStyle> openTags = new Stack<TextStyle>();
            int index = startIndex;
            while (index < line.Length && line.Attributes[index].Link == link)
            {
                TextAttribute oldAttrs = (index == startIndex) ?
                    TextAttribute.Default : line.Attributes[index - 1];
                TextAttribute newAttrs = line.Attributes[index];
                TextStyle stylesToAdd = newAttrs.Style & ~oldAttrs.Style;
                TextStyle stylesToRemove = oldAttrs.Style & ~newAttrs.Style;

                while (stylesToRemove != TextStyle.None)
                {
                    TextStyle top = openTags.Pop();
                    FormatRemovedStyles(html, top);
                    if ((stylesToRemove & top) != 0)
                    {
                        stylesToRemove &= ~top;
                    }
                    else
                    {
                        stylesToAdd |= top;
                    }
                }

                if ((stylesToAdd & TextStyle.Bold) != 0)
                {
                    html.Append("<b>");
                    openTags.Push(TextStyle.Bold);
                }
                if ((stylesToAdd & TextStyle.Italic) != 0)
                {
                    html.Append("<i>");
                    openTags.Push(TextStyle.Italic);
                }
                if ((stylesToAdd & TextStyle.Underline) != 0)
                {
                    html.Append("<u>");
                    openTags.Push(TextStyle.Underline);
                }

                html.Append(Escape("" + line.Text[index]));
                index++;
            }

            while (openTags.Count > 0)
            {
                FormatRemovedStyles(html, openTags.Pop());
            }
            if (link != null)
            {
                html.Append("</a>");
            }

            return index;
        }

        protected virtual string ConvertUri(HelpTopic topic, HelpUri uri)
        {
            return "?" + Escape(uri.ToString());
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

        protected virtual string GetStyleSheet()
        {
            return string.Format("    <style>\n{0}\n    </style>\n", s_styleSheet);
        }

        private static string s_styleSheet = LoadStyleSheet();

        private static string LoadStyleSheet()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "QuickHelp.Converters.Default.css";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static string Escape(string s)
        {
            return System.Security.SecurityElement.Escape(s);
        }
    }
}
