using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using QuickHelp;

namespace HelpBrowser
{
    public partial class MainForm : Form
    {
        readonly HelpViewModel viewModel = new HelpViewModel();

        public MainForm()
        {
            InitializeComponent();

            viewModel.DatabaseAdded += OnDatabasesChanged;
            viewModel.DatabaseRemoved += OnDatabasesChanged;
            viewModel.ActiveDatabaseChanged += OnActiveDatabaseChanged;
            viewModel.ActiveTopicChanged += OnActiveTopicChanged;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            viewModel.LoadSettings();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            viewModel.SaveSettings();
            Properties.Settings.Default.Save();
        }

        private void OnDatabasesChanged(object sender, EventArgs e)
        {
            cbDatabases.Items.Clear();
            foreach (HelpDatabase database in viewModel.Databases)
            {
                cbDatabases.Items.Add(new HelpDatabaseViewItem(database));
                if (database == viewModel.ActiveDatabase)
                    cbDatabases.SelectedIndex = cbDatabases.Items.Count - 1;
            }
        }

        private void OnActiveDatabaseChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < cbDatabases.Items.Count; i++)
            {
                var item = cbDatabases.Items[i] as HelpDatabaseViewItem;
                if (item.Database == viewModel.ActiveDatabase)
                {
                    cbDatabases.SelectedIndex = i;
                    break;
                }
            }

            lstTopics.Items.Clear();
            lstContexts.Items.Clear();

            if (viewModel.ActiveDatabase == null)
                return;

            lstTopics.Visible = false;
            int topicIndex = 0;
            foreach (HelpTopic topic in viewModel.ActiveDatabase.Topics)
            {
                HelpTopicViewItem item = new HelpTopicViewItem(topicIndex, topic);
                lstTopics.Items.Add(item);
                topicIndex++;
            }
            lstTopics.Visible = true;

