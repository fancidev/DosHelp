using System;
using System.Collections.Generic;
using System.Text;

namespace QuickHelp
{
    /// <summary>
    /// Represents a help topic that contains formatted text and hyperlinks.
    /// </summary>
    public class HelpTopic
    {
        private readonly HelpDatabase database;
        private readonly int topicIndex;
        private readonly List<HelpLine> lines = new List<HelpLine>();

        internal HelpTopic(HelpDatabase database, int topicIndex)
        {
            if (database == null)
                throw new ArgumentNullException(nameof(database));
            if (topicIndex < 0)
                throw new IndexOutOfRangeException(nameof(topicIndex));

            this.database = database;
            this.topicIndex = topicIndex;
        }

        /// <summary>
        /// Gets the database that contains this help topic.
        /// </summary>
        public HelpDatabase Database
        {
            get { return this.database; }
        }

        /// <summary>
        /// Gets the zero-based index of this topic within its containing database.
        /// </summary>
        public int TopicIndex
        {
            get { return this.topicIndex; }
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
        /// Gets or sets the number of rows to freeze in the display.
        /// </summary>
        public int FreezeHeight { get; set; }

        /// <summary>
        /// Gets or sets the source of this topic. If the topic is read from
        /// a text file, the object is a string; if the topic is read from a
        /// binary file, the object is a byte[].
        /// </summary>
        public object Source { get; set; }

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

        public bool IsList { get; set; }

        public bool IsRaw { get; set; }

        public bool IsHidden { get; set; }

        public string ExecuteCommand { get; set; }

        public string Category { get; set; }

        public bool IsPopup { get; set; }
    }
}
