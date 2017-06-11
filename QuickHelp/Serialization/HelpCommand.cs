using System;
using System.Collections.Generic;

namespace QuickHelp
{
    /// <summary>
    /// Specifies a dot or colon command in serialized help content.
    /// </summary>
    /// <remarks>
    /// Because dot or colon commands are only relevant in serialized format,
    /// this enum and related classes are internal to the assembly.
    /// </remarks>
    enum HelpCommand
    {
        None = 0,

        /// <summary>
        /// Lists the category in which the current topic appears and its
        /// position in the list of topics. The category name is used by the
        /// QuickHelp Categories command, which displays the list of topics.
        /// Supported only by QuickHelp.
        /// </summary>
        [HelpCommandFormat(".category", ":c", "string")]
        Category,

        /// <summary>
        /// Indicates that the topic cannot be displayed. Use this command to
        /// hide command topics and other internal information.
        /// </summary>
        [HelpCommandFormat(".command", ":x", null)]
        Command,

        /// <summary>
        /// Takes a string as parameter, which is a comment that appears only
        /// in the source file. Comments are not inserted in the database and
        /// are not restored during decoding.
        /// </summary>
        [HelpCommandFormat(".comment", null, "string")]
        [HelpCommandFormat("..", null, "string")]
        Comment,

        /// <summary>
        /// Takes a string as parameter. The string defines a context.
        /// </summary>
        [HelpCommandFormat(".context", null, "string")]
        Context,

        /// <summary>
        /// Ends a paste section. See the .paste command. Supported only by
        /// QuickHelp.
        /// </summary>
        [HelpCommandFormat(".end", ":e", null)]
        End,

        /// <summary>
        /// Executes the specified command. For example, 
        /// .execute Pmark context represents a jump to the specified context
        /// at the specified mark. See the .mark command.
        /// </summary>
        [HelpCommandFormat(".execute", ":y", "command")]
        Execute,

        /// <summary>
        /// Locks the first numlines lines at the top of the screen. These
        /// lines do not move when the text is scrolled.
        /// </summary>
        [HelpCommandFormat(".freeze", ":z", "numlines")]
        Freeze,

        /// <summary>
        /// Sets the default window size for the topic in topiclength lines.
        /// </summary>
        [HelpCommandFormat(".length", ":l", "topiclength")]
        Length,

        /// <summary>
        /// Tells HELPMAKE to reset the line number to begin at number for
        /// subsequent lines of the input file. Line numbers appear in
        /// HELPMAKE error messages. See .source. The .line command is not
        /// inserted in the Help database and is not restored during decoding.
        /// </summary>
        [HelpCommandFormat(".line", null, "number")]
        Line,

        /// <summary>
        /// Indicates that the current topic contains a list of topics. Help
        /// displays a highlighted line; you can choose a topic by moving the
        /// highlighted line over the desired topic and pressing ENTER. If the
        /// line contains a coded link, Help looks up that link. If it does
        /// not contain a link, Help looks within the line for a string
        /// terminated by two spaces or a newline character and looks up that
        /// string. Otherwise, Help looks up the first word.
        /// </summary>
        [HelpCommandFormat(".list", ":i", null)]
        List,

        /// <summary>
        /// Defines a mark immediately preceding the following line of text. The
        /// marked line shows a script command where the display of a topic
        /// begins. The name identifies the mark. The column is an integer value
        /// specifying a column location within the marked line. Supported only
        /// by QuickHelp.
        /// </summary>
        [HelpCommandFormat(".mark", ":m", "name [[column]]")]
        Mark,

        /// <summary>
        /// Tells the Help reader to look up the next topic using context
        /// instead of the topic that physically follows it in the file.
        /// You can use this command to skip large blocks of .command or
        /// .popup topics.
        /// </summary>
        [HelpCommandFormat(".next", ":>", "context")]
        Next,

        /// <summary>
        /// Begins a paste section. The pastename appears in the QuickHelp
        /// Paste menu. Supported only by QuickHelp.
        /// </summary>
        [HelpCommandFormat(".paste", ":p", "pastename")]
        Paste,

        /// <summary>
        /// Tells the Help reader to display the current topic as a popup
        /// window instead of as a normal, scrollable topic. Supported only
        /// by QuickHelp.
        /// </summary>
        [HelpCommandFormat(".popup", ":g", null)]
        Popup,

        /// <summary>
        /// Tells the Help reader to look up the previous topic using context
        /// instead of the topic that physically precedes it in the file. You
        /// can use this command to skip large blocks of .command or .popup
        /// topics.
        /// </summary>
        [HelpCommandFormat(".previous", ":<", "context")]
        Previous,

