using System;
using System.Collections.Generic;
using System.Text;

namespace QuickHelp
{
    /// <summary>
    /// Represents a line of text in a help topic, along with formatting and
    /// hyperlink information.
    /// </summary>
    public class HelpLine
    {
        readonly string text;
        readonly TextAttribute[] attributes;

        public HelpLine(string text, TextAttribute[] attributes)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (attributes == null)
                throw new ArgumentNullException("attributes");
            if (text.Length != attributes.Length)
                throw new ArgumentException("text and attributes must have the same length.");

            this.text = text;
            this.attributes = attributes;
        }

        public HelpLine(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            this.text = text;
            this.attributes = new TextAttribute[text.Length];
        }

        public int Length
        {
            get { return text.Length; }
        }

        /// <summary>
        /// Gets the text in this line without any formatting information.
        /// This is the text that would appear on the screen.
        /// </summary>
        public string Text
        {
            get { return text; }
        }

        /// <summary>
        /// Gets the attribute of each character in the line.
        /// </summary>
        public TextAttribute[] Attributes
        {
            get { return attributes; }
        }

        public IEnumerable<HelpUri> Links
        {
            get
            {
                HelpUri lastLink = null;
                foreach (TextAttribute a in attributes)
                {
                    if (a.Link != lastLink)
                    {
                        if (a.Link != null)
                            yield return a.Link;
                        lastLink = a.Link;
                    }
                }
            }
        }

        public override string ToString()
        {
            return this.text;
        }
    }

    public struct TextAttribute
    {
        readonly TextStyle style;
        readonly HelpUri link;

        public TextAttribute(TextStyle style, HelpUri link)
        {
            this.style = style;
            this.link = link;
        }

        public TextStyle Style
        {
            get { return style; }
        }

        public HelpUri Link
        {
            get { return link; }
        }

        public static readonly TextAttribute Default = new TextAttribute();
    }

    [Flags]
    public enum TextStyle
    {
        None = 0,
        Bold = 1,
        Italics = 2,
        Underline = 4,
    }

    public class HelpLineBuilder
    {
        readonly StringBuilder textBuilder;
        readonly List<TextAttribute> attrBuilder;

        public HelpLineBuilder(int capacity)
        {
            this.textBuilder = new StringBuilder(capacity);
            this.attrBuilder = new List<TextAttribute>(capacity);
        }

        public int Length
        {
            get { return textBuilder.Length; }
        }

        public void Append(string s, int index, int count, TextStyle styles)
        {
            textBuilder.Append(s, index, count);
            for (int i = 0; i < count; i++)
                attrBuilder.Add(new TextAttribute(styles, null));
        }

        public void Append(char c, TextStyle styles)
        {
            textBuilder.Append(c);
            attrBuilder.Add(new TextAttribute(styles, null));
        }

        public void AddLink(int index, int count, HelpUri link)
        {
            for (int i = index; i < index + count; i++)
            {
                attrBuilder[i] = new TextAttribute(attrBuilder[i].Style, link);
            }
        }

        public HelpLine ToLine()
        {
            return new HelpLine(textBuilder.ToString(), attrBuilder.ToArray());
        }
    }
}
