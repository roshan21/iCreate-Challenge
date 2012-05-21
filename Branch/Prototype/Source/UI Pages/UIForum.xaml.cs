using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Collections.ObjectModel;

using InteractIVLE.Data;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InteractIVLE
{    
    public partial class UIForum : PhoneApplicationPage
    {
        private string API_Key, AuthToken;
        List<Module> modules;
        List<ForumPostTitle> forumPostTiles;
        ForumPostTitles obsForumPostTitles;
        bool loaded;

        public UIForum()
        {
            InitializeComponent();
            obsForumPostTitles = new ForumPostTitles();
            loaded = false;
        }

        private void getForumHeadings(int moduleIndex, int forumIndex)
        {
            if (modules == null || moduleIndex == -1 || moduleIndex >= modules.Count() || modules[moduleIndex].forums == null)
                return;

            string url = " https://ivle.nus.edu.sg/api/Lapi.svc/Forum_Headings?APIKey=" + API_Key + "&AuthToken=" + AuthToken + "&Duration=0"
                                + "&ForumID=" + modules[moduleIndex].forums[forumIndex].ForumID + "&IncludeThreads=true" + "&output=json";
            var webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.BeginGetResponse(new AsyncCallback(callback_forum_headings), webRequest);
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
                List<Button> btns = new List<Button>();
                String output = "";

                modules = JSONParser.ParseModules(Result.ToString());
                modules.Add(new Module("Dummy1","ABCD","1234"));
                modules.Add(new Module("Dummy2", "ABCD", "1234"));
                modules.Add(new Module("Dummy3", "ABCD", "1234"));

                //Deployment.Current.Dispatcher.BeginInvoke(() => { textBox1.Text = Result.ToString(); });
                Deployment.Current.Dispatcher.BeginInvoke(() => {
                    for (int i = 0; i < modules.Count(); i++)
                    {
                        Button btn = new Button();
                        btn.Height = 60;
                        btn.Width = 130;
                        btn.FontSize = 16;
                        btn.Name = "btn_module" + (i + 1).ToString();
                        btn.Content = modules[i].CourseCode;
                        btn.Margin = new Thickness(3);                                              
                        btns.Add(btn);

                        cLAPI.moduleIDs.Add(modules[i].ID);
                        for (int j = 0; modules[i].forums != null && j < modules[i].forums.Count(); j++ )
                            output = output + modules[i].forums[j].ForumID + " : " + modules[i].forums[j].Title + "\n";
                    }
                    cLAPI.moduleIDsSet = true;
                    listBox1.ItemsSource = btns;                     
                });

                //Deployment.Current.Dispatcher.BeginInvoke(() => { textBox1.Text = output; });                
                //System.Diagnostics.Debug.WriteLine(Result.ToString());
            }
        }

        private void callback_forum_headings(IAsyncResult result)
        {
            var webRequest = result.AsyncState as HttpWebRequest;
            var response = (HttpWebResponse)webRequest.EndGetResponse(result);
            var baseStream = response.GetResponseStream();

            // if you want to read string response
            using (var reader = new StreamReader(baseStream))
            {
                var Result = reader.ReadToEnd();

                forumPostTiles = JSONParser.ParseForumTitles(Result.ToString());                
                Deployment.Current.Dispatcher.BeginInvoke(() => {                    
                    forumPostTiles.ToList().ForEach(obsForumPostTitles.Add);
                    listBox2.ItemsSource = obsForumPostTitles; 
                });                                
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {            
            getModules();            
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

        private void listBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox2.SelectedIndex == -1)
                return;

            NavigationService.Navigate(new Uri("/UI Pages/UIPost.xaml?token=" + AuthToken.ToString() + "&index=" + listBox2.SelectedIndex, UriKind.Relative));

            listBox2.SelectedIndex = -1;
        }

        private void listBox1_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (loaded == false)
            {
                getForumHeadings(0, 0);
                loaded = true;
            }
        }                
    }
}