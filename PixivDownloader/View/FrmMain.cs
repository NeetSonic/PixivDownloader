using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Neetsonic.Tool;
using PixivDownloader.Util;

namespace PixivDownloader.View
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog
            {
                    RootFolder = Environment.SpecialFolder.Desktop
            };
            if(DialogResult.OK == dlg.ShowDialog())
            {
                txtSaveDir.Text = dlg.SelectedPath;
                Global.Config.StorePath = dlg.SelectedPath;
            }
        }
        private async void BtnStart_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
            txtIllustratorID.Enabled = txtSaveDir.Enabled = btnBrowse.Enabled = btnStart.Enabled = txtProxy.Enabled = false;
            if(chkUpdate.Checked)
            {
                foreach(string dir in Directory.GetDirectories(Global.Config.StorePath).Where(s => s.Contains(@"[已整理]")))
                {
                    int flag = dir.LastIndexOf('_');
                    string id = dir.Substring(flag + 1, dir.Length - flag - 1);
                    List<string> fileDate = new List<string>(Directory.GetFiles(dir).Select(s => s.Substring(s.Length - 12, 8)));
                    fileDate.Sort((a, b) => string.Compare(a, b, StringComparison.Ordinal));
                    DateTime after = DateTime.ParseExact(fileDate.Last(), @"yyyyMMdd", CultureInfo.CurrentCulture);
                    txtLog.Clear();
                    Downloader downloader = new Downloader(id, txtSaveDir.Text, txtProxy.Text, Log, after, chkManga.Checked, chkSkipExists.Checked, Progress);
                    await downloader.Download();
                }
            }
            else
            {
                if(chkMulti.Checked)
                {
                    IEnumerable<string> ids = FileTool.OpenAndReadAllText(txtMulti.Text).Split(new[] {Environment.NewLine}, StringSplitOptions.None);
                    foreach(string id in ids)
                    {
                        txtLog.Clear();
                        Downloader downloader = new Downloader(id, txtSaveDir.Text, txtProxy.Text, Log, chkDateAfter.Checked ? dateAfter.Value : DateTime.MinValue, chkManga.Checked, chkSkipExists.Checked, Progress);
                        await downloader.Download();
                    }
                }
                else
                {
                    Downloader downloader = new Downloader(txtIllustratorID.Text, txtSaveDir.Text, txtProxy.Text, Log, chkDateAfter.Checked ? dateAfter.Value : DateTime.MinValue, chkManga.Checked, chkSkipExists.Checked, Progress);
                    await downloader.Download();
                }
            }
            txtIllustratorID.Enabled = txtSaveDir.Enabled = btnBrowse.Enabled = btnStart.Enabled = txtProxy.Enabled = true;
        }

        private void ChkDateAfter_CheckedChanged(object sender, EventArgs e) => dateAfter.Enabled = chkDateAfter.Checked;

        private void ChkMulti_CheckedChanged(object sender, EventArgs e)
        {
            if(chkMulti.Checked)
            {
                OpenFileDialog dlg = new OpenFileDialog {Filter = @"文本文件|*.txt;"};
                if(DialogResult.OK == dlg.ShowDialog())
                {
                    txtMulti.Text = dlg.FileName;
                }
                else
                {
                    chkMulti.Checked = false;
                }
            }
            else
            {
                txtMulti.Clear();
            }
        }
        private void FrmMain_Load(object sender, EventArgs e)
        {
            txtSaveDir.Text = Global.Config.StorePath;
            txtProxy.Text = Global.Config.Proxy;
        }
        private void Log(string msg) => BeginInvoke(new MethodInvoker(() => txtLog.WriteLog(msg)));
        private void Progress(string progress) => BeginInvoke(new MethodInvoker(() => lblProgress.Text = progress));
        private void TxtProxy_TextChanged(object sender, EventArgs e) => Global.Config.Proxy = txtProxy.Text;
    }
}