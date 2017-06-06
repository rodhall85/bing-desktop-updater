using System;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Configuration;
using System.IO;

namespace NfnProductions.BingDesktopUpdater.Updater {
    public class Program {
        private static void Main() {
            if (ShouldUpdate()) {
                Update();
            }
        }

        private static void Update() {
            using (var client = new WebClient()) {
                try {
                    client.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13";
                    var content = client.DownloadString(ConfigurationManager.AppSettings["BingRestUrl"].ToString());

                    XDocument xDoc = XDocument.Parse(content);
                    string fullImageUrl = xDoc.Descendants("fullImageUrl").FirstOrDefault().Value;

                    if (!string.IsNullOrEmpty(fullImageUrl)) {
                        Wallpaper.Set(new Uri(fullImageUrl), Wallpaper.Style.Stretched);
                        ConfigurationManager.AppSettings["LastUpdated"] = DateTime.Today.ToString();
                    }
                }
                catch (WebException exception) {
                    string responseText;

                    if (exception.Response != null) {
                        var responseStream = exception.Response.GetResponseStream();

                        if (responseStream != null) {
                            using (var reader = new StreamReader(responseStream)) {
                                responseText = reader.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }

        private static bool ShouldUpdate() {
            DateTime lastUpdated = DateTime.Parse(ConfigurationManager.AppSettings["LastUpdated"].ToString());
            return (lastUpdated < DateTime.Today);
        }
    }
}
