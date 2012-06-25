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

namespace InteractIVLE.UIPages
{
    public partial class UIPost : PhoneApplicationPage
    {
        GlobalCache data = GlobalCache.Instance;
        int threadIndex;

        public UIPost()
        {
            InitializeComponent();            
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (this.NavigationContext.QueryString.ContainsKey("token"))
            {                
                this.threadIndex = Convert.ToInt32(this.NavigationContext.QueryString["index"]);
            }
            
            base.OnNavigatedTo(e);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            data.forumPosts.Clear();
            data.obsForumPosts.Clear();
            data.forumPosts = JSONParser.ParseForumThreads(0, 0, threadIndex);
            data.forumPosts.ToList().ForEach(data.obsForumPosts.Add);
            listBox1.ItemsSource = data.obsForumPosts; 
        }

        private void btn_Upvote_Click(object sender, RoutedEventArgs e)
        {
            Button btn_sender = (Button)sender;
            StackPanel stp = (StackPanel)((StackPanel)(btn_sender.Parent)).Parent;
            ForumPost curPost = (ForumPost)stp.DataContext;

            // Update JSON DB
        }

        private void btn_Downvote_Click(object sender, RoutedEventArgs e)
        {
            Button btn_sender = (Button)sender;
            StackPanel stp = (StackPanel)((StackPanel)(btn_sender.Parent)).Parent;
            ForumPost curPost = (ForumPost)stp.DataContext;

            // Update JSON DB
        }
    }
}