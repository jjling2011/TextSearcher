
namespace TextSearcher.Views
{
    partial class Folders
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnBrowseFolder = new System.Windows.Forms.Button();
            this.tboxFolder = new System.Windows.Forms.TextBox();
            this.chkScan = new System.Windows.Forms.CheckBox();
            this.chkSearch = new System.Windows.Forms.CheckBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tboxExts = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnBrowseFolder
            // 
            this.btnBrowseFolder.Location = new System.Drawing.Point(383, 57);
            this.btnBrowseFolder.Name = "btnBrowseFolder";
            this.btnBrowseFolder.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseFolder.TabIndex = 0;
            this.btnBrowseFolder.Text = "Browse";
            this.btnBrowseFolder.UseVisualStyleBackColor = true;
            this.btnBrowseFolder.Click += new System.EventHandler(this.btnBrowseFolder_Click);
            // 
            // tboxFolder
            // 
            this.tboxFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tboxFolder.Location = new System.Drawing.Point(84, 3);
            this.tboxFolder.Name = "tboxFolder";
            this.tboxFolder.Size = new System.Drawing.Size(374, 21);
            this.tboxFolder.TabIndex = 1;
            // 
            // chkScan
            // 
            this.chkScan.AutoSize = true;
            this.chkScan.Location = new System.Drawing.Point(84, 60);
            this.chkScan.Name = "chkScan";
            this.chkScan.Size = new System.Drawing.Size(48, 16);
            this.chkScan.TabIndex = 2;
            this.chkScan.Text = "Scan";
            this.chkScan.UseVisualStyleBackColor = true;
            // 
            // chkSearch
            // 
            this.chkSearch.AutoSize = true;
            this.chkSearch.Location = new System.Drawing.Point(162, 60);
            this.chkSearch.Name = "chkSearch";
            this.chkSearch.Size = new System.Drawing.Size(60, 16);
            this.chkSearch.TabIndex = 3;
            this.chkSearch.Text = "Search";
            this.chkSearch.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(302, 57);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 0;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "Folder";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "Extensions";
            // 
            // tboxExts
            // 
            this.tboxExts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tboxExts.Location = new System.Drawing.Point(84, 30);
            this.tboxExts.Name = "tboxExts";
            this.tboxExts.Size = new System.Drawing.Size(374, 21);
            this.tboxExts.TabIndex = 1;
            // 
            // Folders
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkSearch);
            this.Controls.Add(this.chkScan);
            this.Controls.Add(this.tboxExts);
            this.Controls.Add(this.tboxFolder);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnBrowseFolder);
            this.Name = "Folders";
            this.Size = new System.Drawing.Size(462, 84);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBrowseFolder;
        private System.Windows.Forms.TextBox tboxFolder;
        private System.Windows.Forms.CheckBox chkScan;
        private System.Windows.Forms.CheckBox chkSearch;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tboxExts;
    }
}
