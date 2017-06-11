using System;
using System.Collections.Generic;
using System.Text;

namespace QuickHelp
{
    /// <summary>
    /// Manages multiple cross-referenced help databases as a library.
    /// </summary>
    /// TODO: probably better to rename as HelpLibrary or HelpCollection?
    public class HelpSystem
    {
        readonly List<HelpDatabase> databases = new List<HelpDatabase>();

        /// <summary>
        /// Gets a list of help databases contained in this library.
        /// </summary>
        public List<HelpDatabase> Databases
        {
            get { return databases; }
        }

        /// <summary>
        /// Finds a database with the given name, ignoring case.
        /// </summary>
        /// <param name="name">Name of the database to find.</param>
        /// <returns>The help database, or null if not found.</returns>
        /// TODO: should use a hash table to speed up.
        public HelpDatabase FindDatabase(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            foreach (HelpDatabase database in databases)
            {
                if (database.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    return database;
            }
            return null;
        }

        public HelpTopic ResolveUri(HelpDatabase referrer, HelpUri uri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            HelpUriType uriType = uri.Type;
            if (uriType == HelpUriType.LocalTopic)
            {
                if (referrer != null)
                {
                    int topicIndex = uri.TopicIndex;
                    if (topicIndex >= 0 && topicIndex < referrer.Topics.Count)
                        return referrer.Topics[topicIndex];
                }
            }
            else if (uriType == HelpUriType.GlobalContext)
            {
                string dbName = uri.DatabaseName;
                HelpDatabase db = FindDatabase(dbName);
                if (db != null)
                    return db.ResolveContext(uri.ContextString);
            }
            else if (uriType == HelpUriType.LocalContext)
            {
                if (referrer != null)
                    return referrer.ResolveContext(uri.ContextString);
            }
            else if (uriType == HelpUriType.Context)
            {
                if (referrer != null)
                {
                    HelpTopic topic = referrer.ResolveContext(uri.ContextString);
                    if (topic != null)
                        return topic;
                }
                foreach (HelpDatabase db in databases)
                {
                    HelpTopic topic = db.ResolveContext(uri.ContextString);
                    if (topic != null)
                        return topic;
                }
                return null;
            }
            return null;
        }
    }
}
