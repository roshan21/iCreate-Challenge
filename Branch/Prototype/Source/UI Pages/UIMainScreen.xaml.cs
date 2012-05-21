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

namespace InteractIVLE
{
    public partial class UIMainScreen : PhoneApplicationPage
    {
        private string API_Token;

        public UIMainScreen()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (this.NavigationContext.QueryString.ContainsKey("token"))
            {
                this.API_Token = this.NavigationContext.QueryString["token"];
            }
            //this.API_Key = cLAPI.APIKey;

            base.OnNavigatedTo(e);
        }

        private void btn_Announcements_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/UI Pages/UIAnnouncements.xaml?token=" + API_Token.ToString(), UriKind.Relative));
        }

        private void btn_Forum_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/UI Pages/UIForum.xaml?token=" + API_Token.ToString(), UriKind.Relative));
        }

        private void btn_Organizer_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/UI Pages/UIOrganizer.xaml?token=" + API_Token.ToString(), UriKind.Relative));
        }
    }
}