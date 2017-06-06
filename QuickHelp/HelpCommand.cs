using System;
using System.Collections.Generic;

namespace QuickHelp
{
    // may rename to ColonCommand

    /// <summary>
    /// Represents a control command in Help text. This is only related to
    /// the storage format, and therefore is not visible outside the assembly.
    /// </summary>
    /// <remarks>
    /// A control command typically starts with ':', but can be overridden to
    /// use other characters.
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
        [CommandFormat(".category", ":c", "string")]
        Category,

        /// <summary>
        /// Indicates that the topic cannot be displayed. Use this command to
        /// hide command topics and other internal information.
        /// </summary>
        [CommandFormat(".command", ":x", null)]
        Command,

        /// <summary>
        /// Takes a string as parameter, which is a comment that appears only
        /// in the source file. Comments are not inserted in the database and
        /// are not restored during decoding.
        /// </summary>
        [CommandFormat(".comment", null, "string")]
        [CommandFormat("..", null, "string")]
        Comment,

        /// <summary>
        /// Takes a string as parameter. The string defines a context.
        /// </summary>
        [CommandFormat(".context", null, "string")]
        Context,

        /// <summary>
        /// Ends a paste section. See the .paste command. Supported only by
        /// QuickHelp.
        /// </summary>
        [CommandFormat(".end", ":e", null)]
        End,

        /// <summary>
        /// Executes the specified command. For example, 
        /// .execute Pmark context represents a jump to the specified context
        /// at the specified mark. See the .mark command.
        /// </summary>
        [CommandFormat(".execute", ":y", "command")]
        Execute,

        /// <summary>
        /// Locks the first numlines lines at the top of the screen. These
        /// lines do not move when the text is scrolled.
        /// </summary>
        [CommandFormat(".freeze", ":z", "numlines")]
        Freeze,

        /// <summary>
        /// Sets the default window size for the topic in topiclength lines.
        /// </summary>
        [CommandFormat(".length", ":l", "topiclength")]
        Length,

        /// <summary>
        /// Tells HELPMAKE to reset the line number to begin at number for
        /// subsequent lines of the input file. Line numbers appear in
        /// HELPMAKE error messages. See .source. The .line command is not
        /// inserted in the Help database and is not restored during decoding.
        /// </summary>
        [CommandFormat(".line", null, "number")]
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
        [CommandFormat(".list", ":i", null)]
        List,

        /// <summary>
        /// Defines a mark immediately preceding the following line of text. The
        /// marked line shows a script command where the display of a topic
        /// begins. The name identifies the mark. The column is an integer value
        /// specifying a column location within the marked line. Supported only
        /// by QuickHelp.
        /// </summary>
        [CommandFormat(".mark", ":m", "name [[column]]")]
        Mark,

        /// <summary>
        /// Tells the Help reader to look up the next topic using context
        /// instead of the topic that physically follows it in the file.
        /// You can use this command to skip large blocks of .command or
        /// .popup topics.
        /// </summary>
        [CommandFormat(".next", ":>", "context")]
        Next,

        /// <summary>
        /// Begins a paste section. The pastename appears in the QuickHelp
        /// Paste menu. Supported only by QuickHelp.
        /// </summary>
        [CommandFormat(".paste", ":p", "pastename")]
        Paste,

        /// <summary>
        /// Tells the Help reader to display the current topic as a popup
        /// window instead of as a normal, scrollable topic. Supported only
        /// by QuickHelp.
        /// </summary>
        [CommandFormat(".popup", ":g", null)]
        Popup,

        /// <summary>
        /// Tells the Help reader to look up the previous topic using context
        /// instead of the topic that physically precedes it in the file. You
        /// can use this command to skip large blocks of .command or .popup
        /// topics.
        /// </summary>
        [CommandFormat(".previous", ":<", "context")]
        Previous,

        /// <summary>
        /// Turns off special processing of certain characters by the Help
        /// reader.
        /// </summary>
        [CommandFormat(".raw", ":u", null)]
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
        [CommandFormat(".ref", ":r", "topic[[, topic]]")]
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
        [CommandFormat(".source", null, "filename")]
        Source,

        /// <summary>
        /// Defines text as the name or title to be displayed in place of the
        /// context string if the application Help displays a title. This
        /// command is always the first line in the context unless you also
        /// use the .length or .freeze commands.
        /// </summary>
        [CommandFormat(".topic", ":n", "text")]
        Topic,
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple=true)]
    class CommandFormatAttribute : Attribute
    {
        readonly string dotCommand;
        readonly string colonCommand;
        readonly string parameterFormat;

        public CommandFormatAttribute(
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
        /// Processes a colon command associated with a given topic. A colon
        /// command typically sets a property, such as title, of the topic.
        /// </summary>
        /// <returns>
        /// true if the command is successfully processed; false if the
        /// command is not a colon command or if the parameter syntax is
        /// invalid.
        /// </returns>
        /// TODO: take into account control character.
        public static bool ProcessColonCommand(string commandString, HelpTopic topic)
        {
            string parameter;
            HelpCommand command = ParseColonCommand(commandString, out parameter);
            if (command == HelpCommand.None)
                return false;

            return ProcessColonCommand(command, parameter, topic);
        }

        /// <summary>
        /// Processes a colon command associated with a given topic. A colon
        /// command typically sets a property, such as title, of the topic.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameter"></param>
        /// <param name="topic"></param>
        /// <returns>
        /// true if the command is successfully processed; false if the
        /// command is not a colon command or if the parameter syntax is
        /// invalid.
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
                    break;

                case HelpCommand.Execute:
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
                    break;

                case HelpCommand.Next:
                    break;

                case HelpCommand.Paste:
                    break;

                case HelpCommand.Popup:
                    topic.IsPopup = true;
                    break;

                case HelpCommand.Previous:
                    break;

                case HelpCommand.Raw:
                    topic.IsRaw = true;
                    break;

                case HelpCommand.Ref:
                    break;

                case HelpCommand.Topic:
                    topic.Title = parameter;
                    break;
               
                default:
                    return false;
            }
            return true;
        }

        public static HelpCommand ParseCommand(string line, out string parameters)
        {
            parameters = "";

            if (line == null || line.Length < 2)
                return HelpCommand.None;

            if (line[0] == '.')
                return ParseDotCommand(line, out parameters);
            else if (line[0] == ':')
                return ParseColonCommand(line, out parameters);
            else
                return HelpCommand.None;
        }

        public static HelpCommand ParseColonCommand(string line, out string parameters)
        {
            parameters = "";

            if (line == null || line.Length < 2 || line[0] != ':')
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
