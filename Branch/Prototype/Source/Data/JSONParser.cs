using System;
using System.IO;
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
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InteractIVLE.Data
{
    public class JSONParser
    {
        //public static object Deserialize(Stream streamObject, Type serializedObjectType)
        //{
        //    if (serializedObjectType == null || streamObject == null)
        //        return null;

        //    DataContractJsonSerializer ser = new DataContractJsonSerializer(serializedObjectType);
        //    return ser.ReadObject(streamObject);
        //}

        public static List<Module> ParseModules(String input)
        {
            JObject json = JObject.Parse(input);

            List<Module> modules = new List<Module>();
            JArray jModules = json["Results"] as JArray;

            foreach (var jModule in jModules)
            {
                Module newModule = new Module();
                newModule.CourseCode = jModule["CourseCode"].ToString();
                newModule.CourseName = jModule["CourseName"].ToString();
                newModule.ID = jModule["ID"].ToString();

                var jForumIDs = jModule["Forums"];
                if (jForumIDs is JArray)
                {
                    newModule.forums = jForumIDs.Children().Select(MapForumID()).ToList();
                }
                else
                {
                    newModule.forums = new List<Forum>() { MapForumID().Invoke(jForumIDs) };
                }
                modules.Add(newModule);
            }

            return modules;
        }

        public static List<ForumPost> ParseForumThreads(String input)
        {
            JObject json = JObject.Parse(input);

            List<ForumPost> posts = new List<ForumPost>();
            JArray jHeadings = json["Results"] as JArray;

            // Multiple headings in current forum
            foreach (var jHeading in jHeadings)
            {
                JArray jPosts = jHeading["Threads"] as JArray;

                foreach (var jPost in jPosts)
                {
                    ForumPost newPost = new ForumPost();
                    newPost.Heading = jPost["PostTitle"].ToString();
                    newPost.Timestamp = jPost["PostDate_js"].ToString();
                    newPost.Type = jPost["isSurveyPost"].ToString();
                    newPost.Author = jPost["Poster"]["Name"].ToString();
                    newPost.Votes = 0;
                    newPost.Answers = 0;
                    newPost.Number = 0;                    
                                        
                    posts.Add(newPost);
                }
            }

            return posts;
        }

        // Sample Functor to extract sub-lists from a json object
        // Sample usage:
        //if (attrToken is JArray)
        //{
        //    newGroup.Items = attrToken.Children().Select(MapDetailItem()).ToList();
        //}
        //else
        //{
        //    newGroup.Items = new List<DetailItem>() { MapDetailItem().Invoke(attrToken) };
        //}
        private static Func<JToken, Module> MapDetailItem()
        {
            return json => new Module
            {
                CourseCode = (string)json["CourseCode"],
                CourseName = (string)json["CourseName"],
                ID = (string)json["ID"]
            };
        }

        private static Func<JToken, Forum> MapForumID()
        {
            return json => new Forum
            {
                ForumID = (string)json["ID"],
                Title = (string)json["Title"]                
            };
        }
    }
}
