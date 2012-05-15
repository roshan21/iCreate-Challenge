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

namespace WP7.Data
{
    public class Modules
    {
        string courseCode;
        string courseName;
        string id;

        public string CourseCode
        {
            get { return courseCode; }
            set { courseCode = value; }
        }

        public string CourseName
        {
            get { return courseName; }
            set { courseName = value; }
        }

        public string ID
        {
            get { return id; }
            set { id = value; }
        }
    }
}
