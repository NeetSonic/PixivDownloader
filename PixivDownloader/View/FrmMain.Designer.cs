namespace PixivDownloader.View
{
    partial class FrmMain
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
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.chkDateAfter = new System.Windows.Forms.CheckBox();
            this.dateAfter = new System.Windows.Forms.DateTimePicker();
            this.txtProxy = new Neetsonic.Control.TextBox();
            this.txtLog = new Neetsonic.Control.LogTextBox();
            this.txtSaveDir = new Neetsonic.Control.TextBox();
            this.txtIllustratorID = new Neetsonic.Control.TextBox();
            this.chkManga = new System.Windows.Forms.CheckBox();
            this.chkSkipExists = new System.Windows.Forms.CheckBox();
            this.lblProgress = new System.Windows.Forms.Label();
            this.txtMulti = new Neetsonic.Control.TextBox();
            this.chkMulti = new System.Windows.Forms.CheckBox();
            this.chkUpdate = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(25, 82);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(52, 20);
            label1.TabIndex = 1;
            label1.Text = "画师ID";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 16);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(65, 20);
            label2.TabIndex = 2;
            label2.Text = "存放目录";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(12, 49);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(65, 20);
            label3.TabIndex = 7;
            label3.Text = "访问代理";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(309, 11);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(61, 29);
            this.btnBrowse.TabIndex = 4;
            this.btnBrowse.Text = "浏览";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.BtnBrowse_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(240, 79);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(130, 59);
            this.btnStart.TabIndex = 5;
            this.btnStart.Text = "开始下载";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.BtnStart_Click);
            // 
            // chkDateAfter
            // 
            this.chkDateAfter.AutoSize = true;
            this.chkDateAfter.Location = new System.Drawing.Point(16, 146);
            this.chkDateAfter.Name = "chkDateAfter";
            this.chkDateAfter.Size = new System.Drawing.Size(210, 24);
            this.chkDateAfter.TabIndex = 9;
            this.chkDateAfter.Text = "仅下载此时间之后上传的作品";
            this.chkDateAfter.UseVisualStyleBackColor = true;
            this.chkDateAfter.CheckedChanged += new System.EventHandler(this.ChkDateAfter_CheckedChanged);
            // 
            // dateAfter
            // 
            this.dateAfter.Enabled = false;
            this.dateAfter.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateAfter.Location = new System.Drawing.Point(240, 144);
            this.dateAfter.Name = "dateAfter";
            this.dateAfter.Size = new System.Drawing.Size(130, 26);
            this.dateAfter.TabIndex = 10;
            // 
            // txtProxy
            // 
            this.txtProxy.Location = new System.Drawing.Point(83, 46);
            this.txtProxy.Name = "txtProxy";
            this.txtProxy.Size = new System.Drawing.Size(220, 26);
            this.txtProxy.TabIndex = 8;
            this.txtProxy.TextChanged += new System.EventHandler(this.TxtProxy_TextChanged);
            // 
            // txtLog
            // 
            this.txtLog.AcceptsReturn = true;
            this.txtLog.AcceptsTab = true;
            this.txtLog.Location = new System.Drawing.Point(16, 176);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(354, 236);
            this.txtLog.TabIndex = 6;
            this.txtLog.WordWrap = false;
            // 
            // txtSaveDir
            // 
            this.txtSaveDir.Location = new System.Drawing.Point(83, 13);
            this.txtSaveDir.Name = "txtSaveDir";
            this.txtSaveDir.Size = new System.Drawing.Size(220, 26);
            this.txtSaveDir.TabIndex = 3;
            // 
            // txtIllustratorID
            // 
            this.txtIllustratorID.Location = new System.Drawing.Point(83, 79);
            this.txtIllustratorID.Name = "txtIllustratorID";
            this.txtIllustratorID.Size = new System.Drawing.Size(138, 26);
            this.txtIllustratorID.TabIndex = 0;
            // 
            // chkManga
            // 
            this.chkManga.AutoSize = true;
            this.chkManga.Checked = true;
            this.chkManga.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkManga.Location = new System.Drawing.Point(16, 421);
            this.chkManga.Name = "chkManga";
            this.chkManga.Size = new System.Drawing.Size(84, 24);
            this.chkManga.TabIndex = 11;
            this.chkManga.Text = "下载漫画";
            this.chkManga.UseVisualStyleBackColor = true;
            // 
            // chkSkipExists
            // 
            this.chkSkipExists.AutoSize = true;
            this.chkSkipExists.Checked = true;
            this.chkSkipExists.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSkipExists.Location = new System.Drawing.Point(106, 421);
            this.chkSkipExists.Name = "chkSkipExists";
            this.chkSkipExists.Size = new System.Drawing.Size(140, 24);
            this.chkSkipExists.TabIndex = 12;
            this.chkSkipExists.Text = "跳过已经下载过的";
            this.chkSkipExists.UseVisualStyleBackColor = true;
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(252, 422);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(0, 20);
            this.lblProgress.TabIndex = 13;
            // 
            // txtMulti
            // 
            this.txtMulti.Location = new System.Drawing.Point(83, 112);
            this.txtMulti.Name = "txtMulti";
            this.txtMulti.Size = new System.Drawing.Size(138, 26);
            this.txtMulti.TabIndex = 14;
            // 
            // chkMulti
            // 
            this.chkMulti.AutoSize = true;
            this.chkMulti.Location = new System.Drawing.Point(21, 114);
            this.chkMulti.Name = "chkMulti";
            this.chkMulti.Size = new System.Drawing.Size(56, 24);
            this.chkMulti.TabIndex = 15;
            this.chkMulti.Text = "批量";
            this.chkMulti.UseVisualStyleBackColor = true;
            this.chkMulti.CheckedChanged += new System.EventHandler(this.ChkMulti_CheckedChanged);
            // 
            // chkUpdate
            // 
            this.chkUpdate.AutoSize = true;
            this.chkUpdate.Location = new System.Drawing.Point(309, 48);
            this.chkUpdate.Name = "chkUpdate";
            this.chkUpdate.Size = new System.Drawing.Size(70, 24);
            this.chkUpdate.TabIndex = 16;
            this.chkUpdate.Text = "仅更新";
            this.chkUpdate.UseVisualStyleBackColor = true;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 452);
            this.Controls.Add(this.chkUpdate);
            this.Controls.Add(this.chkMulti);
            this.Controls.Add(this.txtMulti);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.chkSkipExists);
            this.Controls.Add(this.chkManga);
            this.Controls.Add(this.dateAfter);
            this.Controls.Add(this.chkDateAfter);
            this.Controls.Add(this.txtProxy);
            this.Controls.Add(label3);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtSaveDir);
            this.Controls.Add(label2);
            this.Controls.Add(label1);
            this.Controls.Add(this.txtIllustratorID);
            this.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pixiv下载器";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Neetsonic.Control.TextBox txtIllustratorID;
        private Neetsonic.Control.TextBox txtSaveDir;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnStart;
        private Neetsonic.Control.LogTextBox txtLog;
        private Neetsonic.Control.TextBox txtProxy;
        private System.Windows.Forms.CheckBox chkDateAfter;
        private System.Windows.Forms.DateTimePicker dateAfter;
        private System.Windows.Forms.CheckBox chkManga;
        private System.Windows.Forms.CheckBox chkSkipExists;
        private System.Windows.Forms.Label lblProgress;
        private Neetsonic.Control.TextBox txtMulti;
        private System.Windows.Forms.CheckBox chkMulti;
        private System.Windows.Forms.CheckBox chkUpdate;
    }
}

