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
using System.IO;
using InteractIVLE.Data;

namespace InteractIVLE.UIPages
{
    public partial class UIOrganizer : PhoneApplicationPage
    {
        GlobalCache data = GlobalCache.Instance;

        public UIOrganizer()
        {
            InitializeComponent();
        }        

        private void callback_time_table(IAsyncResult result)
        {
            var webRequest = result.AsyncState as HttpWebRequest;
            var response = (HttpWebResponse)webRequest.EndGetResponse(result);
            var baseStream = response.GetResponseStream();

            // if you want to read string response
            using (var reader = new StreamReader(baseStream))
            {
                var Result = reader.ReadToEnd();

                //forumPostTiles = JSONParser.ParseForumTitles(Result.ToString());
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    textBox1.Text = Result.ToString();
                });
            }
        }

        private void getModules()
        {            
            String url = "https://ivle.nus.edu.sg/api/Lapi.svc/Modules?APIKey=" + data.APIKey + "&AuthToken=" + data.AuthToken + "&Duration="
                                    + "10" + "&IncludeAllInfo=true" + "&output=json";
            var webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.BeginGetResponse(new AsyncCallback(callback_modules), webRequest);
        }

        private void callback_modules(IAsyncResult result)
        {
            var webRequest = result.AsyncState as HttpWebRequest;
            var response = (HttpWebResponse)webRequest.EndGetResponse(result);
            var baseStream = response.GetResponseStream();

            // if you want to read string response
            using (var reader = new StreamReader(baseStream))
            {
                var Result = reader.ReadToEnd();                

                data.modules = JSONParser.ParseModules(Result.ToString());                
                
                //Deployment.Current.Dispatcher.BeginInvoke(() => { textBox1.Text = Result.ToString(); });
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {                    
                    //getTimetable();
                });

                //Deployment.Current.Dispatcher.BeginInvoke(() => { textBox1.Text = output; });                
                //System.Diagnostics.Debug.WriteLine(Result.ToString());
            }
        }

        private void getTimetable()
        {
            String url = "https://ivle.nus.edu.sg/api/Lapi.svc/Timetable_Module?APIKey=" + data.APIKey + "&AuthToken=" +
                                        data.AuthToken + "&AcadYear=2011/2012" + "&Semester=3" + "&output=json";            
            var webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.BeginGetResponse(new AsyncCallback(callback_time_table), webRequest);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            getTimetable();            
        }
    }
}