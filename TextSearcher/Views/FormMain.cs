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

            this.Text = "Text searcher v1.0";
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

            var kws = tboxSearch.Text;
            Task.Run(() =>
            {
                var records = dbm.Search(kws).OrderByDescending(fi => fi.modify);
                this.Invoke(
                    (MethodInvoker)
                        delegate
                        {
                            ShowSearchResult(records);
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
            try
            {
                switch (columnindex)
                {
                    case 5:
                        MessageBox.Show(hit.SubItem.Text);
                        break;
                    case 3:
                        var folder = Path.GetDirectoryName(subItems[3].Text);
                        Process.Start(folder);
                        break;
                    case 1:
                        Process.Start(subItems[3].Text);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void bak_lvContent_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            for (int itemIndex = 0; itemIndex < lvContent.Items.Count; itemIndex++)
            {
                ListViewItem item = lvContent.Items[itemIndex];
                var itemRect = item.GetBounds(ItemBoundsPortion.Entire);
                if (itemRect.Contains(e.Location))
                {
                    var subItems = item.SubItems;
                    for (int i = 0; i < subItems.Count; i++)
                    {
                        itemRect = subItems[i].Bounds;
                        if (itemRect.Contains(e.Location))
                        {
                            if (i == subItems.Count - 1)
                            {
                                MessageBox.Show(subItems[i].Text);
                                return;
                            }
                            break;
                        }
                    }

                    var file = item.SubItems[3].Text;
                    var folder = Path.GetDirectoryName(file);
                    if (!string.IsNullOrEmpty(folder))
                    {
                        Process.Start(folder);
                    }
                    return;
                }
            }
        }

        private void compactDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Run(() => dbm.Compact());
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            if (configs.IsFormMaximum())
            {
                this.WindowState = FormWindowState.Maximized;
            }

            var widths = configs.GetListViewColWidths();
            for (int i = 0; i < widths.Count; i++)
            {
                lvContent.Columns[i].Width = widths[i];
            }

            if (configs.GetLastScan().AddHours(12) < DateTime.Now)
            {
                Task.Run(() => dbm.Scan());
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            var cols = lvContent.Columns;
            var widths = new List<int>();
            for (int i = 0; i < cols.Count; i++)
            {
                widths.Add(cols[i].Width);
            }

            configs.SetFormMaximumSize(WindowState == FormWindowState.Maximized);
            configs.SaveListViewColWidths(widths);
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

        void ShowSearchResult(IEnumerable<Models.TextFileInfo> records)
        {
            var index = 1;
            lvContent.SuspendLayout();
            lvContent.Items.Clear();
            foreach (var record in records)
            {
                var row = record.ToArray();
                row[0] = index.ToString();
                var item = new ListViewItem(row);
                if ((index % 2) == 1)
                {
                    item.BackColor = Color.LightGray;
                }
                lvContent.Items.Add(item);
                index++;
            }
            lvContent.ResumeLayout();
        }
        #endregion
    }
}
