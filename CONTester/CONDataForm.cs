//=======================================
//@Original Author:  PureISo
//  @Original Date:  07/10/2011
//  @Last Modified:  27/11/2026
//=======================================

using Isolib.IOPackage;
using Isolib.STFSPackage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CONtester
{
    public partial class ConDataForm : Form
    {
        private Stfs _conFile;
        private String _conFilePath = "";
        private ListViewItem _item;

        /// <summary>
        /// Disposes the images held by the picture boxes. They are owned by the
        /// form once assigned, so replacing or closing a package must free them.
        /// </summary>
        private void DisposeImages()
        {
            var packageImage = packageImagePictureBox.Image;
            packageImagePictureBox.Image = null;
            packageImage?.Dispose();

            var contentImage = contentImagePictureBox.Image;
            contentImagePictureBox.Image = null;
            contentImage?.Dispose();
        }

        /// <summary>
        /// Releases the currently open package. Called before opening another
        /// file and when the form closes, so file handles are never leaked.
        /// </summary>
        private void CloseCurrentPackage()
        {
            DisposeImages();
            _item = null;

            var file = _conFile;
            _conFile = null;

            if (file == null)
                return;

            try
            {
                file.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static bool IsConPackage(string path)
        {
            try
            {
                using (var stream = File.OpenRead(path))
                {
                    var magic = new byte[4];
                    return stream.Read(magic, 0, 4) == 4 &&
                           magic[0] == (byte)'C' && magic[1] == (byte)'O' &&
                           magic[2] == (byte)'N' && magic[3] == (byte)' ';
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Cannot open file");
                return false;
            }
        }

        private void OpenPackage(string path)
        {
            if (!IsConPackage(path))
                return;

            CloseCurrentPackage();
            _conFilePath = path;
            ConReader(path);
        }

        public ConDataForm(IList<string> args)
        {
            if (args.Count > 1)
                _conFilePath = args[1]; //Autoset target con if one has been dragged on, disregard all further args.
            InitializeComponent();
            if (_conFilePath != "")
                OpenPackage(_conFilePath);
        }

        #region Item Clicks

        private void RehashAndResignClick(object sender, EventArgs e)
        {
            if (_conFile == null)
            {
                MessageBox.Show(@"No CON package is open.", @"Rehash and Resign");
                return;
            }
            ConRehashAndReSign();
        }

        private void ExtractToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_conFile == null || _item == null)
            {
                MessageBox.Show(@"Select a file in the package first.", @"Extract");
                return;
            }

            var directoryPath = new FolderBrowserDialog();
            if (directoryPath.ShowDialog() != DialogResult.OK ||
                string.IsNullOrEmpty(directoryPath.SelectedPath))
                return;

            _conFile.Extract(_conFile.EmbeddedFiles[_item.Index], directoryPath.SelectedPath);
            MessageBox.Show(@"File Extracted!");
        }

        private void InjectToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_conFile == null)
            {
                MessageBox.Show(@"No CON package is open.", @"Inject");
                return;
            }

            //var is for implicitly used for local variables
            var open = new OpenFileDialog();
            if (open.ShowDialog() != DialogResult.OK) return;
            if (open.FileName == null) return;
            var file = new RWStream(open.FileName);
            try
            {
                if (_conFile.Replace(file, open.FileName))
                {
                    ConRehashAndReSign();
                }
                else
                {
                    MessageBox.Show("Replace Failed.");
                }
            }
            finally
            {
                file.Close(true);
            }
        }

        private void OpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            //var is for implicitly used for local variables
            var open = new OpenFileDialog();
            if (open.ShowDialog() != DialogResult.OK) return;
            if (open.FileName == null) return;
            OpenPackage(open.FileName);
        }

        #endregion Item Clicks

        #region Package Operations

        // Runs on the UI thread so no cross-thread marshalling is required and
        // the package can never be closed while an operation is in progress.
        private void ConReader(string path)
        {
            try
            {
                //Initialize the CON File reader and as Big Endian. Assign only
                // after confirming construction actually produced a package,
                // so a failed open never leaves a dead object in _conFile.
                var package = new Stfs(path);
                if (package == null)
                {
                    MessageBox.Show(@"This file could not be opened.", @"Open File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (package.HeaderData == null || package.EmbeddedFiles == null)
                {
                    package.Close();
                    throw new InvalidDataException("The package header could not be read.");
                }
                _conFile = package;
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
                DisposeImages();
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
                CloseCurrentPackage();
                MessageBox.Show(e.Message);
            }
        }

        private void ConRehashAndReSign()
        {
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
                CloseCurrentPackage();
                MessageBox.Show(@"Successful!", @"Rehash and Resign", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        #endregion Package Operations

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
            FormClosing += ConDataFormFormClosing;
        }

        private static void ConDataFormFormClosing(object sender, FormClosingEventArgs e)
        {
            ((ConDataForm)sender).CloseCurrentPackage();
        }

        private static void FormDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void FormDragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
                OpenPackage(filePaths[0]);
            }
        }

        #endregion Event Functions
    }
}