        /// <summary>
        /// Turns off special processing of certain characters by the Help
        /// reader.
        /// </summary>
        [HelpCommandFormat(".raw", ":u", null)]
        Raw,

        /// <summary>
        /// Tells the Help reader to display the topic in the Reference menu.
        /// You can list multiple topics; separate each additional topic with
        /// a comma. A .ref command is not affected by the /W option. If no
        /// topic is specified, QuickHelp searches the line immediately
        /// following for a See or See Also reference; if present, the
        /// reference must be the first word on the line. Supported only by
        /// QuickHelp.
        /// </summary>
        [HelpCommandFormat(".ref", ":r", "topic[[, topic]]")]
        Ref,

        /// <summary>
        /// Tells HELPMAKE that subsequent topics come from filename. HELPMAKE
        /// error messages contain the name and line number of the input file.
        /// The .source command tells HELPMAKE to use filename in the message
        /// instead of the name of the input file and to reset the line number
        /// to 1. This is useful when you concatenate several sources to form
        /// the input file. See .line. The .source command is not inserted in
        /// the Help database and is not restored during decoding.
        /// </summary>
        [HelpCommandFormat(".source", null, "filename")]
        Source,

        /// <summary>
        /// Defines text as the name or title to be displayed in place of the
        /// context string if the application Help displays a title. This
        /// command is always the first line in the context unless you also
        /// use the .length or .freeze commands.
        /// </summary>
        [HelpCommandFormat(".topic", ":n", "text")]
        Topic,
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple=true)]
    class HelpCommandFormatAttribute : Attribute
    {
        readonly string dotCommand;
        readonly string colonCommand;
        readonly string parameterFormat;

        public HelpCommandFormatAttribute(
            string dotCommand, string colonCommand, string parameterFormat)
        {
            if (dotCommand == null)
                throw new ArgumentNullException(nameof(dotCommand));
            if (dotCommand.Length == 0 || dotCommand[0] != '.')
                throw new ArgumentException("Dot command must start with a dot.", nameof(dotCommand));
            
            if (colonCommand == null)
                throw new ArgumentNullException(nameof(colonCommand));
            if (colonCommand.Length == 0 || colonCommand[0] != ':')
                throw new ArgumentException("Colon command must start with a colon.", nameof(colonCommand));

            if (parameterFormat == null)
                throw new ArgumentNullException(nameof(parameterFormat));

            this.dotCommand = dotCommand;
            this.colonCommand = colonCommand;
            this.parameterFormat = parameterFormat;
        }
    }

    static class HelpCommandConverter
    {
        /// <summary>
        /// Processes a colon command associated with a given topic.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the command is successfully processed; <c>false</c>
        /// if the command is not supported or if the syntax is invalid.
        /// </returns>
        public static bool ProcessColonCommand(
            string commandString, char controlCharacter, HelpTopic topic)
        {
            string parameter;
            HelpCommand command = ParseColonCommand(
                commandString, controlCharacter, out parameter);
            if (command == HelpCommand.None)
                return false;

            return ProcessColonCommand(command, parameter, topic);
        }

        /// <summary>
        /// Processes a colon command associated with a given topic.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the command is successfully processed; <c>false</c>
        /// if the command is not supported or if the syntax is invalid.
        /// </returns>
        public static bool ProcessColonCommand(
            HelpCommand command, string parameter, HelpTopic topic)
        {
            switch (command)
            {
                case HelpCommand.Category:
                    topic.Category = parameter;
                    break;

                case HelpCommand.Command:
                    topic.IsHidden = true;
                    break;

                case HelpCommand.End:
                    if (topic.Snippets.Count > 0)
                    {
                        topic.Snippets[topic.Snippets.Count - 1].EndLine
                            = topic.Lines.Count;
                    }
                    break;

                case HelpCommand.Execute:
                    // TODO: there could be multiple execute commands.
                    topic.ExecuteCommand = parameter;
                    break;

                case HelpCommand.Freeze:
                    {
                        int n;
                        if (Int32.TryParse(parameter, out n))
                            topic.FreezeHeight = n;
                        else
                            return false;
                    }
                    break;

                case HelpCommand.Length:
                    {
                        int n;
                        if (Int32.TryParse(parameter, out n))
                            topic.WindowHeight = n;
                        else
                            return false;
                    }
                    break;

                case HelpCommand.List:
                    topic.IsList = true;
                    break;

                case HelpCommand.Mark:
                    // TODO: to be implemented
                    System.Diagnostics.Debug.WriteLine(string.Format(
                        "**** NOT IMPLEMENTED **** HelpCommand.Mark @ Line {0} of Topic {1} ({2}): {3}",
                        topic.Lines.Count, topic.TopicIndex, topic.Title, parameter));
                    break;

                case HelpCommand.Next:
                    topic.Successor = new HelpUri(parameter);
                    break;

                case HelpCommand.Paste:
                    {
                        HelpSnippet snippet = new HelpSnippet();
                        snippet.Name = parameter;
                        snippet.StartLine = topic.Lines.Count;
                        snippet.EndLine = topic.Lines.Count;
                        topic.Snippets.Add(snippet);
                    }
                    break;

                case HelpCommand.Popup:
                    topic.IsPopup = true;
                    break;

                case HelpCommand.Previous:
                    topic.Predecessor = new HelpUri(parameter);
                    break;

                case HelpCommand.Raw:
                    topic.IsRaw = true;
                    break;

                case HelpCommand.Ref:
                    if (string.IsNullOrEmpty(parameter))
                    {
                        // TODO: The references are in the following
                        // lines until the next blank line. We don't
                        // handle this for the moment.
                    }
                    else
                    {
                        string[] references = parameter.Split(',');
                        foreach (string reference in references)
                        {
                            string contextString = reference.Trim();
                            if (!string.IsNullOrEmpty(contextString))
                                topic.References.Add(contextString);
                        }
                    }
                    break;

                case HelpCommand.Topic:
                    topic.Title = parameter;
                    break;

                default:
                    return false;
            }
            return true;
        }

        public static HelpCommand ParseCommand(
            string line, char controlCharacter, out string parameters)
        {
            parameters = "";

            if (line == null || line.Length < 2)
                return HelpCommand.None;

            if (line[0] == '.')
                return ParseDotCommand(line, out parameters);
            else if (line[0] == controlCharacter)
                return ParseColonCommand(line, controlCharacter, out parameters);
            else
                return HelpCommand.None;
        }

        public static HelpCommand ParseColonCommand(
            string line, char controlCharacter, out string parameters)
        {
            parameters = "";

            if (line == null || line.Length < 2 || line[0] != controlCharacter)
                return HelpCommand.None;

            HelpCommand command;
            if (ColonCommandToHelpCommandMapping.TryGetValue(line[1], out command))
            {
                parameters = line.Substring(2);
                return command;
            }
            else
            {
                return HelpCommand.None;
            }
        }

        public static HelpCommand ParseDotCommand(string line, out string parameters)
        {
            parameters = "";

            if (line == null || line.Length < 2 || line[0] != '.')
                return HelpCommand.None;

            string dotCommand;
            int k = line.IndexOf(' ');
            if (k < 0)
            {
                dotCommand = line.Substring(1);
                parameters = "";
            }
            else
            {
                dotCommand = line.Substring(1, k - 1);
                parameters = line.Substring(k + 1);
            }

            char colonCommand;
            if (DotCommandToColonCommandMapping.TryGetValue(dotCommand, out colonCommand))
            {
                return ColonCommandToHelpCommandMapping[colonCommand];
            }

            // Process source-only dot commands that do not have an equivalent
            // colon command.
            switch (dotCommand)
            {
                case "comment":
                case ".":
                    return HelpCommand.Comment;
                case "context":
                    return HelpCommand.Context;
                case "source":
                    return HelpCommand.Source;
                default:
                    return HelpCommand.None;
            }
        }

        private static readonly Dictionary<string, char>
            DotCommandToColonCommandMapping = new Dictionary<string, char>
            {
                { "category", 'c' },
                { "command",  'x' },
                { "end",      'e' },
                { "execute",  'y' },
                { "freeze",   'z' },
                { "length",   'l' },
                { "list",     'i' },
                { "mark",     'm' },
                { "next",     '>' },
                { "paste",    'p' },
                { "popup",    'g' },
                { "previous", '<' },
                { "raw",      'u' },
                { "ref",      'r' },
                { "topic",    'n' },
            };

        private static readonly Dictionary<char, HelpCommand>
            ColonCommandToHelpCommandMapping = new Dictionary<char, HelpCommand>
            {
                { '<', HelpCommand.Previous },
                { '>', HelpCommand.Next },
                { 'c', HelpCommand.Category },
                { 'e', HelpCommand.End },
                { 'g', HelpCommand.Popup },
                { 'i', HelpCommand.List },
                { 'l', HelpCommand.Length },
                { 'm', HelpCommand.Mark },
                { 'n', HelpCommand.Topic },
                { 'p', HelpCommand.Paste },
                { 'r', HelpCommand.Ref },
                { 'u', HelpCommand.Raw },
                { 'x', HelpCommand.Command },
                { 'y', HelpCommand.Execute },
                { 'z', HelpCommand.Freeze },
            };
    }
}
