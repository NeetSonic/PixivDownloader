using System.IO;
using System.Windows.Forms;
using Neetsonic.Tool;

namespace PixivDownloader.Model
{
    public class LocalConfig
    {
        private const string XmlElementStorePath = @"StorePath";
        private readonly XmlConfigTool config = new XmlConfigTool(Path.Combine(Application.StartupPath, @"config.xml"));

        public string StorePath
        {
            get => config.ReadConfig(XmlElementStorePath, Application.UserAppDataPath);
            set => config.SaveConfig(XmlElementStorePath, value);
        }
    }
}