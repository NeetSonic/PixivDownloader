using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using Neetsonic.Tool;
using Neetsonic.Tool.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PixivDownloader.Model;

namespace PixivDownloader.Util
{
    public sealed class Downloader
    {
        public Downloader(string illustrator_id, string saveDir, string proxy, Action<string> report, DateTime dateAfter, bool downloadManga)
        {
            IllustratorID = illustrator_id;
            if(!string.IsNullOrWhiteSpace(proxy)) { ProxyObject = new WebProxy(proxy, true); }
            Report = report;
            DateAfter = dateAfter;
            DownloadManga = downloadManga;
            SaveDir = saveDir;
            MenuURL = string.Format($@"https://www.pixiv.net/member_illust.php?id={illustrator_id}");
        }

        private readonly WebProxy ProxyObject;
        private readonly string IllustratorID;
        private readonly string MenuURL;
        private readonly string SaveDir;
        private readonly Action<string> Report;
        private readonly DateTime DateAfter;
        private readonly bool DownloadManga;

        public void Download()
        {
            ReportMessage(@"下载任务开始！");
            List<Illustration> illustrations = new List<Illustration>();
            string illustrator = null;
            ReportMessage(@"正在获取作品...");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format($@"https://www.pixiv.net/ajax/user/{IllustratorID}/profile/all"));
            request.SetHeaderValue(@"authority", @"www.pixiv.net");
            request.Method = "GET";
            request.SetHeaderValue(@"path", string.Format($@"/ajax/user/{IllustratorID}/profile/all"));
            request.SetHeaderValue(@"scheme", @"https");
            request.Accept = @"application/json";
            request.Headers.Add("accept-encoding", @"gzip, deflate, br");
            request.Headers.Add("accept-language", @"zh-CN,zh;q=0.9,ja;q=0.8,en;q=0.7,zh-TW;q=0.6");
            request.Headers.Add("Cookie", @"p_ab_id=7; p_ab_id_2=4; _ga=GA1.2.934273458.1521364658; PHPSESSID=20722713_cc96e4d3fce7f41fc42fd9668e1bb134; c_type=24; a_type=0; b_type=1; login_ever=yes; bookmark_tag_type=count; bookmark_tag_order=desc; module_orders_mypage=%5B%7B%22name%22%3A%22sketch_live%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22tag_follow%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22recommended_illusts%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22showcase%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22everyone_new_illusts%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22following_new_illusts%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22mypixiv_new_illusts%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22fanbox%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22featured_tags%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22contests%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22user_events%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22sensei_courses%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22spotlight%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22booth_follow_items%22%2C%22visible%22%3Atrue%7D%5D; __gads=ID=7e5541f83f2cab50:T=1527099175:S=ALNI_MbqpeLl8UPfDqpOUEqOuBZHo47CVw; first_visit_datetime_pc=2018-05-28+19%3A33%3A26; privacy_policy_agreement=0; GMOSSP_USER=iBAxEvQYugslXSID; __utmv=235335808.|2=login%20ever=yes=1^3=plan=premium=1^5=gender=male=1^6=user_id=20722713=1^9=p_ab_id=7=1^10=p_ab_id_2=4=1^11=lang=zh=1; ki_r=; ki_u=8801c14c-374f-c637-76ce-c8c2; __utmz=235335808.1535791793.73.31.utmcsr=saucenao.com|utmccn=(referral)|utmcmd=referral|utmcct=/search.php; auto_view_enabled=1; yuid_b=EkUFeFc; p_ab_d_id=1042682999; bookToggle=cloud; is_sensei_service_user=1; tags_sended=1; ki_s=189784%3A1.0.0.0.1%3B190555%3A0.0.0.0.0%3B191516%3A0.0.0.0.0; __utma=235335808.934273458.1521364658.1537973667.1538820366.78; limited_ads=%7B%22header%22%3A%22%22%2C%22responsive%22%3A%22%22%2C%22illust_responsive%22%3A%22%22%7D; tag_view_ranking=0xsDLqCEW6~RTJMXD26Ak~Ie2c51_4Sp~tgP8r-gOe_~iFcW6hPGPU~EGefOqA6KB~faHcYIP1U0~BU9SQkS-zU~y8GNntYHsi~qvqXJkzT2e~nRnn8VBmkN~KN7uxuR89w~5oPIfUbtd6~azESOjmQSV~ueeKYaEKwj~RcahSSzeRf~-_xRkJ5N3r~YRDwjaiLZn~_pwIgrV8TB~3gc3uGrU1V~AI_aJCDFn0~zyKU3Q5L4C~mLrrjwTHBm~d-u0duThlB~ZCmbgGhY5R~2-ldUidl2y~AMwBN_-Fo_~TmJBC3K3bw~xL8L4NxzvM~CiSfl_AE0h~udkRh_mjvd~1mFsW1xBBW~a3zvshTj4U~pnCQRVigpy~Lt-oEicbBr~q3eUobDMJW~NpsIVvS-GF~JN2fNJ_Ue2~zpxRZSQQmq~gooMLQqB9a~qDqmZnzmtE~pzzjRSV6ZO~ZTBAtZUDtQ~HY55MqmzzQ~TilVzXRalL~7X9tDmTHTN~E8zdna9OwN~WlKkwEuUi0~FH69TLSzdM~E2creqW-lX~HzBi0V_d0-~W8b5FozT7j~CwLGRJQEGQ~xZ6jtQjaj9~mzJgaDwBF5~ouiK2OKQ-A~_RfiUqtsxe~kRVrU5mTs2~Bd2L9ZBE8q~8d67oBa0hs~6s8EWwIFtr~UgLGWGysi-~G_f4j5NH8i~dxe3abx4rF~dqqWNpq7ul~fafuO7KcEk~XHvHN9WS_x~dGrh46_SR4~dKg5JtGqUU~QaiOjmwQnI~X_1kwTzaXt~VLb0kLlPK2~CrFcrMFJzz~gVfGX_rH_Y~LSG3qSZIDS~fg8EOt4owo~JxWMqWYB6S~GgKeUugbDA~bopfpc8En6~gpglyfLkWs~0F4QqxTayD~TWrozby2UO~w8ffkPoJ_S~E1yS6jImG-~CLR9k9dHAQ~Hjx7wJwsUT~89uzPVUy9U~52-15K-YXl~-V9Dhj6ijP~bUvbcXvcCG~L58xyNakWW~E6USWSAVMi~ZnmOm5LdC3~dCgGY2jML4~OxeWGr811U~j2Cs25NHKk~wgrA9w_7EX~D0nMcn6oGk~k6GrMC5bzc~WI561SX4pn; ki_t=1533556001787%3B1538820347134%3B1538825155853%3B6%3B23");
            request.Referer = string.Format($@"https://www.pixiv.net/member.php?id={IllustratorID}");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";
            if(null != ProxyObject) { request.Proxy = ProxyObject; }
            List<string> illustIDs = new List<string>();
            List<string> mangaIDs = new List<string>();
            using(HttpWebResponse myResponse = (HttpWebResponse)request.GetResponse())
            {
                using(StreamReader reader = new StreamReader(new GZipStream(myResponse.GetResponseStream(), CompressionMode.Decompress), Encoding.UTF8))
                {
                    JObject illustratorInfo = JsonConvert.DeserializeObject<JObject>(reader.ReadToEnd());
                    JObject illusts = illustratorInfo["body"].Value<JObject>("illusts");
                    illustIDs.AddRange(illusts.Cast<KeyValuePair<string, JToken>>().Select(item => item.Key));
                    if(DownloadManga)
                    {
                        JObject mangas = illustratorInfo["body"].Value<JObject>("manga");
                        mangaIDs.AddRange(mangas.Cast<KeyValuePair<string, JToken>>().Select(item => item.Key));
                    }
                    reader.Close();
                }
                myResponse.Close();
            }
            foreach(string illustID in illustIDs)
            {
                Illustration illust = GetIllustration(illustID);
                if(illust.Date < DateAfter.Date)
                {
                    break;
                }
                if(null == illustrator) { illustrator = illust.Illustrator; }
                illustrations.Add(illust);
            }
            foreach(string mangaID in mangaIDs)
            {
                Illustration manga = GetIllustration(mangaID);
                if(manga.Date < DateAfter.Date)
                {
                    break;
                }
                if(null == illustrator)
                { illustrator = manga.Illustrator; }
                illustrations.Add(manga);
            }
            string dir = null;
            foreach(string directory in Directory.GetDirectories(SaveDir))
            {
                if(directory.EndsWith($@"_{IllustratorID}"))
                {
                    dir = directory;
                    break;
                }
            }
            if(null == dir)
            {
                dir = Path.Combine(SaveDir, string.Format($@"{illustrator}_{IllustratorID}"));
                ReportMessage(string.Format($@"创建存储目录[{dir}]..."));
                Directory.CreateDirectory(dir);
            }
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
                        webRequest.Referer = MenuURL;
                        if(null != ProxyObject) { webRequest.Proxy = ProxyObject; }
                        using(HttpWebResponse myResponse = (HttpWebResponse)webRequest.GetResponse())
                        {
                            using(Stream reader = myResponse.GetResponseStream())
                            {
                                string fileName = string.Format($@"{illust.ID}_p{i}_{illust.Name}_{illust.Date:yyyyMMdd}{illust.FileFormat}");
                                fileName = FileTool.LegalizeFileName(fileName);
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
                    catch(Exception) { break; }
                }
            }
            ReportMessage(@"下载任务完成！");
            FileTool.OpenDirectory(dir);
        }
        private Illustration GetIllustration(string illust_id)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format($@"https://www.pixiv.net/ajax/illust/{illust_id}"));
            request.SetHeaderValue(@"authority", @"www.pixiv.net");
            request.Method = "GET";
            request.SetHeaderValue(@"path", string.Format($@"/ajax/illust/{illust_id}"));
            request.SetHeaderValue(@"scheme", @"https");
            request.Accept = @"application/json";
            request.Headers.Add("accept-encoding", @"gzip, deflate, br");
            request.Headers.Add("accept-language", @"zh-CN,zh;q=0.9,ja;q=0.8,en;q=0.7,zh-TW;q=0.6");
            request.Headers.Add("Cookie", @"p_ab_id=7; p_ab_id_2=4; _ga=GA1.2.934273458.1521364658; PHPSESSID=20722713_cc96e4d3fce7f41fc42fd9668e1bb134; c_type=24; a_type=0; b_type=1; login_ever=yes; bookmark_tag_type=count; bookmark_tag_order=desc; module_orders_mypage=%5B%7B%22name%22%3A%22sketch_live%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22tag_follow%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22recommended_illusts%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22showcase%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22everyone_new_illusts%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22following_new_illusts%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22mypixiv_new_illusts%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22fanbox%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22featured_tags%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22contests%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22user_events%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22sensei_courses%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22spotlight%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22booth_follow_items%22%2C%22visible%22%3Atrue%7D%5D; __gads=ID=7e5541f83f2cab50:T=1527099175:S=ALNI_MbqpeLl8UPfDqpOUEqOuBZHo47CVw; first_visit_datetime_pc=2018-05-28+19%3A33%3A26; privacy_policy_agreement=0; GMOSSP_USER=iBAxEvQYugslXSID; __utmv=235335808.|2=login%20ever=yes=1^3=plan=premium=1^5=gender=male=1^6=user_id=20722713=1^9=p_ab_id=7=1^10=p_ab_id_2=4=1^11=lang=zh=1; ki_r=; ki_u=8801c14c-374f-c637-76ce-c8c2; __utmz=235335808.1535791793.73.31.utmcsr=saucenao.com|utmccn=(referral)|utmcmd=referral|utmcct=/search.php; auto_view_enabled=1; yuid_b=EkUFeFc; p_ab_d_id=1042682999; bookToggle=cloud; is_sensei_service_user=1; tags_sended=1; ki_s=189784%3A1.0.0.0.1%3B190555%3A0.0.0.0.0%3B191516%3A0.0.0.0.0; __utma=235335808.934273458.1521364658.1537973667.1538820366.78; ki_t=1533556001787%3B1538820347134%3B1538825155853%3B6%3B23; limited_ads=%7B%22header%22%3A%22%22%2C%22responsive%22%3A%22%22%2C%22illust_responsive%22%3A%226829%22%7D; tag_view_ranking=0xsDLqCEW6~RTJMXD26Ak~Ie2c51_4Sp~tgP8r-gOe_~iFcW6hPGPU~EGefOqA6KB~faHcYIP1U0~BU9SQkS-zU~nRnn8VBmkN~qvqXJkzT2e~y8GNntYHsi~5oPIfUbtd6~KN7uxuR89w~azESOjmQSV~YRDwjaiLZn~RcahSSzeRf~_pwIgrV8TB~AI_aJCDFn0~mLrrjwTHBm~3gc3uGrU1V~ueeKYaEKwj~zyKU3Q5L4C~Lt-oEicbBr~udkRh_mjvd~q3eUobDMJW~AMwBN_-Fo_~d-u0duThlB~pnCQRVigpy~a3zvshTj4U~1mFsW1xBBW~xL8L4NxzvM~TmJBC3K3bw~-_xRkJ5N3r~CiSfl_AE0h~2-ldUidl2y~ZCmbgGhY5R~gooMLQqB9a~E8zdna9OwN~CwLGRJQEGQ~qDqmZnzmtE~zpxRZSQQmq~7X9tDmTHTN~uukX-mN8J7~NpsIVvS-GF~78lv_o5xOl~TilVzXRalL~JN2fNJ_Ue2~ZTBAtZUDtQ~FH69TLSzdM~HzBi0V_d0-~E2creqW-lX~WlKkwEuUi0~pzzjRSV6ZO~W8b5FozT7j~SuVYClquvg~QaiOjmwQnI~gpglyfLkWs~kRVrU5mTs2~E1yS6jImG-~bopfpc8En6~LSG3qSZIDS~JxWMqWYB6S~X_1kwTzaXt~ouiK2OKQ-A~mzJgaDwBF5~dqqWNpq7ul~UgLGWGysi-~6s8EWwIFtr~8d67oBa0hs~0F4QqxTayD~52-15K-YXl~CLR9k9dHAQ~gVfGX_rH_Y~Hjx7wJwsUT~XHvHN9WS_x~HY55MqmzzQ~fafuO7KcEk~dxe3abx4rF~G_f4j5NH8i~Bd2L9ZBE8q~_RfiUqtsxe~TWrozby2UO~GgKeUugbDA~xZ6jtQjaj9~89uzPVUy9U~w8ffkPoJ_S~fg8EOt4owo~VLb0kLlPK2~CrFcrMFJzz~dKg5JtGqUU~dGrh46_SR4~pYlUxeIoeg~n2tofz1Xl7~dCgGY2jML4~K8esoIs2eW~OxeWGr811U~u8McsBs7WV~j2Cs25NHKk~SqrPRxU2RZ~wgrA9w_7EX");
            request.Referer = string.Format($@"https://www.pixiv.net/member_illust.php?mode=medium&illust_id={illust_id}");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";
            if(null != ProxyObject) { request.Proxy = ProxyObject; }
            Illustration illustration = new Illustration();
            using(HttpWebResponse myResponse = (HttpWebResponse)request.GetResponse())
            {
                using(StreamReader reader = new StreamReader(new GZipStream(myResponse.GetResponseStream(), CompressionMode.Decompress), Encoding.UTF8))
                {
                    JObject illustInfo = JsonConvert.DeserializeObject<JObject>(reader.ReadToEnd());
                    illustration.Name = illustInfo["body"].Value<string>("illustTitle");
                    illustration.Date = illustInfo["body"].Value<DateTime>("uploadDate");
                    illustration.ID = illust_id;
                    illustration.URL = illustInfo["body"]["urls"].Value<string>("original");
                    illustration.Illustrator = illustInfo["body"].Value<string>("userName");
                    reader.Close();
                }
                myResponse.Close();
            }
            return illustration;
        }
        private void ReportMessage(string msg) => Report?.Invoke(msg);
    }
}