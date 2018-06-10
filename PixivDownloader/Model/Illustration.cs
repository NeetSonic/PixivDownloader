namespace PixivDownloader.Model
{
    public class Illustration
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public string URL { get; set; }
        public string Date { get; set; }
        public string FileFormat => URL.Substring(URL.LastIndexOf('.'));
    }
}