using System;
using System.Collections.Generic;

namespace QuickHelp
{
    /// <summary>
    /// Represents a help topic that contains formatted text and links.
    /// </summary>
    public class HelpTopic
    {
        private readonly List<HelpLine> lines = new List<HelpLine>();
        private readonly List<HelpSnippet> m_snippets = new List<HelpSnippet>();
        private readonly List<string> m_references = new List<string>();

        public HelpTopic()
        {
        }

        /// <summary>
        /// Gets the database that contains this help topic.
        /// </summary>
        public HelpDatabase Database { get; internal set; }

        /// <summary>
        /// Gets the zero-based index of this topic within its containing database.
        /// </summary>
        /// TODO: remove this field
        public int TopicIndex
        {
            get
            {
                if (this.Database != null)
                    return this.Database.Topics.IndexOf(this);
                else
                    return -1;
            }
        }

        /// <summary>
        /// Gets or sets the title of the topic.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the default window height in number of lines.
        /// </summary>
        public int WindowHeight { get; set; }

        /// <summary>
        /// Gets or sets the number of rows to freeze at the top of the help
        /// screen.
        /// </summary>
        public int FreezeHeight { get; set; }

#if true
        /// <summary>
        /// Gets or sets the source of this topic. If the topic is read from
        /// a text file, the object is a string; if the topic is read from a
        /// binary file, the object is a byte[].
        /// </summary>
        public object Source { get; set; }
#endif

        /// <summary>
        /// Gets the collection of lines in this topic.
        /// </summary>
        public List<HelpLine> Lines
        {
            get { return lines; }
        }

        public override string ToString()
        {
            if (this.Title == null)
                return "(Untitled Topic)";
            else
                return this.Title;
        }

        /// <summary>
        /// Gets or sets a flag that indicates whether the topic should be
        /// treated as a list of topics.
        /// </summary>
        /// <remarks>
        /// If this value is <c>true</c>, each line in the topic is treated
        /// as a list item and is supposed to point to a topic. If the line
        /// contains a link, that link points to the target. Otherwise, the
        /// first string terminated by two spaces or a newline character is
        /// treated as a context string that points to the target. If no such
        /// string exists, the first word is treated as a context string.
        /// </remarks>
        public bool IsList { get; set; }

        /// <summary>
        /// Gets or sets a flag that instructs the help viewer to turn off
        /// special processing of certain characters.
        /// </summary>
        public bool IsRaw { get; set; }

        /// <summary>
        /// Gets or sets a flag that indicates the topic should not be
        /// displayed in the help viewer because it contains commands or
        /// internal information.
        /// </summary>
        public bool IsHidden { get; set; }

        /// <summary>
        /// Gets or sets the command to execute.
        /// </summary>
        public string ExecuteCommand { get; set; }

        /// <summary>
        /// Gets or sets the category of the topic. May be <c>null</c> if the
        /// topic is not assigned to any category.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets a flag that instructs the help viewer to display the
        /// topic in a popup window instead of in a regular, scrollable
        /// window.
        /// </summary>
        public bool IsPopup { get; set; }

        /// <summary>
        /// Gets the collection of help snippets in this topic.
        /// </summary>
        public List<HelpSnippet> Snippets
        {
            get { return m_snippets; }
        }

        /// <summary>
        /// Gets or sets the previous topic in navigation.
        /// </summary>
        /// <remarks>
        /// This is used by QuickHelp to skip large number of hidden or popup
        /// topics. If not specified, the previous topic in the sequence is
        /// used.
        /// </remarks>
        public HelpUri Predecessor { get; set; }

        /// <summary>
        /// Gets or sets the next topic in navigation.
        /// </summary>
        /// <remarks>
        /// This is used by QuickHelp to skip large number of hidden or popup
        /// topics. If not specified, the next topic in the sequence is used.
        /// </remarks>
        public HelpUri Successor { get; set; }

        /// <summary>
        /// Gets the collection of references for the topic. Each reference
        /// is represented by a context string.
        /// </summary>
        public List<string> References
        {
            get { return m_references; }
        }
    }

    /// <summary>
    /// Represents a range of lines in a help topic with a name.
    /// </summary>
    /// <remarks>
    /// A snippet is called a "paste" in QuickHelp terms. It is typically
    /// used to point to a section of code that can be pasted.
    /// </remarks>
    public class HelpSnippet
    {
        /// <summary>
        /// Gets or sets the name of the snippet.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the zero-based line number of the first line of the
        /// snippet.
        /// </summary>
        public int StartLine { get; set; }

        /// <summary>
        /// Gets or sets the zero-based line number of the line just past the
        /// end of the snippet.
        /// </summary>
        public int EndLine { get; set; }
    }
}
