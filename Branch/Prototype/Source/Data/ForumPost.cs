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
using System.Collections.ObjectModel;

namespace InteractIVLE.Data
{
    public class ForumPost
    {
        public String Heading { get; set; }
        public String Timestamp { get; set; }
        public String Author { get; set; }
        public String Type { get; set; }
        public int Votes { get; set; }
        public int Answers { get; set; }
        public int Number { get; set; }

        public ForumPost()
        {
        }

        public ForumPost(String heading, String timestamp, String author, String type, int votes, int ans, int num)
        {
            this.Heading = heading;
            this.Timestamp = timestamp;
            this.Author = author;
            this.Type = type;
            this.Votes = votes;
            this.Answers = ans;
            this.Number = num;
        }
    }

    public class ForumPosts : ObservableCollection<ForumPost>
    {
        public ForumPosts()
        {            
            //Add(new ForumPost("Heading", "Timestamp","Author", "Type", 0, 0, 1));
        }
    }
}
