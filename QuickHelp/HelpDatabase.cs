using System;
using System.Collections;
using System.Collections.Generic;

namespace QuickHelp
{
    /// <summary>
    /// Represents a help database that contains a collection of help topics.
    /// </summary>
    /// <remarks>
    /// This class is the logical representation of a help database. The
    /// <c>BinaryHelpSerializer</c> class handles the serialization of the
    /// help database on disk.
    /// </remarks>
    public class HelpDatabase
    {
        private readonly HelpTopicCollection topics;
        private readonly SortedDictionary<string, int> contextMap;
        private readonly bool isCaseSensitive;
        private readonly string name;

        /// <summary>
        /// Creates a help database with the given name. The name is used to
        /// resolve external context strings.
        /// </summary>
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
            this.topics = new HelpTopicCollection(this);
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
        public HelpTopicCollection Topics
        {
            get { return topics; }
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

    public class HelpTopicCollection : IList<HelpTopic>
    {
        private readonly HelpDatabase m_database;
        private readonly List<HelpTopic> m_topics = new List<HelpTopic>();

        internal HelpTopicCollection(HelpDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException(nameof(database));
            m_database = database;
        }

        private void Attach(HelpTopic topic)
        {
            if (topic != null)
            {
                if (topic.Database != null)
                {
                    throw new InvalidOperationException("Topic is already part of a database.");
                }
                topic.Database = m_database;
            }
        }

        private void Detach(HelpTopic topic)
        {
            if (topic != null)
            {
                if (topic.Database != m_database)
                {
                    throw new InvalidOperationException("Topic to detach is not part of this database.");
                }
                topic.Database = null;
            }
        }

        public HelpTopic this[int index]
        {
            get { return m_topics[index]; }
            set
            {
                if (m_topics[index] != value)
                {
                    Attach(value);
                    Detach(m_topics[index]);
                    m_topics[index] = value;
                }
            }
        }

        public int Count
        {
            get { return m_topics.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(HelpTopic topic)
        {
            Attach(topic);
            m_topics.Add(topic);
        }

        public void Clear()
        {
            foreach (HelpTopic topic in m_topics)
            {
                Detach(topic);
            }
            m_topics.Clear();
        }

        public bool Contains(HelpTopic topic)
        {
            return m_topics.Contains(topic);
        }

        public void CopyTo(HelpTopic[] array, int arrayIndex)
        {
            m_topics.CopyTo(array, arrayIndex);
        }

        public IEnumerator<HelpTopic> GetEnumerator()
        {
            return m_topics.GetEnumerator();
        }

        public int IndexOf(HelpTopic topic)
        {
            return m_topics.IndexOf(topic);
        }

        public void Insert(int index, HelpTopic topic)
        {
            Attach(topic);
            m_topics.Insert(index, topic);
        }

        public bool Remove(HelpTopic topic)
        {
            if (m_topics.Remove(topic))
            {
                Detach(topic);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            HelpTopic topic = m_topics[index];
            m_topics.RemoveAt(index);
            Detach(topic);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)m_topics).GetEnumerator();
        }
    }
}