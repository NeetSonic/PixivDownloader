using System;

namespace PixivDownloader.Model
{
    public class Illustration
    {
        public string Illustrator { get; set; }
        public string Name { get; set; }
        public string ID { get; set; }
        public string URL { get; set; }
        public DateTime Date { get; set; }
        public string FileFormat => URL.Substring(URL.LastIndexOf('.'));
    }
}