using System;
using System.Collections.Generic;

namespace QuickHelp
{
    /// <summary>
    /// Represents a help database that contains a collection of help topics.
    /// </summary>
    /// <remarks>
    /// This class is the logical representation of a help database. The
    /// storage of a help database in a physical file is handled by the
    /// HelpFile and DatabaseDecoder classes.
    /// </remarks>
    public class HelpDatabase
    {
        private readonly List<HelpTopic> topics = new List<HelpTopic>();
        private readonly SortedDictionary<string, int> contextMap;
        private readonly bool isCaseSensitive;
        private readonly string name;

        /// <summary>
        /// Creates a help database with the given name. The name is used to
        /// resolve external context strings.
        /// </summary>
        /// <param name="name">Name of the database.</param>
        public HelpDatabase(string name)
            : this(name, false)
        {
        }

        /// <summary>
        /// Creates a help database with the given name, and specifies whether
        /// context strings are case sensitive.
        /// </summary>
        /// <param name="name">Name of the database</param>
        /// <param name="isCaseSensitive"></param>
        public HelpDatabase(string name, bool isCaseSensitive)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            StringComparer stringComparer = isCaseSensitive ?
                StringComparer.InvariantCulture :
                StringComparer.InvariantCultureIgnoreCase;
            this.contextMap = new SortedDictionary<string, int>(stringComparer);
            this.isCaseSensitive = isCaseSensitive;
            this.name = name;
        }

        /// <summary>
        /// Gets the name of the help database. This name is used to resolve
        /// external context strings. The name is case-insensitive.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets or sets a flag that indicates whether context strings are
        /// case-sensitive when resolving links.
        /// </summary>
        public bool IsCaseSensitive
        {
            get { return isCaseSensitive; }
        }

        /// <summary>
        /// Gets the collection of topics in this database.
        /// </summary>
        public List<HelpTopic> Topics
        {
            get { return topics; }
        }

        /// <summary>
        /// Creates a new topic within this database and appends it to the
        /// end of the topic list.
        /// </summary>
        /// <returns>The newly created topic.</returns>
        public HelpTopic NewTopic()
        {
            HelpTopic topic = new HelpTopic(this, topics.Count);
            topics.Add(topic);
            return topic;
        }

        /// <summary>
        /// Gets a list of the context strings defined in this database.
        /// </summary>
        public IEnumerable<string> ContextStrings
        {
            get { return contextMap.Keys; }
        }

        /// <summary>
        /// Associates a context string with a topic in this database.
        /// Multiple context strings may be associated with a single topic.
        /// </summary>
        /// <param name="contextString">The context string.</param>
        /// <param name="topicIndex">Zero-based topic index.</param>
        /// <remarks>Whether the context string is treated as case-sensitive
        /// is controlled by the <code>IsCaseSensitive</code> property.</remarks>
        public void AddContext(string contextString, int topicIndex)
        {
            if (contextString == null)
                throw new ArgumentNullException(nameof(contextString));
            if (topicIndex < 0)
                throw new IndexOutOfRangeException(nameof(topicIndex));

            contextMap[contextString] = topicIndex;
        }

        /// <summary>
        /// Finds the topic associated with the given context string.
        /// </summary>
        /// <param name="contextString">The context string to resolve.</param>
        /// <returns>The help topic associated with the given context string,
        /// or null if the context string cannot be resolved.</returns>
        /// <remarks>Whether the context string is treated as case-sensitive
        /// is controlled by the <code>IsCaseSensitive</code> property.</remarks>
        public HelpTopic ResolveContext(string contextString)
        {
            if (contextString == null)
                throw new ArgumentNullException("contextString");

            int topicIndex;
            if (contextMap.TryGetValue(contextString, out topicIndex))
                return topics[topicIndex];
            else
                return null;
        }
    }
}
