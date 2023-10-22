using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TextSearcher.Models;

namespace TextSearcher.Views
{
    public partial class Folders : UserControl
    {
        public readonly FolderInfo folderInfo;

        public Folders(FolderInfo folderInfo)
        {
            InitializeComponent();
            this.folderInfo = folderInfo;
            Init();
        }

        public void Init()
        {
            tboxFolder.Text = folderInfo.folder;
            tboxExts.Text = folderInfo.exts;
            chkScan.Checked = folderInfo.isScan;
            chkSearch.Checked = folderInfo.isSearch;
        }

        public Models.FolderInfo GetFolderInfo()
        {
            var fi = new Models.FolderInfo();
            this.Invoke(
                (MethodInvoker)
                    delegate
                    {
                        fi.folder = tboxFolder.Text;
                        fi.exts = tboxExts.Text;
                        fi.isScan = chkScan.Checked;
                        fi.isSearch = chkSearch.Checked;
                    }
            );
            return fi;
        }

        private void btnBrowseFolder_Click(object sender, EventArgs e)
        {
            var old = tboxFolder.Text;
            var folder = ShowBrowseFolderDialog(old);
            if (!string.IsNullOrEmpty(folder))
            {
                tboxFolder.Text = folder;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            this.Parent.Controls.Remove(this);
        }

        #region private
        public static string ShowBrowseFolderDialog(string initPath)
        {
            string folderPath = "";
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = initPath;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                folderPath = dialog.SelectedPath;
            }

            return folderPath;
        }
        #endregion
    }
}
