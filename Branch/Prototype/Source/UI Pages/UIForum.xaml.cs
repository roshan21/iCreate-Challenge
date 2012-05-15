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
using WP7.AppRes;

namespace WP7
{
    public partial class UIForum : PhoneApplicationPage
    {
        private string API_Key, AuthToken;
        List<Modules> moduleList;
        List<string> moduleCodes;

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
                Deployment.Current.Dispatcher.BeginInvoke(() => { textBox1.Text = Result.ToString(); });                
                
                var entry = XDocument.Parse(Result.ToString());
                
                var data = from query in entry.Descendants("Data_Module")
                           select new Modules
                           {
                               CourseCode = (string)query.Element("CourseCode"),
                               CourseName = (string)query.Element("CourseName"),
                               ID = (string)query.Element("ID")
                           };
                moduleList = data.ToList();

                for (int i = 0; i < moduleList.Count; i++)
                    moduleCodes.Add(moduleList[i].CourseCode);
                Deployment.Current.Dispatcher.BeginInvoke(() => { listBox1.ItemsSource = moduleCodes; });                
                                
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
                                    + "10" + "&IncludeAllInfo=true";
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