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

namespace InteractIVLE.UI_Pages
{
    public partial class UIOrganizer : PhoneApplicationPage
    {
        private string API_Key, AuthToken;
        List<Module> modules;

        public UIOrganizer()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (this.NavigationContext.QueryString.ContainsKey("token"))
            {
                this.AuthToken = this.NavigationContext.QueryString["token"];
            }
            this.API_Key = cLAPI.APIKey;

            base.OnNavigatedTo(e);
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
            if (cLAPI.moduleIDsSet == false)
            {
            }

            String url = "https://ivle.nus.edu.sg/api/Lapi.svc/Modules?APIKey=" + API_Key + "&AuthToken=" + AuthToken + "&Duration="
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

                modules = JSONParser.ParseModules(Result.ToString());
                modules.Add(new Module("Dummy1", "ABCD", "1234"));
                modules.Add(new Module("Dummy2", "ABCD", "1234"));
                modules.Add(new Module("Dummy3", "ABCD", "1234"));

                cLAPI.moduleIDs = new List<string>();
                //Deployment.Current.Dispatcher.BeginInvoke(() => { textBox1.Text = Result.ToString(); });
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    for (int i = 0; i < modules.Count(); i++)                                           
                        cLAPI.moduleIDs.Add(modules[i].ID);                                            
                    cLAPI.moduleIDsSet = true;
                    //getTimetable();
                });

                //Deployment.Current.Dispatcher.BeginInvoke(() => { textBox1.Text = output; });                
                //System.Diagnostics.Debug.WriteLine(Result.ToString());
            }
        }

        private void getTimetable()
        {
            String url = "https://ivle.nus.edu.sg/api/Lapi.svc/Timetable_Module?APIKey=" + API_Key + "&AuthToken=" +
                                        AuthToken + "&AcadYear=2011/2012" + "&Semester=3" + "&output=json";// +"&CourseID=" + cLAPI.moduleIDs[0];
            var webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.BeginGetResponse(new AsyncCallback(callback_time_table), webRequest);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            getTimetable();            
        }
    }
}