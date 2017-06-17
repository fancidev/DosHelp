namespace HelpBrowser
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.lstTopics = new System.Windows.Forms.ListBox();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabTopics = new System.Windows.Forms.TabPage();
            this.tabContexts = new System.Windows.Forms.TabPage();
            this.lstContexts = new System.Windows.Forms.ListBox();
            this.tabErrors = new System.Windows.Forms.TabPage();
            this.lstErrors = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbDatabases = new System.Windows.Forms.ComboBox();
            this.btnAddArchive = new System.Windows.Forms.Button();
            this.btnRemoveArchive = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabHtml = new System.Windows.Forms.TabPage();
            this.txtTopicTitle = new System.Windows.Forms.TextBox();
            this.tabText = new System.Windows.Forms.TabPage();
            this.txtNoFormat = new System.Windows.Forms.TextBox();
            this.tabSource = new System.Windows.Forms.TabPage();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewUnresolvedLinks = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewErrors = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabTopics.SuspendLayout();
            this.tabContexts.SuspendLayout();
            this.tabErrors.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabHtml.SuspendLayout();
            this.tabText.SuspendLayout();
            this.tabSource.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstTopics
            // 
            this.lstTopics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstTopics.FormattingEnabled = true;
            this.lstTopics.HorizontalScrollbar = true;
            this.lstTopics.ItemHeight = 15;
            this.lstTopics.Location = new System.Drawing.Point(3, 3);
            this.lstTopics.Margin = new System.Windows.Forms.Padding(4);
            this.lstTopics.Name = "lstTopics";
            this.lstTopics.Size = new System.Drawing.Size(227, 293);
            this.lstTopics.TabIndex = 0;
            this.lstTopics.SelectedIndexChanged += new System.EventHandler(this.lstTopics_SelectedIndexChanged);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 23);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(481, 299);
            this.webBrowser1.TabIndex = 1;
            this.webBrowser1.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowser1_Navigating);
            // 
            // txtSource
            // 
            this.txtSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSource.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSource.Location = new System.Drawing.Point(3, 3);
            this.txtSource.Multiline = true;
            this.txtSource.Name = "txtSource";
            this.txtSource.ReadOnly = true;
            this.txtSource.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSource.Size = new System.Drawing.Size(475, 318);
            this.txtSource.TabIndex = 2;
            this.txtSource.WordWrap = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl2);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(734, 350);
            this.splitContainer1.SplitterDistance = 241;
            this.splitContainer1.TabIndex = 4;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabTopics);
            this.tabControl2.Controls.Add(this.tabContexts);
            this.tabControl2.Controls.Add(this.tabErrors);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 23);
            this.tabControl2.Multiline = true;
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(241, 327);
            this.tabControl2.TabIndex = 1;
            // 
            // tabTopics
            // 
            this.tabTopics.Controls.Add(this.lstTopics);
            this.tabTopics.Location = new System.Drawing.Point(4, 24);
            this.tabTopics.Name = "tabTopics";
            this.tabTopics.Padding = new System.Windows.Forms.Padding(3);
            this.tabTopics.Size = new System.Drawing.Size(233, 299);
            this.tabTopics.TabIndex = 0;
            this.tabTopics.Text = "Topics";
            this.tabTopics.UseVisualStyleBackColor = true;
            // 
            // tabContexts
            // 
            this.tabContexts.Controls.Add(this.lstContexts);
            this.tabContexts.Location = new System.Drawing.Point(4, 22);
            this.tabContexts.Name = "tabContexts";
            this.tabContexts.Padding = new System.Windows.Forms.Padding(3);
            this.tabContexts.Size = new System.Drawing.Size(233, 303);
            this.tabContexts.TabIndex = 1;
            this.tabContexts.Text = "Contexts";
            this.tabContexts.UseVisualStyleBackColor = true;
            // 
            // lstContexts
            // 
            this.lstContexts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstContexts.FormattingEnabled = true;
            this.lstContexts.ItemHeight = 15;
            this.lstContexts.Location = new System.Drawing.Point(3, 3);
            this.lstContexts.Name = "lstContexts";
            this.lstContexts.Size = new System.Drawing.Size(227, 297);
            this.lstContexts.TabIndex = 0;
            this.lstContexts.SelectedIndexChanged += new System.EventHandler(this.lstContexts_SelectedIndexChanged);
            // 
            // tabErrors
            // 
            this.tabErrors.Controls.Add(this.lstErrors);
            this.tabErrors.Location = new System.Drawing.Point(4, 22);
            this.tabErrors.Name = "tabErrors";
            this.tabErrors.Size = new System.Drawing.Size(233, 303);
            this.tabErrors.TabIndex = 2;
            this.tabErrors.Text = "Errors";
            this.tabErrors.UseVisualStyleBackColor = true;
            // 
            // lstErrors
            // 
            this.lstErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstErrors.FormattingEnabled = true;
            this.lstErrors.ItemHeight = 15;
            this.lstErrors.Location = new System.Drawing.Point(0, 0);
            this.lstErrors.Name = "lstErrors";
            this.lstErrors.Size = new System.Drawing.Size(233, 303);
            this.lstErrors.TabIndex = 0;
            this.lstErrors.SelectedIndexChanged += new System.EventHandler(this.lstErrors_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.cbDatabases);
            this.panel1.Controls.Add(this.btnAddArchive);
            this.panel1.Controls.Add(this.btnRemoveArchive);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(241, 23);
            this.panel1.TabIndex = 2;
            // 
            // cbDatabases
            // 
            this.cbDatabases.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbDatabases.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDatabases.FormattingEnabled = true;
            this.cbDatabases.Location = new System.Drawing.Point(0, 0);
            this.cbDatabases.Name = "cbDatabases";
            this.cbDatabases.Size = new System.Drawing.Size(187, 23);
            this.cbDatabases.TabIndex = 1;
            this.cbDatabases.SelectedIndexChanged += new System.EventHandler(this.cbDatabases_SelectedIndexChanged);
            // 
            // btnAddArchive
            // 
            this.btnAddArchive.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnAddArchive.Location = new System.Drawing.Point(187, 0);
            this.btnAddArchive.Name = "btnAddArchive";
            this.btnAddArchive.Size = new System.Drawing.Size(27, 23);
            this.btnAddArchive.TabIndex = 3;
            this.btnAddArchive.Text = "+";
            this.btnAddArchive.UseVisualStyleBackColor = true;
            this.btnAddArchive.Click += new System.EventHandler(this.btnAddArchive_Click);
            // 
            // btnRemoveArchive
            // 
            this.btnRemoveArchive.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnRemoveArchive.Location = new System.Drawing.Point(214, 0);
            this.btnRemoveArchive.Name = "btnRemoveArchive";
            this.btnRemoveArchive.Size = new System.Drawing.Size(27, 23);
            this.btnRemoveArchive.TabIndex = 2;
            this.btnRemoveArchive.Text = "-";
            this.btnRemoveArchive.UseVisualStyleBackColor = true;
            this.btnRemoveArchive.Click += new System.EventHandler(this.btnRemoveArchive_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControl1.Controls.Add(this.tabHtml);
            this.tabControl1.Controls.Add(this.tabText);
            this.tabControl1.Controls.Add(this.tabSource);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(489, 350);
            this.tabControl1.TabIndex = 2;
            // 
            // tabHtml
            // 
            this.tabHtml.Controls.Add(this.webBrowser1);
            this.tabHtml.Controls.Add(this.txtTopicTitle);
            this.tabHtml.Location = new System.Drawing.Point(4, 4);
            this.tabHtml.Name = "tabHtml";
            this.tabHtml.Size = new System.Drawing.Size(481, 322);
            this.tabHtml.TabIndex = 0;
            this.tabHtml.Text = "HTML";
            this.tabHtml.UseVisualStyleBackColor = true;
            // 
            // txtTopicTitle
            // 
            this.txtTopicTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTopicTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtTopicTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTopicTitle.Location = new System.Drawing.Point(0, 0);
            this.txtTopicTitle.Margin = new System.Windows.Forms.Padding(0);
            this.txtTopicTitle.Name = "txtTopicTitle";
            this.txtTopicTitle.ReadOnly = true;
            this.txtTopicTitle.Size = new System.Drawing.Size(481, 23);
            this.txtTopicTitle.TabIndex = 2;
            this.txtTopicTitle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tabText
            // 
            this.tabText.Controls.Add(this.txtNoFormat);
            this.tabText.Location = new System.Drawing.Point(4, 4);
            this.tabText.Name = "tabText";
            this.tabText.Size = new System.Drawing.Size(481, 324);
            this.tabText.TabIndex = 2;
            this.tabText.Text = "Text";
            this.tabText.UseVisualStyleBackColor = true;
            // 
            // txtNoFormat
            // 
            this.txtNoFormat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNoFormat.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNoFormat.Location = new System.Drawing.Point(0, 0);
            this.txtNoFormat.Multiline = true;
            this.txtNoFormat.Name = "txtNoFormat";
            this.txtNoFormat.ReadOnly = true;
            this.txtNoFormat.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtNoFormat.Size = new System.Drawing.Size(481, 324);
            this.txtNoFormat.TabIndex = 0;
            this.txtNoFormat.WordWrap = false;
            // 
            // tabSource
            // 
            this.tabSource.Controls.Add(this.txtSource);
            this.tabSource.Location = new System.Drawing.Point(4, 4);
            this.tabSource.Name = "tabSource";
            this.tabSource.Padding = new System.Windows.Forms.Padding(3);
            this.tabSource.Size = new System.Drawing.Size(481, 324);
            this.tabSource.TabIndex = 1;
            this.tabSource.Text = "Source";
            this.tabSource.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(734, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileOpen,
            this.toolStripMenuItem1,
            this.mnuFileExit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(37, 20);
            this.mnuFile.Text = "&File";
            // 
            // mnuFileOpen
            // 
            this.mnuFileOpen.Name = "mnuFileOpen";
            this.mnuFileOpen.Size = new System.Drawing.Size(112, 22);
            this.mnuFileOpen.Text = "&Open...";
            this.mnuFileOpen.Click += new System.EventHandler(this.mnuFileOpen_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(109, 6);
            // 
            // mnuFileExit
            // 
            this.mnuFileExit.Name = "mnuFileExit";
            this.mnuFileExit.Size = new System.Drawing.Size(112, 22);
            this.mnuFileExit.Text = "E&xit";
            this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewUnresolvedLinks,
            this.mnuViewErrors});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // mnuViewUnresolvedLinks
            // 
            this.mnuViewUnresolvedLinks.Name = "mnuViewUnresolvedLinks";
            this.mnuViewUnresolvedLinks.Size = new System.Drawing.Size(172, 22);
            this.mnuViewUnresolvedLinks.Text = "&Unresolved Links...";
            this.mnuViewUnresolvedLinks.Click += new System.EventHandler(this.mnuViewUnresolvedLinks_Click);
            // 
            // mnuViewErrors
            // 
            this.mnuViewErrors.Name = "mnuViewErrors";
            this.mnuViewErrors.Size = new System.Drawing.Size(172, 22);
            this.mnuViewErrors.Text = "&Errors...";
            this.mnuViewErrors.Click += new System.EventHandler(this.mnuViewErrors_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "DOS help files|*.hlp|QuickHelp markup source|*.txt";
            this.openFileDialog1.Multiselect = true;
            this.openFileDialog1.Title = "Open DOS Help Files";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 374);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "DOS Help Viewer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabTopics.ResumeLayout(false);
            this.tabContexts.ResumeLayout(false);
            this.tabErrors.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabHtml.ResumeLayout(false);
            this.tabHtml.PerformLayout();
            this.tabText.ResumeLayout(false);
            this.tabText.PerformLayout();
            this.tabSource.ResumeLayout(false);
            this.tabSource.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstTopics;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabHtml;
        private System.Windows.Forms.TabPage tabSource;
        private System.Windows.Forms.TextBox txtNoFormat;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabTopics;
        private System.Windows.Forms.TabPage tabContexts;
        private System.Windows.Forms.ListBox lstContexts;
        private System.Windows.Forms.TabPage tabText;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuFileExit;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox txtTopicTitle;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuViewUnresolvedLinks;
        private System.Windows.Forms.ToolStripMenuItem mnuViewErrors;
        private System.Windows.Forms.TabPage tabErrors;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox cbDatabases;
        private System.Windows.Forms.Button btnAddArchive;
        private System.Windows.Forms.Button btnRemoveArchive;
        private System.Windows.Forms.ListBox lstErrors;
    }
}

