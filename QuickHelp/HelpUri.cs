using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace QuickHelp
{
    /// <summary>
    /// Represents a location within a help system.
    /// </summary>
    /// <remarks>
    /// A help uri may take one of the following formats, and is resolved
    /// in that order:
    ///
    /// @LXXXX -- where XXXX is a hexidecimal number with the higest bit set
    ///     Display the topic with index (XXXX & 0x7FFF) in the local
    ///     database.
    ///     
    /// @contextstring
    ///     Display the topic associated with "@contextstring". Only the
    ///     local database is searched for the context.
    /// 
    /// !command
    ///     Execute the command specified after the exclamation point (!).
    ///     The command is case sensitive. Commands are application-specific.
    ///
    /// filename!
    ///     Display filename as a single topic. The specified file must be a
    ///     text file no larger than 64K.
    ///     
    /// helpfile!contextstring
    ///     Search helpfile for contextstring and display the associated
    ///     topic. Only the specified Help database or physical Help file is
    ///     searched for the context.
    ///     
    /// contextstring
    ///     Display the topic associated with contextstring. The context
    ///     string is first searched for in the local database; if it is
    ///     not found, it is searched for in other databases in the help
    ///     system, and the first match is returned.
    /// </remarks>
    public class HelpUri
    {
        readonly string target;

        /// <summary>
        /// Constructs a uri that points to a topic in the local database.
        /// This corresponds to "local context" in QuickHelp terms.
        /// </summary>
        /// <param name="topicIndex">Zero-based topic index.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If topic index is less than 0 or greater than or equal to 0x8000.
        /// </exception>
        public HelpUri(int topicIndex)
        {
            if (topicIndex < 0 || topicIndex >= 0x8000)
                throw new ArgumentOutOfRangeException(nameof(topicIndex));

            this.target = string.Format("@L{0:X4}", topicIndex | 0x8000);
        }

        /// <summary>
        /// Constructs a uri directly.
        /// </summary>
        /// <param name="target"></param>
        public HelpUri(string target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            this.target = target;
        }

        public HelpUriType Type
        {
            get
            {
                if (target == "")
                    return HelpUriType.None;

                if (target.StartsWith("@"))
                {
                    if (TopicIndex >= 0)
                        return HelpUriType.TopicIndex;
                    else
                        return HelpUriType.LocalContext;
                }

                if (target.StartsWith("!"))
                    return HelpUriType.Command;
                else if (target.EndsWith("!"))
                    return HelpUriType.File;
                else if (target.Contains("!"))
                    return HelpUriType.GlobalContext;
                else
                    return HelpUriType.Context;
            }
        }

        /// <summary>
        /// Gets the topic index specified by this uri, or -1 if this uri
        /// does not specify a topic index.
        /// </summary>
        public int TopicIndex
        {
            get
            {
                if (target.Length == 6 && target.StartsWith("@L"))
                {
                    int topicIndexOr8000;
                    if (Int32.TryParse(
                        target.Substring(2),
                        NumberStyles.AllowHexSpecifier,
                        CultureInfo.InvariantCulture,
                        out topicIndexOr8000) &&
                        (topicIndexOr8000 & 0x8000) != 0)
                    {
                        return topicIndexOr8000 & 0x7FFF;
                    }
                }
                return -1;
            }
        }

        /// <summary>
        /// Gets the database name part of the url. This is the text on the
        /// left of the first '!'. If it contains no '!', returns null.
        /// </summary>
        public string DatabaseName
        {
            get
            {
                int k = target.IndexOf('!');
                if (k <= 0)
                    return null;
                else
                    return target.Substring(0, k);
            }
        }

        /// <summary>
        /// Gets the context string part of the url. This is the text on the
        /// right of the first '!'. If it contains no '!', returns the entire
        /// target string.
        /// </summary>
        public string ContextString
        {
            get
            {
                int k = target.IndexOf('!');
                if (k < 0)
                    return target;
                else
                    return target.Substring(k + 1);
            }
        }

        public override string ToString()
        {
            return target;
        }
    }

    /// <summary>
    /// Specifies the type of a help uri.
    /// </summary>
    public enum HelpUriType
    {
        /// <summary>
        /// The uri is empty.
        /// </summary>
        None = 0,

        /// <summary>
        /// The uri specifies a command to be executed.
        /// </summary>
        Command = 1,

        /// <summary>
        /// The uri contains a topic index that must be resolved in the
        /// local database.
        /// </summary>
        TopicIndex = 2,

        /// <summary>
        /// The uri contains a context string that must be resolved in the
        /// local database.
        /// </summary>
        LocalContext = 3,

        /// <summary>
        /// The uri contains a database name and a context string; the
        /// context string must be resolved in that database.
        /// </summary>
        GlobalContext = 4,

        /// <summary>
        /// The uri contains a context string that can be resolved in any
        /// database in the help system, with the local database searched
        /// first.
        /// </summary>
        Context = 5,
    
        /// <summary>
        /// The uri contains a database name to be displayed as a single
        /// help topic.
        /// </summary>
        File = 6,
    }

#if false
    public class HelpLocation
    {
        HelpTopic Topic;
        ushort LineNumber; // zero-based
        byte ColumnNumber; // zero-based
    }
#endif
}
