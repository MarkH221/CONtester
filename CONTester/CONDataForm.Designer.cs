namespace CONtester
{
    partial class ConDataForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConDataForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rehashResignToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.extractToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.injectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.contentInfoTabPage = new System.Windows.Forms.TabPage();
            this.contentListView = new System.Windows.Forms.ListView();
            this.contentName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contentOffset = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contentSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.packageInfoTabPage = new System.Windows.Forms.TabPage();
            this.rehashAndResign = new System.Windows.Forms.Button();
            this.contentImagePictureBox = new System.Windows.Forms.PictureBox();
            this.titleIDTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.packageImagePictureBox = new System.Windows.Forms.PictureBox();
            this.consoleIDTextBox = new System.Windows.Forms.TextBox();
            this.deviceIDTextBox = new System.Windows.Forms.TextBox();
            this.profileIDTextBox = new System.Windows.Forms.TextBox();
            this.titleNameTextBox = new System.Windows.Forms.TextBox();
            this.displayNameTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.contentInfoTabPage.SuspendLayout();
            this.packageInfoTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contentImagePictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.packageImagePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.rehashResignToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(419, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItemClick);
            // 
            // rehashResignToolStripMenuItem
            // 
            this.rehashResignToolStripMenuItem.Name = "rehashResignToolStripMenuItem";
            this.rehashResignToolStripMenuItem.Size = new System.Drawing.Size(108, 20);
            this.rehashResignToolStripMenuItem.Text = "Rehash && Resign";
            this.rehashResignToolStripMenuItem.Click += new System.EventHandler(this.RehashAndResignClick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 244);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(419, 24);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(86, 19);
            this.toolStripStatusLabel1.Text = "Version 2.2.2.0";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 18);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractToolStripMenuItem,
            this.injectToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip1";
            this.contextMenuStrip.Size = new System.Drawing.Size(116, 48);
            // 
            // extractToolStripMenuItem
            // 
            this.extractToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("extractToolStripMenuItem.Image")));
            this.extractToolStripMenuItem.Name = "extractToolStripMenuItem";
            this.extractToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.extractToolStripMenuItem.Text = "Extract";
            this.extractToolStripMenuItem.Click += new System.EventHandler(this.ExtractToolStripMenuItemClick);
            // 
            // injectToolStripMenuItem
            // 
            this.injectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("injectToolStripMenuItem.Image")));
            this.injectToolStripMenuItem.Name = "injectToolStripMenuItem";
            this.injectToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.injectToolStripMenuItem.Text = "Replace";
            this.injectToolStripMenuItem.Click += new System.EventHandler(this.InjectToolStripMenuItemClick);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.contentInfoTabPage);
            this.tabControl1.Controls.Add(this.packageInfoTabPage);
            this.tabControl1.Location = new System.Drawing.Point(0, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(419, 218);
            this.tabControl1.TabIndex = 4;
            // 
            // contentInfoTabPage
            // 
            this.contentInfoTabPage.Controls.Add(this.contentListView);
            this.contentInfoTabPage.Location = new System.Drawing.Point(4, 22);
            this.contentInfoTabPage.Name = "contentInfoTabPage";
            this.contentInfoTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.contentInfoTabPage.Size = new System.Drawing.Size(411, 192);
            this.contentInfoTabPage.TabIndex = 1;
            this.contentInfoTabPage.Text = "Content Info";
            this.contentInfoTabPage.UseVisualStyleBackColor = true;
            // 
            // contentListView
            // 
            this.contentListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.contentName,
            this.contentOffset,
            this.contentSize});
            this.contentListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentListView.GridLines = true;
            this.contentListView.Location = new System.Drawing.Point(3, 3);
            this.contentListView.MultiSelect = false;
            this.contentListView.Name = "contentListView";
            this.contentListView.Size = new System.Drawing.Size(405, 186);
            this.contentListView.TabIndex = 1;
            this.contentListView.UseCompatibleStateImageBehavior = false;
            this.contentListView.View = System.Windows.Forms.View.Details;
            this.contentListView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ContentListViewMouseDown);
            // 
            // contentName
            // 
            this.contentName.Text = "Name";
            this.contentName.Width = 162;
            // 
            // contentOffset
            // 
            this.contentOffset.Text = "Offset";
            this.contentOffset.Width = 81;
            // 
            // contentSize
            // 
            this.contentSize.Text = "Size";
            this.contentSize.Width = 95;
            // 
            // packageInfoTabPage
            // 
            this.packageInfoTabPage.Controls.Add(this.rehashAndResign);
            this.packageInfoTabPage.Controls.Add(this.contentImagePictureBox);
            this.packageInfoTabPage.Controls.Add(this.titleIDTextBox);
            this.packageInfoTabPage.Controls.Add(this.label6);
            this.packageInfoTabPage.Controls.Add(this.packageImagePictureBox);
            this.packageInfoTabPage.Controls.Add(this.consoleIDTextBox);
            this.packageInfoTabPage.Controls.Add(this.deviceIDTextBox);
            this.packageInfoTabPage.Controls.Add(this.profileIDTextBox);
            this.packageInfoTabPage.Controls.Add(this.titleNameTextBox);
            this.packageInfoTabPage.Controls.Add(this.displayNameTextBox);
            this.packageInfoTabPage.Controls.Add(this.label5);
            this.packageInfoTabPage.Controls.Add(this.label4);
            this.packageInfoTabPage.Controls.Add(this.label3);
            this.packageInfoTabPage.Controls.Add(this.label2);
            this.packageInfoTabPage.Controls.Add(this.label1);
            this.packageInfoTabPage.Location = new System.Drawing.Point(4, 22);
            this.packageInfoTabPage.Name = "packageInfoTabPage";
            this.packageInfoTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.packageInfoTabPage.Size = new System.Drawing.Size(411, 192);
            this.packageInfoTabPage.TabIndex = 0;
            this.packageInfoTabPage.Text = "Package Info";
            this.packageInfoTabPage.UseVisualStyleBackColor = true;
            // 
            // rehashAndResign
            // 
            this.rehashAndResign.Location = new System.Drawing.Point(338, 141);
            this.rehashAndResign.Name = "rehashAndResign";
            this.rehashAndResign.Size = new System.Drawing.Size(65, 39);
            this.rehashAndResign.TabIndex = 30;
            this.rehashAndResign.Text = "Rehash Resign";
            this.rehashAndResign.UseVisualStyleBackColor = true;
            this.rehashAndResign.Click += new System.EventHandler(this.RehashAndResignClick);
            // 
            // contentImagePictureBox
            // 
            this.contentImagePictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.contentImagePictureBox.Location = new System.Drawing.Point(338, 71);
            this.contentImagePictureBox.Name = "contentImagePictureBox";
            this.contentImagePictureBox.Size = new System.Drawing.Size(65, 60);
            this.contentImagePictureBox.TabIndex = 29;
            this.contentImagePictureBox.TabStop = false;
            // 
            // titleIDTextBox
            // 
            this.titleIDTextBox.Location = new System.Drawing.Point(90, 151);
            this.titleIDTextBox.MaxLength = 8;
            this.titleIDTextBox.Name = "titleIDTextBox";
            this.titleIDTextBox.ReadOnly = true;
            this.titleIDTextBox.Size = new System.Drawing.Size(242, 20);
            this.titleIDTextBox.TabIndex = 28;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(39, 158);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 13);
            this.label6.TabIndex = 27;
            this.label6.Text = "Title ID:";
            // 
            // packageImagePictureBox
            // 
            this.packageImagePictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.packageImagePictureBox.Location = new System.Drawing.Point(338, 6);
            this.packageImagePictureBox.Name = "packageImagePictureBox";
            this.packageImagePictureBox.Size = new System.Drawing.Size(65, 60);
            this.packageImagePictureBox.TabIndex = 26;
            this.packageImagePictureBox.TabStop = false;
            // 
            // consoleIDTextBox
            // 
            this.consoleIDTextBox.Location = new System.Drawing.Point(90, 122);
            this.consoleIDTextBox.MaxLength = 10;
            this.consoleIDTextBox.Name = "consoleIDTextBox";
            this.consoleIDTextBox.Size = new System.Drawing.Size(242, 20);
            this.consoleIDTextBox.TabIndex = 25;
            // 
            // deviceIDTextBox
            // 
            this.deviceIDTextBox.Location = new System.Drawing.Point(89, 93);
            this.deviceIDTextBox.MaxLength = 40;
            this.deviceIDTextBox.Name = "deviceIDTextBox";
            this.deviceIDTextBox.Size = new System.Drawing.Size(242, 20);
            this.deviceIDTextBox.TabIndex = 24;
            // 
            // profileIDTextBox
            // 
            this.profileIDTextBox.Location = new System.Drawing.Point(90, 64);
            this.profileIDTextBox.MaxLength = 16;
            this.profileIDTextBox.Name = "profileIDTextBox";
            this.profileIDTextBox.Size = new System.Drawing.Size(242, 20);
            this.profileIDTextBox.TabIndex = 23;
            // 
            // titleNameTextBox
            // 
            this.titleNameTextBox.Location = new System.Drawing.Point(90, 35);
            this.titleNameTextBox.MaxLength = 128;
            this.titleNameTextBox.Name = "titleNameTextBox";
            this.titleNameTextBox.Size = new System.Drawing.Size(242, 20);
            this.titleNameTextBox.TabIndex = 22;
            // 
            // displayNameTextBox
            // 
            this.displayNameTextBox.Location = new System.Drawing.Point(90, 6);
            this.displayNameTextBox.MaxLength = 128;
            this.displayNameTextBox.Name = "displayNameTextBox";
            this.displayNameTextBox.Size = new System.Drawing.Size(242, 20);
            this.displayNameTextBox.TabIndex = 21;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 129);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Console ID:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Device ID:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Profile ID:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Title Name:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Display Name:";
            // 
            // ConDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 268);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ConDataForm";
            this.Text = "CON Data";
            this.Load += new System.EventHandler(this.FormLoad);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStrip.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.contentInfoTabPage.ResumeLayout(false);
            this.packageInfoTabPage.ResumeLayout(false);
            this.packageInfoTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contentImagePictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.packageImagePictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem extractToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage packageInfoTabPage;
        private System.Windows.Forms.Button rehashAndResign;
        private System.Windows.Forms.PictureBox contentImagePictureBox;
        private System.Windows.Forms.TextBox titleIDTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox packageImagePictureBox;
        private System.Windows.Forms.TextBox consoleIDTextBox;
        private System.Windows.Forms.TextBox deviceIDTextBox;
        private System.Windows.Forms.TextBox profileIDTextBox;
        private System.Windows.Forms.TextBox titleNameTextBox;
        private System.Windows.Forms.TextBox displayNameTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage contentInfoTabPage;
        private System.Windows.Forms.ListView contentListView;
        private System.Windows.Forms.ColumnHeader contentName;
        private System.Windows.Forms.ColumnHeader contentOffset;
        private System.Windows.Forms.ColumnHeader contentSize;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripMenuItem injectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rehashResignToolStripMenuItem;
    }
}

