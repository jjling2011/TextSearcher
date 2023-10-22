using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextSearcher.Views
{
    public partial class FormConfig : Form
    {
        readonly Svcs.Configs configs = Svcs.Configs.Instance;

        public FormConfig()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var ctrls = panelFolderInfos.Controls;
            var f = new Folders(new Models.FolderInfo());
            ctrls.Add(f);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveControlConfigs();
            Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        void ClearPanel()
        {
            this.Invoke(
                (MethodInvoker)
                    delegate
                    {
                        var ctrls = panelFolderInfos.Controls;
                        ctrls.Clear();
                    }
            );
        }

        void RefreshPanel()
        {
            ClearPanel();

            var fis = configs.GetFolderInfos();
            this.Invoke(
                (MethodInvoker)
                    delegate
                    {
                        var ctrls = panelFolderInfos.Controls;
                        foreach (var fi in fis)
                        {
                            ctrls.Add(new Folders(fi));
                        }
                    }
            );
        }

        #region private
        void SaveControlConfigs()
        {
            var ctrls = panelFolderInfos.Controls;
            var fs = ctrls.OfType<Folders>();
            var fis = fs.Select(f => f.GetFolderInfo()).ToList();
            configs.SaveFolderInfos(fis);
        }

        private void FormConfig_Load(object sender, EventArgs e)
        {
            RefreshPanel();
        }
        #endregion
    }
}
