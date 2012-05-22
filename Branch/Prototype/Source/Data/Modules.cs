using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InteractIVLE.Data
{        
    public class Module
    {
        public Module()
        {
        }

        public Module(string a, string b, string c)
        {
            CourseCode = a;
            CourseName = b;
            ID = c;
        }
        
        public string CourseCode { get; set; }        
        public string CourseName { get; set; }
        public string ID { get; set; }
        public List<ForumId> forums;
        public List<List<JArray>> jPosts;
    }

    public class ForumId
    {
        public string ForumID { get; set; }
        public string Title { get; set; }
    }
}
