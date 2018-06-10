using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using Neetsonic.Tool.Extensions;
using PixivDownloader.Model;

namespace PixivDownloader.Util
{
    public sealed class Downloader
    {
        public Downloader(string illustrator_id, string saveDir, Action<string> report)
        {
            _illustratorID = illustrator_id;
            _report = report;
            _saveDir = saveDir;
            _menuURL = string.Format($@"https://www.pixiv.net/member_illust.php?id={illustrator_id}");
        }

        private readonly WebProxy ProxyObject = new WebProxy(@"127.0.0.1:8123", true);
        private readonly string _illustratorID;
        private readonly string _menuURL;
        private readonly string _saveDir;
        private readonly Action<string> _report;

        public void Download()
        {
            ReportMessage(@"下载任务开始！");
            List<Illustration> illustrations = new List<Illustration>();
            int page = 0;
            HtmlNode haveNext;
            string illustrator = null;
            do
            {
                page++;
                ReportMessage(string.Format($@"正在获取第{page}页的作品..."));
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format($@"{_menuURL}&type=all&p={page}"));
                request.Method = "GET";
                request.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                request.Headers.Add("Accept-Encoding", @"gzip, deflate, br");
                request.Headers.Add("Accept-Language", @"zh-CN,zh;q=0.9,ja;q=0.8,en;q=0.7,zh-TW;q=0.6");
                request.Host = @"www.pixiv.net";
                request.SetHeaderValue(@"Connection", @"keep-alive");
                request.Referer = @"https://www.pixiv.net";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";
                request.Headers.Add("Upgrade-Insecure-Requests", @"1");
                request.Headers.Add("Cookie", @"first_visit_datetime_pc=2018-06-08+13%3A48%3A14; p_ab_id=5; p_ab_id_2=2; yuid=GQYEZzA58; __utma=235335808.657713264.1528433295.1528433295.1528433295.1; __utmc=235335808; __utmz=235335808.1528433295.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none); __utmt=1; limited_ads=%7B%22header%22%3A%22%22%7D; _ga=GA1.2.657713264.1528433295; _gid=GA1.2.890661813.1528433319; login_bc=1; PHPSESSID=20722713_06d5f16685a8652a2e6827803858424d; device_token=246818d8418f7e288ecafab2d3f6ae48; privacy_policy_agreement=1; c_type=24; a_type=0; b_type=1; login_ever=yes; __utmv=235335808.|2=login%20ever=yes=1^3=plan=premium=1^5=gender=male=1^6=user_id=20722713=1^9=p_ab_id=5=1^10=p_ab_id_2=2=1^11=lang=zh=1; __utmb=235335808.2.10.1528433295");
                request.Proxy = ProxyObject;
                HtmlDocument doc = new HtmlDocument();
                using(HttpWebResponse myResponse = (HttpWebResponse)request.GetResponse())
                {
                    doc.Load(new GZipStream(myResponse.GetResponseStream(), CompressionMode.Decompress), Encoding.UTF8);
                    myResponse.Close();
                }
                if(null == illustrator) { illustrator = doc.DocumentNode.SelectSingleNode(@"//*[@id='wrapper']/div[1]/div[2]/div/div[1]/div[1]/a[2]").InnerText; }
                int count = doc.DocumentNode.SelectNodes(@"//*[@id='wrapper']/div[1]/div[1]/div/div[2]/ul/li").Count;
                for(int i = 1; i <= count; i++)
                {
                    HtmlNode nodeURL = doc.DocumentNode.SelectSingleNode(string.Format($@"//*[@id='wrapper']/div[1]/div[1]/div/div[2]/ul/li[{i}]/a[1]/div[1]/img"));
                    HtmlNode nodeName = doc.DocumentNode.SelectSingleNode(string.Format($@"//*[@id='wrapper']/div[1]/div[1]/div/div[2]/ul/li[{i}]/a[2]"));
                    string href = nodeName.Attributes[@"href"].Value;
                    string illust_id = href.Substring(href.LastIndexOf('=') + 1);
                    string url = GetURL(illust_id);
                    illustrations.Add(new Illustration
                    {
                            ID = href.Substring(href.LastIndexOf('=') + 1),
                            Name = nodeName.ChildNodes[0].InnerText,
                            URL = url,
                            Date = GetDateByURL(url)
                    });
                }
                haveNext = doc.DocumentNode.SelectSingleNode(@"//*[@id='wrapper']/div[1]/div[1]/div/ul[1]/div/span[2]/a");
            }
            while(haveNext != null);
            string dir = Path.Combine(_saveDir, string.Format($@"{illustrator}_{_illustratorID}"));
            ReportMessage(string.Format($@"创建存储目录[{dir}]..."));
            Directory.CreateDirectory(dir);
            foreach(Illustration illust in illustrations)
            {
                int i = 0;
                while(true)
                {
                    try
                    {
                        if(i > 0)
                        {
                            illust.URL = string.Format($@"{illust.URL.Substring(0, illust.URL.LastIndexOf(string.Format($@"p{i - 1}"), StringComparison.Ordinal))}p{i}{illust.FileFormat}");
                        }
                        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(illust.URL);
                        webRequest.Method = "GET";
                        webRequest.Referer = _menuURL;
                        webRequest.Proxy = ProxyObject;
                        using(HttpWebResponse myResponse = (HttpWebResponse)webRequest.GetResponse())
                        {
                            using(Stream reader = myResponse.GetResponseStream())
                            {
                                string fileName = string.Format($@"{illust.ID}_p{i}_{illust.Name}_{illust.Date}{illust.FileFormat}");
                                string filePath = Path.Combine(dir, fileName);
                                ReportMessage(string.Format($@"下载图片[{fileName}]..."));
                                using(FileStream writer = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                                {
                                    byte[] buff = new byte[512];
                                    int c;
                                    while(reader != null && (c = reader.Read(buff, 0, buff.Length)) > 0)
                                    {
                                        writer.Write(buff, 0, c);
                                    }
                                    writer.Close();
                                }
                                reader?.Close();
                            }
                        }
                        i++;
                    }
                    catch(Exception)
                    {
                        break;
                    }
                }
            }
            ReportMessage(@"下载任务完成！");
        }
        private static string GetDateByURL(string url) => url.Substring(url.LastIndexOf(@"img", StringComparison.Ordinal) + 4, 10).Replace(@"/", string.Empty);
        private string GetURL(string illust_id)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format($@"https://www.pixiv.net/member_illust.php?mode=medium&illust_id={illust_id}"));
            request.Method = "GET";
            request.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Headers.Add("Accept-Encoding", @"gzip, deflate, br");
            request.Headers.Add("Accept-Language", @"zh-CN,zh;q=0.9,ja;q=0.8,en;q=0.7,zh-TW;q=0.6");
            request.Host = @"www.pixiv.net";
            request.SetHeaderValue(@"Connection", @"keep-alive");
            request.Referer = _menuURL;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";
            request.Headers.Add("Upgrade-Insecure-Requests", @"1");
            request.Headers.Add("Cache-Control", @"max-age=0");
            request.Headers.Add("Cookie", @"first_visit_datetime_pc=2018-06-08+13%3A48%3A14; p_ab_id=5; p_ab_id_2=2; yuid=GQYEZzA58; __utmc=235335808; __utmz=235335808.1528433295.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none); limited_ads=%7B%22header%22%3A%22%22%7D; _ga=GA1.2.657713264.1528433295; _gid=GA1.2.890661813.1528433319; login_bc=1; PHPSESSID=20722713_06d5f16685a8652a2e6827803858424d; device_token=246818d8418f7e288ecafab2d3f6ae48; privacy_policy_agreement=1; c_type=24; a_type=0; b_type=1; login_ever=yes; OX_plg=swf|sl|wmp|shk|pm; __utma=235335808.657713264.1528433295.1528441113.1528443138.4; GMOSSP_USER=LxIdoplcRqyfLP96; module_orders_mypage=%5B%7B%22name%22%3A%22sketch_live%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22tag_follow%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22recommended_illusts%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22showcase%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22everyone_new_illusts%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22following_new_illusts%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22mypixiv_new_illusts%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22fanbox%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22featured_tags%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22contests%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22user_events%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22sensei_courses%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22spotlight%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22booth_follow_items%22%2C%22visible%22%3Atrue%7D%5D; is_sensei_service_user=1; __utmv=235335808.|2=login%20ever=yes=1^3=plan=premium=1^5=gender=male=1^6=user_id=20722713=1^9=p_ab_id=5=1^10=p_ab_id_2=2=1^11=lang=zh=1; __gads=ID=aab55b03e6d876e3:T=1528447041:S=ALNI_MYqFjBSkXKUvVBd9kkePwgCzSZTRg; __utmt=1; __utmb=235335808.39.9.1528447071900; tag_view_ranking=I8PKmJXPGb~RTJMXD26Ak~LtW-gO6CmS~4dxKmeew3P~a-yCMcqYxL~aO5pLrFNOB~4uaq8PBs7U~RFVdOq-YjA~Js5EBY4gOW~Ms9Iyj7TRt~Z9VMjBRtsS~YLmVA5rwXe~BU9SQkS-zU~zCtkCvzrOi~EGefOqA6KB~Ow9mLSvmxK~hQzD2l5xLh~4-_9de7LBH~iFcW6hPGPU~PGv6mTCThy~NCZvWchGEm~edXUchbQp5~iajmMoolkv~c8UxuNkgG6~8HRshblb4Q~tgP8r-gOe_~OUkihvwBMZ~cpt_Nk5mjc~lRxin4V3-v~HowAxXvXGp~7tRe7oIUrM~kSnbadY9nM~6gkYqC1mvr~KaIFU4VLba");
            request.Proxy = ProxyObject;
            HtmlDocument doc = new HtmlDocument();
            using(HttpWebResponse myResponse = (HttpWebResponse)request.GetResponse())
            {
                doc.Load(new GZipStream(myResponse.GetResponseStream(), CompressionMode.Decompress), Encoding.UTF8);
                myResponse.Close();
            }
            int begin = doc.ParsedText.IndexOf(@"original", StringComparison.Ordinal);
            int end = doc.ParsedText.IndexOf(@"}", begin, StringComparison.Ordinal);
            string raw = doc.ParsedText.Substring(begin, end - begin + 1);
            begin = raw.IndexOf(@"https", StringComparison.Ordinal);
            end = raw.LastIndexOf('"');
            string ret = raw.Substring(begin, end - begin);
            ret = ret.Replace(@"\", string.Empty);
            return ret;
        }
        private void ReportMessage(string msg) => _report?.Invoke(msg);
    }
}