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

namespace InteractIVLE
{
    public partial class UIPost : PhoneApplicationPage
    {
        private string AuthToken;
        //List<Module> modules;
        List<ForumPost> forumPosts;
        ForumPosts obsForumPosts;
        int index;

        public UIPost()
        {
            InitializeComponent();
            obsForumPosts = new ForumPosts();
        }             

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (this.NavigationContext.QueryString.ContainsKey("token"))
            {
                this.AuthToken = this.NavigationContext.QueryString["token"];
                this.index = Convert.ToInt32( this.NavigationContext.QueryString["index"] );
            }            

            base.OnNavigatedTo(e);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            forumPosts = JSONParser.ParseForumThreads(index);
            forumPosts.ToList().ForEach(obsForumPosts.Add);
            listBox1.ItemsSource = obsForumPosts; 
        }
    }
}