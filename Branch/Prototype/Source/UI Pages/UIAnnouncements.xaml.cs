using System;
using System.IO;
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

namespace InteractIVLE
{
    public partial class UIAnnouncements : PhoneApplicationPage
    {
        GlobalCache data = GlobalCache.Instance;

        public UIAnnouncements()
        {
            InitializeComponent();
        }

        private void request_CallBack(IAsyncResult result)
        {
            var webRequest = result.AsyncState as HttpWebRequest;
            var response = (HttpWebResponse)webRequest.EndGetResponse(result);
            var baseStream = response.GetResponseStream();        
            
            // Read String response
            using (var reader = new StreamReader(baseStream))
            {
                var Result = reader.ReadToEnd();
                Deployment.Current.Dispatcher.BeginInvoke(() => { textBox1.Text = Result.ToString(); });
                
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            string url = "https://ivle.nus.edu.sg/API/Lapi.svc/Announcements?APIKey=" + data.APIKey + "&AuthToken=" + data.AuthToken + "&CourseID=CS4273" + "&Duration=43200" + "&TitleOnly=" + "true";
            var webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.BeginGetResponse(new AsyncCallback(request_CallBack), webRequest);

        }        
    }


}