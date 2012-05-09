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

namespace WP7
{
    public partial class Display : PhoneApplicationPage
    {
        private string API_Key, API_Token;


        public Display()
        {
            InitializeComponent();
        }

        private void request_CallBack(IAsyncResult result)
        {
            var webRequest = result.AsyncState as HttpWebRequest;
            var response = (HttpWebResponse)webRequest.EndGetResponse(result);
            var baseStream = response.GetResponseStream();

            // if you want to read binary response
            //using (var reader = new BinaryReader(baseStream))
            //{
            //    var DataBytes = reader.ReadBytes((int)baseStream.Length);
            //}
            
            // if you want to read string response
            using (var reader = new StreamReader(baseStream))
            {
                var Result = reader.ReadToEnd();
                Deployment.Current.Dispatcher.BeginInvoke(() => { textBox1.Text = Result.ToString(); });
                
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            string url = "https://ivle.nus.edu.sg/API/Lapi.svc/Announcements?APIKey=" + API_Key + "&AuthToken=" + API_Token + "&CourseID=CS4273" + "&Duration=43200" + "&TitleOnly=" + "true";
            var webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.BeginGetResponse(new AsyncCallback(request_CallBack), webRequest);

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (this.NavigationContext.QueryString.ContainsKey("token"))
            {
                this.API_Token = this.NavigationContext.QueryString["token"];
            }
            this.API_Key = cLAPI.APIKey;

            base.OnNavigatedTo(e);
        }
    }


}