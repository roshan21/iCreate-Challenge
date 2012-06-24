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

namespace InteractIVLE.UIPages
{
    public partial class UIMainScreen : PhoneApplicationPage
    {
        public UIMainScreen()
        {
            InitializeComponent();
        }

        private void btn_Announcements_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/UIPages/UIAnnouncements.xaml", UriKind.Relative));
        }

        private void btn_Forum_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/UIPages/UIForum.xaml", UriKind.Relative));
        }

        private void btn_Organizer_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/UIPages/UIOrganizer.xaml", UriKind.Relative));
        }
    }
}