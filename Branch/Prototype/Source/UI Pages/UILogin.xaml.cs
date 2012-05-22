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

namespace InteractIVLE
{
    public partial class UILogin : PhoneApplicationPage
    {
        GlobalCache data = GlobalCache.Instance;

        // Constructor
        public UILogin()
        {
            InitializeComponent();
        }

        private void wb_Login_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            //this event is triggered when the web browser control has successfully navigated to a url
            var url = e.Uri.ToString();
            
            //2. when login is complete, the url will be login_result.ashx?r=0
            if (e.Uri.AbsolutePath == "/api/login/login_result.ashx")
            {
                if (url.IndexOf("&r=0") > 0)
                {
                    //3. When login is successful, there will be a &r=0 in the url. It also means the return data is the token itself.
                    //However as this is a web browser, the reply will be wrapped inside the <body> tag, therefore we use regular expressions to grab the actual token
                    var R = System.Text.RegularExpressions.Regex.Match(wb_Login.SaveToString(), @"body\>(.*?)\<\/body", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (R.Success)
                    {
                        //4. Token is now saved as a local variable, you have to figure how to promote this to a global value (e.g isolated storage)
                        var IVLE_Token = R.Groups[1].Value;

                        //5. Now navigate to the main app
                        //NavigationService.Navigate(new Uri("/Display.xaml?token=" + IVLE_Token.ToString(), UriKind.Relative));
                        data.APIKey = cLAPI.APIKey;
                        data.AuthToken = IVLE_Token.ToString();
                        NavigationService.Navigate(new Uri("/UI Pages/UIMainScreen.xaml", UriKind.Relative));
                    }
                }
                else
                    wb_Login.Navigate(new Uri(cLAPI.LoginURL));
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {                        
            wb_Login.IsEnabled = true;
            wb_Login.Visibility = Visibility.Visible;
            wb_Login.Navigate(new Uri(cLAPI.LoginURL));
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            wb_Login.IsEnabled = true;
            wb_Login.Visibility = Visibility.Visible;
            wb_Login.Navigate(new Uri(cLAPI.LoginURL));
        }
    }
}