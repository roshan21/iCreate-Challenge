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

            JSONParser.AmazonSyncPostVotes(threadIndex, this);

            data.obsUiPostTitle.Clear();
            data.uiPostTitle.Clear();
            data.uiPostTitle.Add(data.forumPosts[0]);
            data.uiPostTitle.ForEach(data.obsUiPostTitle.Add);
            listBox_title.ItemsSource = data.obsUiPostTitle;
        }

        public void updateUI()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                data.obsForumPosts.Clear();
                data.forumPosts = JSONParser.ParseForumThreads(0, 0, threadIndex);
                data.forumPosts.ToList().ForEach(data.obsForumPosts.Add);

                listBox1.ItemsSource = data.obsForumPosts;
            });
        }

        private void localUpdateUI()
        {
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
            JToken jPost = data.modules[data.curModuleIndex].jPosts["Results"][0]["Threads"][threadIndex];             
            data.modules[data.curModuleIndex].jPosts["Results"][0]["Threads"][threadIndex] = 
                JSONParser.updateDB(jPost,curPost.ID, Action.Add);
            curPost = JSONParser.convertP(data.curUpdatedPost);
            JSONParser.AmazonUpdateVotes((curPost.ID), Action.Add, curPost.Votes);

            localUpdateUI();
        }

        private void btn_Downvote_Click(object sender, RoutedEventArgs e)
        {
            Button btn_sender = (Button)sender;
            StackPanel stp = (StackPanel)((StackPanel)(btn_sender.Parent)).Parent;
            ForumPost curPost = (ForumPost)stp.DataContext;

            // Update JSON DB
            JToken jPost = data.modules[data.curModuleIndex].jPosts["Results"][0]["Threads"][threadIndex];
            data.modules[data.curModuleIndex].jPosts["Results"][0]["Threads"][threadIndex] =
                JSONParser.updateDB(jPost, curPost.ID, Action.Subtract);
            curPost = JSONParser.convertP(data.curUpdatedPost);
            JSONParser.AmazonUpdateVotes((curPost.ID), Action.Add, curPost.Votes);

            localUpdateUI();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/UIPages/UIReply.xaml", UriKind.Relative));
        }
    }
}