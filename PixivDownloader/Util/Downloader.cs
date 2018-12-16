using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Neetsonic.Tool;
using Neetsonic.Tool.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PixivDownloader.Model;

namespace PixivDownloader.Util
{
    public sealed class Downloader
    {
        public Downloader(string illustrator_id, string saveDir, string proxy, Action<string> report, DateTime dateAfter, bool downloadManga, bool skipExists, Action<string> reportProgress)
        {
            IllustratorID = illustrator_id;
            if(!string.IsNullOrWhiteSpace(proxy)) { ProxyObject = new WebProxy(proxy, true); }
            Report = report;
            DateAfter = dateAfter;
            DownloadManga = downloadManga;
            SaveDir = saveDir;
            SkipExists = skipExists;
            ReportProgress = reportProgress;
            MenuURL = string.Format($@"https://www.pixiv.net/member_illust.php?id={illustrator_id}");
        }

        private readonly WebProxy ProxyObject;
        private readonly string IllustratorID;
        private readonly string MenuURL;
        private readonly string SaveDir;
        private readonly Action<string> Report;
        private readonly Action<string> ReportProgress;
        private readonly DateTime DateAfter;
        private readonly bool DownloadManga;
        private readonly bool SkipExists;

        public async Task Download()
        {
            await Task.Run(() =>
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
                request.Headers.Add("Cookie", @"p_ab_id=7; p_ab_id_2=4; _ga=GA1.2.934273458.1521364658; c_type=24; a_type=0; b_type=1; login_ever=yes; bookmark_tag_type=count; bookmark_tag_order=desc; __gads=ID=7e5541f83f2cab50:T=1527099175:S=ALNI_MbqpeLl8UPfDqpOUEqOuBZHo47CVw; first_visit_datetime_pc=2018-05-28+19%3A33%3A26; GMOSSP_USER=iBAxEvQYugslXSID; ki_r=; ki_u=8801c14c-374f-c637-76ce-c8c2; auto_view_enabled=1; yuid_b=EkUFeFc; p_ab_d_id=1042682999; bookToggle=cloud; ki_s=189784%3A1.0.0.0.1%3B190555%3A0.0.0.0.0%3B191516%3A0.0.0.0.0; __utmz=235335808.1539605149.81.34.utmcsr=t.co|utmccn=(referral)|utmcmd=referral|utmcct=/aFTZ3RDel5; module_orders_mypage=%5B%7B%22name%22%3A%22sketch_live%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22tag_follow%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22recommended_illusts%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22everyone_new_illusts%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22following_new_illusts%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22mypixiv_new_illusts%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22fanbox%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22featured_tags%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22contests%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22user_events%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22sensei_courses%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22spotlight%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22booth_follow_items%22%2C%22visible%22%3Atrue%7D%5D; is_sensei_service_user=1; __utmc=235335808; tags_sended=1; privacy_policy_agreement=1; login_bc=1; _gid=GA1.2.1718057145.1539787105; PHPSESSID=20722713_4c61193979bcb02f05cb64c2518d9ca8; device_token=bc6918a48447d2b4829e54728fbd722f; __utma=235335808.934273458.1521364658.1539779770.1539788472.83; __utmv=235335808.|2=login%20ever=yes=1^3=plan=premium=1^5=gender=male=1^6=user_id=20722713=1^9=p_ab_id=7=1^10=p_ab_id_2=4=1^11=lang=zh=1; ki_t=1533556001787%3B1539779740745%3B1539790317873%3B8%3B32; tag_view_ranking=0xsDLqCEW6~tgP8r-gOe_~RTJMXD26Ak~iFcW6hPGPU~Ie2c51_4Sp~faHcYIP1U0~EGefOqA6KB~BU9SQkS-zU~qvqXJkzT2e~KN7uxuR89w~y8GNntYHsi~nRnn8VBmkN~RcahSSzeRf~d-u0duThlB~udkRh_mjvd~azESOjmQSV~ZCmbgGhY5R~gpglyfLkWs~_pwIgrV8TB~oCR2Pbz1ly~f-c_0dUV8c~HY55MqmzzQ~ZTBAtZUDtQ~plqXT5B4--~Bd2L9ZBE8q~AI_aJCDFn0~5oPIfUbtd6~zpxRZSQQmq~ueeKYaEKwj~CiSfl_AE0h~qiO14cZMBI~xL8L4NxzvM~1mFsW1xBBW~a3zvshTj4U~pnCQRVigpy~AMwBN_-Fo_~YRDwjaiLZn~SuVYClquvg~xZ6jtQjaj9~1F9SMtTyiX~Lt-oEicbBr~pzzjRSV6ZO~3gc3uGrU1V~mzJgaDwBF5~CwLGRJQEGQ~qDqmZnzmtE~bXMh6mBhl8~at5kYG0Mvu~CrFcrMFJzz~n2tofz1Xl7~_hSAdpN9rx~zyKU3Q5L4C~78lv_o5xOl~uukX-mN8J7~gooMLQqB9a~7X9tDmTHTN~wyZOlBKxtg~tJVyo5DCeS~GX5cZxE2GY~E1yS6jImG-~w8ffkPoJ_S~FH69TLSzdM~Vb-jIgE23z~kRVrU5mTs2~TilVzXRalL~JN2fNJ_Ue2~WlKkwEuUi0~XHvHN9WS_x~Hjx7wJwsUT~gVfGX_rH_Y~fb-npFifEY~0F4QqxTayD~2QTW_H5tVX~ouiK2OKQ-A~X_1kwTzaXt~JxWMqWYB6S~LSG3qSZIDS~q3eUobDMJW~mLrrjwTHBm~dqqWNpq7ul~UgLGWGysi-~6s8EWwIFtr~-_xRkJ5N3r~QaiOjmwQnI~fafuO7KcEk~dxe3abx4rF~G_f4j5NH8i~NpsIVvS-GF~bopfpc8En6~5OpNiTfB6S~BAOGRD22OG~LxdXI7-B2R~GC-eDdG096~28gdfFXlY7~RfGr66d1Lx~XYT8kV_Hul~pGv7p05oAU~JXguVGpfOO~k6GrMC5bzc~KS5kY2k_1V; limited_ads=%7B%22header%22%3A%22%22%2C%22responsive%22%3A%22%22%2C%22illust_responsive%22%3A%226829~6827%22%7D");
                request.Referer = string.Format($@"https://www.pixiv.net/member.php?id={IllustratorID}");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";
                if(null != ProxyObject)
                { request.Proxy = ProxyObject; }
                List<string> illustIDs = new List<string>();
                List<string> mangaIDs = new List<string>();
                using(HttpWebResponse myResponse = (HttpWebResponse)request.GetResponse())
                {
                    using(StreamReader reader = new StreamReader(new GZipStream(myResponse.GetResponseStream(), CompressionMode.Decompress), Encoding.UTF8))
                    {
                        string json = reader.ReadToEnd();
                        JObject illustratorInfo = JsonConvert.DeserializeObject<JObject>(json);
                        JObject illusts = illustratorInfo["body"].Value<JObject>("illusts");
                        illustIDs.AddRange(illusts.Cast<KeyValuePair<string, JToken>>().Select(item => item.Key));
                        if(DownloadManga && illustratorInfo["body"]["manga"].HasValues)
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
                    if(null == illustrator)
                    { illustrator = illust.Illustrator; }
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
                        ReportMessage(string.Format($@"定位到存储目录[{dir}]..."));
                        break;
                    }
                }
                if(null == dir)
                {
                    dir = Path.Combine(SaveDir, string.Format($@"{FileTool.LegalizeFileName(illustrator)}_{IllustratorID}"));
                    ReportMessage(string.Format($@"创建存储目录[{dir}]..."));
                    Directory.CreateDirectory(dir);
                }
                int progress = 0;
                foreach(Illustration illust in illustrations)
                {
                    progress++;
                    ReportProgress(string.Format($@"进度[{progress}/{illustrations.Count}]"));
                    int i = 0;
                    while(true)
                    {
                        try
                        {
                            if(i > 0)
                            {
                                illust.URL = string.Format($@"{illust.URL.Substring(0, illust.URL.LastIndexOf(string.Format($@"p{i - 1}"), StringComparison.Ordinal))}p{i}{illust.FileFormat}");
                            }
                            string fileName = string.Format($@"{illust.ID}_p{i}_{illust.Name}_{illust.Date:yyyyMMdd}{illust.FileFormat}");
                            fileName = FileTool.LegalizeFileName(fileName);
                            string filePath = Path.Combine(dir, fileName);
                            if(SkipExists && File.Exists(filePath))
                            {
                                ReportMessage(string.Format($@"跳过下载[{fileName}]..."));
                                i++;
                                continue;
                            }
                            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(illust.URL);
                            webRequest.Method = "GET";
                            webRequest.Referer = MenuURL;
                            if(null != ProxyObject)
                            { webRequest.Proxy = ProxyObject; }
                            using(HttpWebResponse myResponse = (HttpWebResponse)webRequest.GetResponse())
                            {
                                using(Stream reader = myResponse.GetResponseStream())
                                {
                                    ReportMessage(string.Format($@"下载图片[{fileName}]..."));
                                    File.Delete(filePath);
                                    using(FileStream writer = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                                    {
                                        byte[] buff = new byte[512];
                                        int bytesRead;
                                        while(reader != null && (bytesRead = reader.Read(buff, 0, buff.Length)) > 0)
                                        {
                                            writer.Write(buff, 0, bytesRead);
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
            });
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
            request.Headers.Add("Cookie", @"p_ab_id=7; p_ab_id_2=4; _ga=GA1.2.934273458.1521364658; c_type=24; a_type=0; b_type=1; login_ever=yes; bookmark_tag_type=count; bookmark_tag_order=desc; __gads=ID=7e5541f83f2cab50:T=1527099175:S=ALNI_MbqpeLl8UPfDqpOUEqOuBZHo47CVw; first_visit_datetime_pc=2018-05-28+19%3A33%3A26; GMOSSP_USER=iBAxEvQYugslXSID; ki_r=; ki_u=8801c14c-374f-c637-76ce-c8c2; auto_view_enabled=1; yuid_b=EkUFeFc; p_ab_d_id=1042682999; bookToggle=cloud; ki_s=189784%3A1.0.0.0.1%3B190555%3A0.0.0.0.0%3B191516%3A0.0.0.0.0; __utmz=235335808.1539605149.81.34.utmcsr=t.co|utmccn=(referral)|utmcmd=referral|utmcct=/aFTZ3RDel5; module_orders_mypage=%5B%7B%22name%22%3A%22sketch_live%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22tag_follow%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22recommended_illusts%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22everyone_new_illusts%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22following_new_illusts%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22mypixiv_new_illusts%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22fanbox%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22featured_tags%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22contests%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22user_events%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22sensei_courses%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22spotlight%22%2C%22visible%22%3Atrue%7D%2C%7B%22name%22%3A%22booth_follow_items%22%2C%22visible%22%3Atrue%7D%5D; is_sensei_service_user=1; __utmc=235335808; tags_sended=1; privacy_policy_agreement=1; login_bc=1; _gid=GA1.2.1718057145.1539787105; PHPSESSID=20722713_4c61193979bcb02f05cb64c2518d9ca8; device_token=bc6918a48447d2b4829e54728fbd722f; __utma=235335808.934273458.1521364658.1539779770.1539788472.83; __utmv=235335808.|2=login%20ever=yes=1^3=plan=premium=1^5=gender=male=1^6=user_id=20722713=1^9=p_ab_id=7=1^10=p_ab_id_2=4=1^11=lang=zh=1; ki_t=1533556001787%3B1539779740745%3B1539790317873%3B8%3B32; tag_view_ranking=0xsDLqCEW6~tgP8r-gOe_~RTJMXD26Ak~iFcW6hPGPU~Ie2c51_4Sp~faHcYIP1U0~EGefOqA6KB~BU9SQkS-zU~qvqXJkzT2e~KN7uxuR89w~y8GNntYHsi~nRnn8VBmkN~RcahSSzeRf~d-u0duThlB~udkRh_mjvd~azESOjmQSV~ZCmbgGhY5R~gpglyfLkWs~_pwIgrV8TB~oCR2Pbz1ly~f-c_0dUV8c~HY55MqmzzQ~ZTBAtZUDtQ~plqXT5B4--~Bd2L9ZBE8q~AI_aJCDFn0~5oPIfUbtd6~zpxRZSQQmq~ueeKYaEKwj~CiSfl_AE0h~qiO14cZMBI~xL8L4NxzvM~1mFsW1xBBW~a3zvshTj4U~pnCQRVigpy~AMwBN_-Fo_~YRDwjaiLZn~SuVYClquvg~xZ6jtQjaj9~1F9SMtTyiX~Lt-oEicbBr~pzzjRSV6ZO~3gc3uGrU1V~mzJgaDwBF5~CwLGRJQEGQ~qDqmZnzmtE~bXMh6mBhl8~at5kYG0Mvu~CrFcrMFJzz~n2tofz1Xl7~_hSAdpN9rx~zyKU3Q5L4C~78lv_o5xOl~uukX-mN8J7~gooMLQqB9a~7X9tDmTHTN~wyZOlBKxtg~tJVyo5DCeS~GX5cZxE2GY~E1yS6jImG-~w8ffkPoJ_S~FH69TLSzdM~Vb-jIgE23z~kRVrU5mTs2~TilVzXRalL~JN2fNJ_Ue2~WlKkwEuUi0~XHvHN9WS_x~Hjx7wJwsUT~gVfGX_rH_Y~fb-npFifEY~0F4QqxTayD~2QTW_H5tVX~ouiK2OKQ-A~X_1kwTzaXt~JxWMqWYB6S~LSG3qSZIDS~q3eUobDMJW~mLrrjwTHBm~dqqWNpq7ul~UgLGWGysi-~6s8EWwIFtr~-_xRkJ5N3r~QaiOjmwQnI~fafuO7KcEk~dxe3abx4rF~G_f4j5NH8i~NpsIVvS-GF~bopfpc8En6~5OpNiTfB6S~BAOGRD22OG~LxdXI7-B2R~GC-eDdG096~28gdfFXlY7~RfGr66d1Lx~XYT8kV_Hul~pGv7p05oAU~JXguVGpfOO~k6GrMC5bzc~KS5kY2k_1V; limited_ads=%7B%22header%22%3A%22%22%2C%22responsive%22%3A%22%22%2C%22illust_responsive%22%3A%226829~6827%22%7D");
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