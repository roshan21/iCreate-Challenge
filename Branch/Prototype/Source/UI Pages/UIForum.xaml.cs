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
        GlobalCache data = GlobalCache.Instance;        
        List<bool> isForumLoaded;

        public UIForum()
        {
            InitializeComponent();                               
        }

        private void getForumHeadings(int moduleIndex, int forumIndex)
        {
            if (data.modules == null || moduleIndex == -1 || moduleIndex >= data.modules.Count() || data.modules[moduleIndex].forums == null
                || data.modules[moduleIndex].forums.Count() == 0)
                return;

            string url = " https://ivle.nus.edu.sg/api/Lapi.svc/Forum_Headings?APIKey=" + data.APIKey + "&AuthToken=" + data.AuthToken + "&Duration=0"
                                + "&ForumID=" + data.modules[moduleIndex].forums[forumIndex].ForumID + "&IncludeThreads=true" + "&output=json";
            var webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.BeginGetResponse(new AsyncCallback(callback_forum_headings), webRequest);
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
                String output = "";

                data.modules = JSONParser.ParseModules(Result.ToString());
                isForumLoaded = new List<bool>();

                //Deployment.Current.Dispatcher.BeginInvoke(() => { textBox1.Text = Result.ToString(); });
                Deployment.Current.Dispatcher.BeginInvoke(() => {
                    for (int i = 0; i < data.modules.Count(); i++)
                    {
                        Button btn = new Button();
                        btn.Height = 60;
                        btn.Width = 130;
                        btn.FontSize = 16;
                        btn.Name = "btn_module" + (i + 1).ToString();
                        btn.Content = data.modules[i].CourseCode;
                        btn.Margin = new Thickness(3);
                        btn.Click += new RoutedEventHandler(btn_modules_Click);                      
                        data.btn_modules.Add(btn);

                        data.modules[i].jPosts = new List<List<JArray>>();
                        data.modules[i].jPosts.Add(new List<JArray>());
                        isForumLoaded.Add(false);

                        for (int j = 0; data.modules[i].forums != null && j < data.modules[i].forums.Count(); j++)
                            output = output + data.modules[i].forums[j].ForumID + " : " + data.modules[i].forums[j].Title + "\n";
                    }
                    data.moduleCacheLoaded = true;
                    listBox1.ItemsSource = data.btn_modules;                     
                });

                //Deployment.Current.Dispatcher.BeginInvoke(() => { textBox1.Text = output; });                
                //System.Diagnostics.Debug.WriteLine(Result.ToString());
            }
        }

        private void btn_modules_Click(object sender, RoutedEventArgs e)
        {
            Button myBtn = sender as Button;
            String btnIndex = myBtn.Name.ToString().Substring(10);
            int myIndex = Convert.ToInt32(btnIndex) - 1;
            data.curModuleIndex = myIndex;

            if (isForumLoaded[myIndex] == false)
            {
                for (int i = 0; data.btn_modules != null && i < data.btn_modules.Count(); i++)
                    data.btn_modules[i].IsEnabled = false;
                getForumHeadings(myIndex, 0);                
            }
            else
            {
                data.forumPostTitles.Clear();
                data.obsForumPostTitles.Clear();
                data.forumPostTitles = JSONParser.fetchForumTitles(myIndex, 0, 0);
                data.forumPostTitles.ToList().ForEach(data.obsForumPostTitles.Add);
                listBox2.ItemsSource = data.obsForumPostTitles;
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

                if (data.modules[data.curModuleIndex].forums.Count() == 1)
                {                    
                    data.forumPostTitles = JSONParser.ParseForumTitles(Result.ToString(), 0);
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {                        
                        data.obsForumPostTitles.Clear();
                        data.forumPostTitles.ToList().ForEach(data.obsForumPostTitles.Add);
                        listBox2.ItemsSource = data.obsForumPostTitles;

                        for (int i = 0; data.btn_modules != null && i < data.btn_modules.Count(); i++)
                            data.btn_modules[i].IsEnabled = true;
                        isForumLoaded[data.curModuleIndex] = true;
                    });
                }
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!data.moduleCacheLoaded)
            {
                for (int i = 0; data.btn_modules != null && i < data.btn_modules.Count(); i++)
                    data.btn_modules[i].IsEnabled = false;
                getModules();
            }
            else
            {
                listBox1.ItemsSource = data.btn_modules;
                listBox2.ItemsSource = data.obsForumPostTitles;
            }
        }        

        private void listBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox2.SelectedIndex == -1)
                return;

            NavigationService.Navigate(new Uri("/UI Pages/UIPost.xaml?token=" + data.AuthToken.ToString() + "&index=" + listBox2.SelectedIndex, UriKind.Relative));

            listBox2.SelectedIndex = -1;
        }

        private void listBox1_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (isForumLoaded[4] == false)
            {
                data.curModuleIndex = 4;
                getForumHeadings(4, 0);
                isForumLoaded[4] = true;                
            }
        }                
    }
}