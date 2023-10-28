using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextSearcher.Views
{
    public partial class FormMain : Form
    {
        Svcs.DbMgr dbm = Svcs.DbMgr.Instance;
        Svcs.Configs configs = Svcs.Configs.Instance;

        public FormMain()
        {
            InitializeComponent();
            dbm.onStatus += UpdateState;

            this.Text = "Text searcher v1.0.4";
        }

        #region handler
        FormConfig frmConfig;

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmConfig == null)
            {
                frmConfig = new FormConfig();
                frmConfig.FormClosed += (_, __) =>
                {
                    frmConfig = null;
                };
                frmConfig.Show();
            }
            frmConfig.Activate();
        }

        private void clearDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Utils.UI.Confirm(@"All data will be deleted!!"))
            {
                return;
            }
            Task.Run(() => dbm.Clear());
        }

        private void updateDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Run(() => dbm.Scan());
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tboxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }
            e.SuppressKeyPress = true;

            var kws = tboxSearch.Text
                ?.ToLower()
                ?.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                ?.ToList();

            Task.Run(() =>
            {
                var records = dbm.Search(kws).OrderByDescending(fi => fi.modify);
                this.Invoke(
                    (MethodInvoker)
                        delegate
                        {
                            ShowSearchResult(records, kws);
                        }
                );
            });
        }

        private void lvContent_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Point mousePosition = lvContent.PointToClient(Control.MousePosition);
            ListViewHitTestInfo hit = lvContent.HitTest(mousePosition);
            if (hit.Item == null)
            {
                return;
            }

            var subItems = hit.Item.SubItems;
            int columnindex = subItems.IndexOf(hit.SubItem);
            var path = subItems[2].Text;
            try
            {
                switch (columnindex)
                {
                    case 4:
                        var content = dbm.GetContent(path);
                        MessageBox.Show(content);
                        break;
                    case 2:
                        var folder = Path.GetDirectoryName(path);
                        Process.Start(folder);
                        break;
                    case 1:
                        Process.Start(path);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void compactDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Run(() => dbm.Compact());
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            SetFormBounds();

            var widths = configs.GetListViewColWidths();
            var len = Math.Min(widths.Count, lvContent.Columns.Count);
            for (int i = 0; i < len; i++)
            {
                lvContent.Columns[i].Width = widths[i];
            }

            if (configs.IsLastScanOutdated())
            {
                Task.Run(() => dbm.Scan());
            }

            tboxSearch.Focus();
        }

        private void SetFormBounds()
        {
            if (configs.IsFormMaximum())
            {
                this.WindowState = FormWindowState.Maximized;
                return;
            }

            var rect = configs.GetFormMainBounds();
            if (rect.Width < 100 || rect.Height < 100)
            {
                return;
            }

            var bounds = Screen.FromControl(this).Bounds;
            this.Left = Math.Max(0, rect.Left);
            this.Top = Math.Max(0, rect.Top);
            this.Width = Math.Min(rect.Width, bounds.Width - rect.Left);
            this.Height = Math.Min(rect.Height, bounds.Height - rect.Top);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !Utils.UI.Confirm(@"Exit app?"))
            {
                e.Cancel = true;
                return;
            }

            var cols = lvContent.Columns;
            var widths = new List<int>();
            for (int i = 0; i < cols.Count; i++)
            {
                widths.Add(cols[i].Width);
            }

            var bounds = new int[] { this.Left, this.Top, this.Width, this.Height };
            configs.SaveFormMainBunds(string.Join(",", bounds));
            configs.SaveFormMaximumSize(WindowState == FormWindowState.Maximized);
            configs.SaveListViewColWidths(widths);
        }

        private void gitHubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var url = @"https://www.github.com/jjling2011/TextSearcher";
            if (!Utils.UI.Confirm($"Open website:\n{url}"))
            {
                return;
            }
            Process.Start(url);
        }
        #endregion

        #region override

        // https://stackoverflow.com/questions/400113/best-way-to-implement-keyboard-shortcuts-in-a-windows-forms-application
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F))
            {
                tboxSearch.Focus();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        #region private
        void UpdateState(string text)
        {
            this.Invoke(
                (MethodInvoker)
                    delegate
                    {
                        tsStatus.Text = text;
                    }
            );
        }

        void ShowSearchResult(
            IEnumerable<Models.TextFileInfo> records,
            IEnumerable<string> keywords
        )
        {
            var index = 1;
            lvContent.SuspendLayout();
            lvContent.Items.Clear();
            foreach (var record in records)
            {
                var row = record.ToArray(keywords);
                row[0] = index.ToString();
                var item = new ListViewItem(row);
                if ((index % 2) == 1)
                {
                    item.BackColor = Color.Gainsboro;
                }
                lvContent.Items.Add(item);
                index++;
            }
            lvContent.ResumeLayout();
        }
        #endregion
    }
}
