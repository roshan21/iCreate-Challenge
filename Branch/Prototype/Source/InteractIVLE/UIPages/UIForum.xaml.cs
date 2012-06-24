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
using System.Windows.Controls.Primitives;
using System.Collections;

namespace InteractIVLE.UIPages
{
    public partial class UIForum : PhoneApplicationPage
    {
        GlobalCache data;
        
        private ScrollBar sb = null;
        private ScrollViewer sv = null;
        private bool _isBouncy = false;
        private bool alreadyHookedScrollEvents = false;

        public UIForum()
        {
            data = GlobalCache.Instance;
            InitializeComponent();
        }

        private void getForumHeadings(int moduleIndex, int forumIndex, int Duration)
        {
            //if (data.modules == null || moduleIndex == -1 || moduleIndex >= data.modules.Count() || data.modules[moduleIndex].forums == null
            //    || data.modules[moduleIndex].forums.Count() == 0)
            //{
            //    data.forumPostTitles.Clear();
            //    data.obsForumPostTitles.Clear();
            //    listBox2.ItemsSource = data.obsForumPostTitles;
            //    data.isForumLoaded[data.curModuleIndex] = true;
            //    for (int i = 0; data.btn_modules != null && i < data.btn_modules.Count(); i++)
            //        data.btn_modules[i].IsEnabled = true;  

            //    return;
            //}

            string url = " https://ivle.nus.edu.sg/api/Lapi.svc/Forum_Headings?APIKey=" + data.APIKey + "&AuthToken=" + data.AuthToken + "&Duration="
                                + Duration.ToString() + "&ForumID=" + data.modules[moduleIndex].forums[forumIndex].ForumID + "&IncludeThreads=true" + "&output=json";
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
                data.isForumLoaded = new List<bool>();

                //Deployment.Current.Dispatcher.BeginInvoke(() => { textBox1.Text = Result.ToString(); });
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
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
                        btn.IsEnabled = false;
                        data.btn_modules.Add(btn);
                       
                        data.isForumLoaded.Add(false);

                        for (int j = 0; data.modules[i].forums != null && j < data.modules[i].forums.Count(); j++)
                            output = output + data.modules[i].forums[j].ForumID + " : " + data.modules[i].forums[j].Title + "\n";
                    }
                    data.moduleCacheLoaded = true;
                    listBox1.ItemsSource = data.btn_modules;

                    if (data.isForumLoaded[0] == false)
                    {
                        data.curModuleIndex = 0;
                        getForumHeadings(data.curModuleIndex, 0, 0);
                        data.isForumLoaded[data.curModuleIndex] = true;
                        data.modules[data.curModuleIndex].lastUpdated = DateTime.Now;
                    }
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

            if (data.isForumLoaded[myIndex] == false)
            {
                for (int i = 0; data.btn_modules != null && i < data.btn_modules.Count(); i++)
                    data.btn_modules[i].IsEnabled = false;
                int diff = 0;                
                getForumHeadings(myIndex, 0, diff);
                data.modules[myIndex].lastUpdated = DateTime.Now;
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

                        data.isForumLoaded[data.curModuleIndex] = true;
                        for (int i = 0; data.btn_modules != null && i < data.btn_modules.Count(); i++)
                            data.btn_modules[i].IsEnabled = true;

                        customizeListBox();
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
                data.btn_modules = new List<Button>();
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
                }
                listBox1.ItemsSource = data.btn_modules;
                listBox2.ItemsSource = data.obsForumPostTitles;
            }
        }

        private void listBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox2.SelectedIndex == -1)
                return;

            NavigationService.Navigate(new Uri("/UIPages/UIPost.xaml?token=" + data.AuthToken.ToString() + "&index=" + listBox2.SelectedIndex, UriKind.Relative));

            listBox2.SelectedIndex = -1;
        }

        // For "Refresh" feature

        private void customizeListBox()
        {
            if (alreadyHookedScrollEvents)
                return;

            alreadyHookedScrollEvents = true;
            listBox2.AddHandler(ListBox.ManipulationCompletedEvent, (EventHandler<ManipulationCompletedEventArgs>)listBox2_ManipulationCompleted, true);
            sb = (ScrollBar)FindElementRecursive(listBox2, typeof(ScrollBar));
            sv = (ScrollViewer)FindElementRecursive(listBox2, typeof(ScrollViewer));

            if (sv != null)
            {
                // Visual States are always on the first child of the control template 
                FrameworkElement element = VisualTreeHelper.GetChild(sv, 0) as FrameworkElement;
                if (element != null)
                {
                    VisualStateGroup group = FindVisualState(element, "ScrollStates");
                    if (group != null)
                    {
                        group.CurrentStateChanging += new EventHandler<VisualStateChangedEventArgs>(group_CurrentStateChanging);
                    }

                    VisualStateGroup vgroup = FindVisualState(element, "VerticalCompression");                    
                    if (vgroup != null)
                    {
                        vgroup.CurrentStateChanging += new EventHandler<VisualStateChangedEventArgs>(vgroup_CurrentStateChanging);
                    }        
                }
            }
        }        

        private void listBox2_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (_isBouncy)
            {
                ReleaseBounce();
            }
        }        

        private void vgroup_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState.Name == "CompressionTop")
            {
                ProcessBounce();                
                int myIndex = data.curModuleIndex;
                for (int i = 0; data.btn_modules != null && i < data.btn_modules.Count(); i++)
                    data.btn_modules[i].IsEnabled = false;
                int diff = 0;
                if (data.isForumLoaded[myIndex])
                {
                    diff = (int)(DateTime.Now - data.modules[myIndex].lastUpdated).TotalMinutes;
                    diff = diff + 1;
                }
                getForumHeadings(myIndex, 0, diff);
                data.modules[myIndex].lastUpdated = DateTime.Now;
            }

            if (e.NewState.Name == "CompressionBottom")
            {
                ProcessBounce();                
            }

            if (e.NewState.Name == "NoVerticalCompression")
            {
                ReleaseBounce();                
            }
        }

        private void group_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState.Name == "Scrolling")
            {                                
            }
            else
            {                
            }
        }

        private void ReleaseBounce()
        {
            _isBouncy = false;            
        }

        private void ProcessBounce()
        {
            _isBouncy = true;            
        }

        private UIElement FindElementRecursive(FrameworkElement parent, Type targetType)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            UIElement returnElement = null;
            if (childCount > 0)
            {
                for (int i = 0; i < childCount; i++)
                {
                    Object element = VisualTreeHelper.GetChild(parent, i);
                    if (element.GetType() == targetType)
                    {
                        return element as UIElement;
                    }
                    else
                    {
                        returnElement = FindElementRecursive(VisualTreeHelper.GetChild(parent, i) as FrameworkElement, targetType);
                    }
                }
            }
            return returnElement;
        }

        private VisualStateGroup FindVisualState(FrameworkElement element, string name)
        {
            if (element == null)
                return null;

            IList groups = VisualStateManager.GetVisualStateGroups(element);
            foreach (VisualStateGroup group in groups)
                if (group.Name == name)
                    return group;

            return null;
        }
    }
}
