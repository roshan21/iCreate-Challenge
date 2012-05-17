using System;
using System.IO;
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
using System.Xml;
using System.Xml.Linq;
using WP7.Data;
using System.Text;

namespace WP7
{
    public partial class UIForum : PhoneApplicationPage
    {
        private string API_Key, AuthToken;
        List<Module> modules;        

        public UIForum()
        {
            InitializeComponent();             
        }

        private void request_CallBack(IAsyncResult result)
        {
            var webRequest = result.AsyncState as HttpWebRequest;
            var response = (HttpWebResponse)webRequest.EndGetResponse(result);
            var baseStream = response.GetResponseStream();            

            // if you want to read string response
            using (var reader = new StreamReader(baseStream))
            {                
                var Result = reader.ReadToEnd();
                List<Button> btns = new List<Button>();

                modules = JSONParser.Parse(Result.ToString());
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
                    }
                    listBox1.ItemsSource = btns;                     
                });                                                
                System.Diagnostics.Debug.WriteLine(Result.ToString());
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {            
            getModules();          
        }

        private void getModules()
        {
            String url = "https://ivle.nus.edu.sg/api/Lapi.svc/Modules?APIKey=" + API_Key + "&AuthToken=" + AuthToken + "&Duration=" 
                                    + "10" + "&IncludeAllInfo=true" + "&output=json";
            var webRequest = (HttpWebRequest)HttpWebRequest.Create(url);            
            webRequest.BeginGetResponse(new AsyncCallback(request_CallBack), webRequest);            
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
    }
}