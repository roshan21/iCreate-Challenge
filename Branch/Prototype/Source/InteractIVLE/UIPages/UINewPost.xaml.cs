using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using InteractIVLE.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace InteractIVLE.UIPages
{
    public partial class UINewPost : PhoneApplicationPage
    {
        GlobalCache data = GlobalCache.Instance;

        public UINewPost()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            WebClient webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            var uri = new Uri("https://ivle.nus.edu.sg/api/Lapi.svc/Forum_PostNewThread_JSON", UriKind.Absolute);
            StringBuilder postData = new StringBuilder();
            postData.AppendFormat("{0}={1}", "APIKey", HttpUtility.UrlEncode(data.APIKey));
            postData.AppendFormat("&{0}={1}", "AuthToken", HttpUtility.UrlEncode(data.AuthToken));
            postData.AppendFormat("&{0}={1}", "HeadingID", HttpUtility.UrlEncode(data.curHeadingID));
            postData.AppendFormat("&{0}={1}", "Title", HttpUtility.UrlEncode(textBox1.Text));
            postData.AppendFormat("&{0}={1}", "Reply", HttpUtility.UrlEncode(textBox2.Text));
            webClient.Headers[HttpRequestHeader.ContentLength] = postData.Length.ToString();
            webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(webClient_UploadPostStringCompleted);
            webClient.UploadProgressChanged += webClient_UploadPostProgressChanged;
            webClient.UploadStringAsync(uri, "POST", postData.ToString());
        }
        
        void webClient_UploadPostStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            System.Console.WriteLine("completed");
            NavigationService.Navigate(new Uri("/UIPages/UIForum.xaml", UriKind.Relative));
        }

        void webClient_UploadPostProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            System.Console.WriteLine(string.Format("Progress: {0} ", e.ProgressPercentage));
        }

        //END - ADDED: CODE FOR CREATING NEW POST

        //ADDED: CODE FOR REPLYING TO A THREAD

        private void btn_reply_Click(object sender, RoutedEventArgs e)
        {
            if (data.forumPosts[data.forumPosts.Count - 1] != null)
            {
                WebClient webClient = new WebClient();
                webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var uri = new Uri("https://ivle.nus.edu.sg/api/Lapi.svc/Forum_PostNewThread_JSON", UriKind.Absolute);
                StringBuilder postData = new StringBuilder();
                postData.AppendFormat("{0}={1}", "APIKey", HttpUtility.UrlEncode(data.APIKey));
                postData.AppendFormat("&{0}={1}", "AuthToken", HttpUtility.UrlEncode(data.AuthToken));
                postData.AppendFormat("&{0}={1}", "HeadingID", HttpUtility.UrlEncode(data.curHeadingID));
                postData.AppendFormat("&{0}={1}", "Title", HttpUtility.UrlEncode(textBox1.Text));
                postData.AppendFormat("&{0}={1}", "Reply", HttpUtility.UrlEncode(textBox2.Text));
                webClient.Headers[HttpRequestHeader.ContentLength] = postData.Length.ToString();
                webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(webClient_UploadReplyStringCompleted);
                webClient.UploadProgressChanged += webClient_UploadReplyProgressChanged;
                webClient.UploadStringAsync(uri, "POST", postData.ToString());
            }
        }

        void webClient_UploadReplyStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            System.Console.WriteLine("completed");
        }

        void webClient_UploadReplyProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            System.Console.WriteLine(string.Format("Progress: {0} ", e.ProgressPercentage));
        }
        // End Roshan
       
    }
}