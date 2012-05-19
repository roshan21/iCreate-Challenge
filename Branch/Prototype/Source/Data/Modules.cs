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

//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Json;

namespace InteractIVLE.Data
{
    public class Forum
    {
        public string ForumID { get; set; }
        public string Title { get; set; }        
    }

    //[DataContract]
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

        //[DataMember(Name = "CourseCode")]
        public string CourseCode { get; set; }        
        public string CourseName { get; set; }
        public string ID { get; set; }
        public List<Forum> forums;
    }
}
