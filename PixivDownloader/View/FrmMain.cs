using System;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            Downloader downloader = new Downloader(txtIllustratorID.Text, txtSaveDir.Text, txtProxy.Text, Log, chkDateAfter.Checked ? dateAfter.Value: DateTime.MinValue);
            await Task.Run((Action)downloader.Download);
            txtIllustratorID.Enabled = txtSaveDir.Enabled = btnBrowse.Enabled = btnStart.Enabled = txtProxy.Enabled = true;
        }

        private void ChkDateAfter_CheckedChanged(object sender, EventArgs e) => dateAfter.Enabled = chkDateAfter.Checked;
        private void FrmMain_Load(object sender, EventArgs e)
        {
            txtSaveDir.Text = Global.Config.StorePath;
            txtProxy.Text = Global.Config.Proxy;
        }
        private void Log(string msg) => BeginInvoke(new MethodInvoker(() => txtLog.WriteLog(msg)));
        private void TxtProxy_TextChanged(object sender, EventArgs e) => Global.Config.Proxy = txtProxy.Text;
    }
}