            foreach (string contextString in viewModel.ActiveDatabase.ContextStrings)
            {
                lstContexts.Items.Add(contextString);
            }
        }

        private void OnActiveTopicChanged(object sender, EventArgs e)
        {
            HelpTopic topic = viewModel.ActiveTopic;
            if (topic == null)
                return;

            // ---- lstTopics ----
            foreach (HelpTopicViewItem item in lstTopics.Items)
            {
                if (item.Topic == topic)
                {
                    lstTopics.SelectedItem = item;
                    break;
                }
            }

            // ---- Right-hand side panel ----
            string html = QuickHelp.Converters.HtmlConverter.Default.ConvertTopic(topic);
            string text = QuickHelp.Converters.TextConverter.ConvertTopic(topic);
            txtNoFormat.Text = text;
            webBrowser1.DocumentText = html;
            txtTopicTitle.Text = HelpTopicViewItem.GetTopicDisplayTitle(topic);
            if (topic.Source is string)
                txtSource.Text = (string)topic.Source;
            else if (topic.Source is byte[])
                txtSource.Text = FormatHexData((byte[])topic.Source);
            else
                txtSource.Text = "";
        }
        
        private void lstTopics_SelectedIndexChanged(object sender, EventArgs e)
        {
            HelpTopicViewItem item = lstTopics.SelectedItem as HelpTopicViewItem;
            if (item == null)
                return;

            HelpTopic topic = item.Topic;
            viewModel.ActiveTopic = topic;
        }

        private static string FormatHexData(byte[] data)
        {
            StringBuilder sbText = new StringBuilder(8);
            StringBuilder sb = new StringBuilder();
            sb.Append("        0  1  2  3  4  5  6  7   8  9  A  B  C  D  E  F");
            
            for (int i = 0; i < data.Length; i++)
            {
                if (i % 16 == 0)
                {
                    sb.Append("    ");
                    sb.Append(sbText.ToString());
                    sbText.Remove(0, sbText.Length);

                    sb.AppendLine();
                    sb.Append(i.ToString("X4"));
                    sb.Append("  ");
                }
                else if (i % 8 == 0)
                {
                    sb.Append(" ");
                }
                sb.Append(' ');
                sb.Append(data[i].ToString("X2"));

                if (data[i] >= 32 && data[i] < 127)
                    sbText.Append((char)data[i]);
                else
                    sbText.Append('.');
            }
            if (data.Length % 16 != 0)
            {
                for (int i = 0; i < 16 - data.Length % 16; i++)
                    sb.Append("   ");
                if (data.Length % 16 <= 8)
                    sb.Append(' ');
            }
            sb.Append("    ");
            sb.Append(sbText.ToString());
            return sb.ToString();
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            string url = e.Url.OriginalString;
            if (!url.StartsWith("about:blank?"))
                return;

            string target = url.Substring(12);
            HelpUri link = new HelpUri(target);
            if (!viewModel.NavigateTo(link))
            {
                MessageBox.Show("Cannot resolve link: " + target);
                e.Cancel = true;
            }
        }

        private void lstContexts_SelectedIndexChanged(object sender, EventArgs e)
        {
            string contextString = lstContexts.SelectedItem as string;
            if (contextString == null)
                return;

            viewModel.NavigateTo(new HelpUri(contextString));
        }

        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mnuFileOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            string[] fileNames = openFileDialog1.FileNames;
            foreach (string fileName in fileNames)
            {
                viewModel.LoadDatabases(fileName);
            }
        }

        private void cbDatabases_SelectedIndexChanged(object sender, EventArgs e)
        {
            HelpDatabaseViewItem item = cbDatabases.SelectedItem as HelpDatabaseViewItem;
            if (item == null)
                viewModel.ActiveDatabase = null;
            else
                viewModel.ActiveDatabase = item.Database;
        }

        private void btnAddArchive_Click(object sender, EventArgs e)
        {
            mnuFileOpen_Click(sender, e);
        }

        private void btnRemoveArchive_Click(object sender, EventArgs e)
        {
            int index = cbDatabases.SelectedIndex;
            if (index < 0)
                return;

            HelpDatabaseViewItem item = cbDatabases.Items[index] as HelpDatabaseViewItem;
            viewModel.RemoveDatabase(item.Database);

            if (index < cbDatabases.Items.Count)
            {
                item = cbDatabases.Items[index] as HelpDatabaseViewItem;
                viewModel.ActiveDatabase = item.Database;
            }
        }

        private void mnuViewUnresolvedLinks_Click(object sender, EventArgs e)
        {
            viewModel.DumpHierarchy();
        }

        private void mnuViewErrors_Click(object sender, EventArgs e)
        {
            MessageBox.Show("There are " + viewModel.TopicsWithError.Count + " errors.");
            foreach (HelpTopic topic in viewModel.TopicsWithError)
            {
                if (MessageBox.Show(string.Format(
                    "{0}: {1}: {2}", topic.Database.Name, topic.TopicIndex, topic.Title),
                    "Error", MessageBoxButtons.OKCancel) != DialogResult.OK)
                    break;
            }
        }

        private void RecoverTopicError(HelpTopic topic)
        {

        }
    }

    class HelpViewModel
    {
        readonly HelpSystem system = new HelpSystem();

        private HelpDatabase activeDatabase;
        private HelpTopic activeTopic;

        public void LoadSettings()
        {
            var fileNames = Properties.Settings.Default.OpenFileNames;
            if (fileNames != null)
            {
                foreach (string fileName in fileNames)
                {
                    // TODO: handle exceptions
                    LoadDatabases(fileName);
                }
            }
        }

        public void SaveSettings()
        {
            var fileNames = new SortedDictionary<string, bool>(
                StringComparer.InvariantCultureIgnoreCase);

            foreach (HelpDatabase database in system.Databases)
            {
                fileNames[database.FileName] = true;
            }

            var savedNames = new System.Collections.Specialized.StringCollection();
            foreach (string fileName in fileNames.Keys)
            {
                savedNames.Add(fileName);
            }
            Properties.Settings.Default.OpenFileNames = savedNames;
        }

        public IEnumerable<HelpDatabase> Databases
        {
            get { return system.Databases; }
        }

        public void AddDatabase(HelpDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("database");

            system.Databases.Add(database);
            if (DatabaseAdded != null)
                DatabaseAdded(this, null);
            if (this.ActiveDatabase == null)
                this.ActiveDatabase = database;
        }

        public void RemoveDatabase(HelpDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("database");

            if (database == this.ActiveDatabase)
                this.ActiveDatabase = null;

            system.Databases.Remove(database);
            if (DatabaseRemoved != null)
                DatabaseRemoved(this, null);
        }

        public void LoadDatabases(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            var decoder = new QuickHelp.Serialization.BinaryHelpDeserializer();
            decoder.InvalidTopicData += decoder_TopicDecodingError;

            using (FileStream stream = File.OpenRead(fileName))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                while (stream.Position < stream.Length)
                {
                    HelpDatabase database = decoder.DeserializeDatabase(reader);
                    database.FileName = fileName;

                    if (system.FindDatabase(database.Name) == null)
                    {
                        AddDatabase(database);
                    }
                }
            }
        }

        private List<HelpTopic> topicsWithError = new List<HelpTopic>();

        public List<HelpTopic> TopicsWithError
        {
            get { return topicsWithError; }
        }

        private void decoder_TopicDecodingError(object sender, QuickHelp.Serialization.InvalidTopicDataEventArgs e)
        {
            topicsWithError.Add(e.Topic);
        }

        public event EventHandler DatabaseAdded;

        public event EventHandler DatabaseRemoved;

        public HelpDatabase ActiveDatabase
        {
            get { return activeDatabase; }
            set
            {
                if (this.activeDatabase == value)
                    return;

                this.activeDatabase = value;
                if (ActiveDatabaseChanged != null)
                    ActiveDatabaseChanged(this, null);

                if (this.activeTopic == null ||
                    this.activeTopic.Database != activeDatabase)
                    this.ActiveTopic = GetDefaultTopicOfDatabase(activeDatabase);
            }
        }

        public HelpTopic ActiveTopic
        {
            get { return activeTopic; }
            set
            {
                if (this.activeTopic == value)
                    return;

                this.activeTopic = value;
                if (activeTopic != null)
                    this.ActiveDatabase = activeTopic.Database;
                if (ActiveTopicChanged != null)
                    ActiveTopicChanged(this, null);
            }
        }

        public event EventHandler ActiveDatabaseChanged;

        public event EventHandler ActiveTopicChanged;

        public bool NavigateTo(HelpUri uri)
        {
            HelpTopic topic = system.ResolveUri(activeDatabase, uri);
            if (topic != null)
            {
                // TODO: also need to change active database
                ActiveTopic = topic;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static HelpTopic GetDefaultTopicOfDatabase(HelpDatabase database)
        {
            if (database == null)
                return null;

            HelpTopic topic = database.ResolveContext("h.contents");
            if (topic != null)
                return topic;

            if (database.Topics.Count > 0)
                return database.Topics[0];

            return null;
        }

        public void DumpHierarchy()
        {
            HelpTopic root = system.ResolveUri(null, new HelpUri("h.contents"));
            if (root == null)
                return;

            //TopicHierarchy tree = new TopicHierarchy();
            //tree.Build(system, root);
            //tree.Dump();
        }
    }

    class HelpDatabaseViewItem
    {
        readonly HelpDatabase database;

        public HelpDatabaseViewItem(HelpDatabase database)
        {
            this.database = database;
        }

        public HelpDatabase Database
        {
            get { return this.database; }
        }

        public override string ToString()
        {
            HelpTopic titleTopic = database.ResolveContext("h.title");
            if (titleTopic != null &&
                titleTopic.Lines.Count > 0 &&
                titleTopic.Lines[0].Text != null)
                return titleTopic.Lines[0].Text;

            if (database.Name == null)
                return "(unnamed database)";
            else
                return database.Name;
        }
    }

    class HelpTopicViewItem
    {
        readonly int topicIndex;
        readonly HelpTopic topic;

        public HelpTopicViewItem(int topicIndex, HelpTopic topic)
        {
            this.topicIndex = topicIndex;
            this.topic = topic;
        }

        public HelpTopic Topic
        {
            get { return topic; }
        }

        public override string ToString()
        {
            return GetTopicDisplayTitle(topic);
        }

        public static string GetTopicDisplayTitle(HelpTopic topic)
        {
            if (topic == null)
                return null;

            if (topic.Title != null)
                return topic.Title;

            // try find a context string that points to this topic
            // TODO: make this part of topic's member
            List<string> contextStrings = new List<string>();
            foreach (string contextString in topic.Database.ContextStrings)
            {
                if (topic.Database.ResolveContext(contextString) == topic)
                    contextStrings.Add(contextString);
            }
            if (contextStrings.Count > 0)
            {
                contextStrings.Sort();
                StringBuilder sb = new StringBuilder();
                sb.Append('[');
                foreach (string contextString in contextStrings)
                {
                    if (sb.Length > 1)
                        sb.Append(", ");
                    sb.Append(contextString);
                }
                sb.Append(']');
                return sb.ToString();
            }

            //return string.Format("[{0:000}] {1}", topicIndex, topic);
            return "(Untitled Topic)";
        }

    }
}
