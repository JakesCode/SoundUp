using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Web;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;

namespace SoundUp
{
    public partial class Form1 : Form
    {
        public class SoundcloudResponse
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string scope { get; set; }
            public string refresh_token { get; set; }
        }





        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.Url = (new Uri("https://soundcloud.com/connect?client_id=efd5e62289b2258912d952bb8c371b44&client_secret=968c115cea2b7e30205d615186ed3b57&redirect_uri=https://soundcloud.com/boatye&response_type=code"));
        }

        private async void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser1.Url.ToString() != "https://soundcloud.com/connect?client_id=efd5e62289b2258912d952bb8c371b44&client_secret=968c115cea2b7e30205d615186ed3b57&redirect_uri=https://soundcloud.com/boatye&response_type=code")
            {
                string code = HttpUtility.ParseQueryString(webBrowser1.Url.Query).Get("code");

                var client = new HttpClient();
                var requestContent = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("client_id", "efd5e62289b2258912d952bb8c371b44"),
                    new KeyValuePair<string, string>("client_secret", "968c115cea2b7e30205d615186ed3b57"),
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("redirect_uri", "https://soundcloud.com/boatye"),
                    new KeyValuePair<string, string>("code", code)
                });
                HttpResponseMessage response = await client.PostAsync(
                    "https://api.soundcloud.com/oauth2/token",
                    requestContent);
                HttpContent responseContent = response.Content;
                using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
                {
                    SoundcloudResponse soundcloud = JsonConvert.DeserializeObject<SoundcloudResponse>(await reader.ReadToEndAsync());
                    MessageBox.Show(soundcloud.access_token);

                    postTrack(soundcloud);
                }

            }
        }

        private async void postTrack(SoundcloudResponse soundcloud)
        {
            var client = new HttpClient();
            var requestContent = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("client_id", "efd5e62289b2258912d952bb8c371b44"),
                    new KeyValuePair<string, string>("client_secret", "968c115cea2b7e30205d615186ed3b57"),
                    new KeyValuePair<string, string>("code", soundcloud.access_token)
            });
            HttpResponseMessage response = await client.PostAsync(
                    "https://api.soundcloud.com/tracks",
                    requestContent);
            HttpContent responseContent = response.Content;
            using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
            {
                MessageBox.Show(await reader.ReadToEndAsync());
            }
        }
    }
}
