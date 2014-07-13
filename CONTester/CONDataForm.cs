//=======================================
//@Author:  PureISo
//@Date:    07/10/2011
//=======================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Isolib.IOPackage;
using Isolib.STFSPackage;

namespace CONtester
{
    public partial class ConDataForm : Form
    {
        private Stfs _conFile;
        private String _conFilePath = "";
        private bool _confileActive;
        private ListViewItem _item;

        public ConDataForm(IList<string> args)
        {
            if (args.Count > 1)
                _conFilePath = args[1]; //Autoset target con if one has been dragged on, disregard all further args.
            InitializeComponent();
            if (_conFilePath != "")
            {
                var othread = new Thread(ConReader);
                othread.Start();
            }
        }

        #region Item Clicks

        private void RehashAndResignClick(object sender, EventArgs e)
        {
            //Starting a new thread
            var othread = new Thread(ConRehashAndReSign);
            othread.Start();
        }

        private void ExtractToolStripMenuItemClick(object sender, EventArgs e)
        {
            var directoryPath = new FolderBrowserDialog();
            directoryPath.ShowDialog();
            _conFile.Extract(_conFile.EmbeddedFiles[_item.Index], directoryPath.SelectedPath);
            MessageBox.Show(@"File Extracted!");
        }

        private void InjectToolStripMenuItemClick(object sender, EventArgs e)
        {
            //var is for implicitly used for local variables
            var open = new OpenFileDialog();
            if (open.ShowDialog() != DialogResult.OK) return;
            if (open.FileName == null) return;
            var file = new RWStream(open.FileName);
            if (_conFile.Replace(file, open.FileName))
            {
                var othread = new Thread(ConRehashAndReSign);
                othread.Start();
            }
            else
            {
                MessageBox.Show("Replace Failed.");
            }
            file.Close(true);
        }

        private void OpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            //var is for implicitly used for local variables
            var open = new OpenFileDialog();
            if (open.ShowDialog() != DialogResult.OK) return;
            if (open.FileName == null) return;
            _conFilePath = open.FileName;
            //Starting a new thread
            var othread = new Thread(ConReader);
            othread.Start();
        }

        #endregion Item Clicks

        #region Thread Work

        //Private function thread to read the CON file while the UI stays responsive
        private void ConReader()
        {
            CheckForIllegalCrossThreadCalls = false;
            try
            {
                //Initialize the CON File reader and as Big Endian
                _conFile = new Stfs(_conFilePath);
                _confileActive = true;
                //Package Information
                //Max Length = 128
                displayNameTextBox.Text = _conFile.HeaderData.DisplayName;
                displayNameTextBox.Invalidate(); //Re-drawing the text box
                //Max Length = 128
                titleNameTextBox.Text = _conFile.HeaderData.TitleName;
                titleNameTextBox.Invalidate();
                //Max Length = 16
                profileIDTextBox.Text = _conFile.HeaderData.ProfileID;
                profileIDTextBox.Invalidate();
                //Max Length = 40
                deviceIDTextBox.Text = _conFile.HeaderData.DeviceID;
                deviceIDTextBox.Invalidate();
                //Max Length = 10
                consoleIDTextBox.Text = _conFile.HeaderData.ConsoleID;
                consoleIDTextBox.Invalidate();
                //Max Length = 8 (Read-Only)
                titleIDTextBox.Text = _conFile.HeaderData.TitleID.Replace("00", "");
                titleIDTextBox.Invalidate();
                packageImagePictureBox.Image = _conFile.HeaderData.PackageImage;
                packageImagePictureBox.Invalidate();
                contentImagePictureBox.Image = _conFile.HeaderData.ContentImage;
                contentImagePictureBox.Invalidate();
                //Content Information
                var contentData = new string[3];
                int index;
                contentListView.Items.Clear();
                for (index = 0; index < _conFile.EmbeddedFiles.Count; index++)
                {
                    contentData[0] = _conFile.EmbeddedFiles[index].Name;
                    contentData[1] = _conFile.EmbeddedFiles[index].DataOffset;
                    contentData[2] = _conFile.EmbeddedFiles[index].Size;
                    //Add in the new data

                    contentListView.Items.Add(new ListViewItem(contentData));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                //Aborting the thread
                Thread.CurrentThread.Abort();
            }
        }

        //Private function thread to read the CON file while the UI stays responsive
        private void ConRehashAndReSign()
        {
            CheckForIllegalCrossThreadCalls = false;
            try
            {
                //Package Information
                _conFile.HeaderData.DisplayName = displayNameTextBox.Text;
                //Max Length = 128
                _conFile.HeaderData.TitleName = titleNameTextBox.Text;
                //Max Length = 16
                _conFile.HeaderData.ProfileID = profileIDTextBox.Text;
                //Max Length = 40
                _conFile.HeaderData.DeviceID = deviceIDTextBox.Text;
                //Max Length = 10
                _conFile.HeaderData.ConsoleID = consoleIDTextBox.Text;
                //Reshash and Resign
                _conFile.Finish();
                //Close
                _conFile.Close();
                _confileActive = false;
                MessageBox.Show(@"Successful!", @"Rehash and Resign", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                //Aborting the thread
                Thread.CurrentThread.Abort();
            }
        }

        #endregion Thread Work

        #region Event Functions

        private void ContentListViewMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            _item = contentListView.GetItemAt(e.X, e.Y);
            if (_item == null) return;
            //int b = item.Index;
            Point pt = contentListView.PointToScreen(e.Location);
            contextMenuStrip.Show(pt);
        }

        private void FormLoad(object sender, EventArgs e)
        {
            AllowDrop = true;
            DragEnter += FormDragEnter;
            DragDrop += FormDragDrop;
        }

        private static void FormDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void FormDragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var filePaths = (string[]) (e.Data.GetData(DataFormats.FileDrop));
                if (_confileActive) _conFile.Close();
                _conFilePath = filePaths[0];
                var othread = new Thread(ConReader);
                othread.Start();
            }
        }

        #endregion Event Functions
    }
}