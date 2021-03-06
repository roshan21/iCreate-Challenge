﻿using System;
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
        static GlobalCache data = GlobalCache.Instance;

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
                    newModule.forums = new List<ForumId>() { MapForumID().Invoke(jForumIDs) };
                }
                modules.Add(newModule);
            }

            return modules;
        }

        public static List<ForumPostTitle> ParseForumTitles(String input, int forumIndex)
        {
            JObject json = JObject.Parse(input);

            List<ForumPostTitle> posts = new List<ForumPostTitle>();
            JArray jHeadings = json["Results"] as JArray;
            int i = 0;

            // Multiple headings in current forum
            foreach (var jHeading in jHeadings)
            {
                data.modules[data.curModuleIndex].jPosts[forumIndex].Add(jHeading["Threads"] as JArray);

                foreach (var jPost in data.modules[data.curModuleIndex].jPosts[forumIndex][i])
                {
                    ForumPostTitle newPost = new ForumPostTitle();
                    newPost.Heading = jPost["PostTitle"].ToString();
                    newPost.Timestamp = jPost["PostDate_js"].ToString();
                    newPost.Type = jPost["isSurveyPost"].ToString();
                    newPost.Author = jPost["Poster"]["Name"].ToString();
                    newPost.Votes = 0;
                    newPost.Answers = 0;
                    newPost.Number = 0;                    
                                        
                    posts.Add(newPost);
                }
                i++;
            }

            return posts;
        }

        public static List<ForumPost> ParseForumThreads(int forumIndex, int headingIndex, int threadIndex)
        {
            var jPost = data.modules[data.curModuleIndex].jPosts[forumIndex][headingIndex][threadIndex];
            List<ForumPost> posts = new List<ForumPost>();

            DFS(posts, jPost);
            return posts;
        }

        public static List<ForumPostTitle> fetchForumTitles(int moduleIndex, int forumIndex, int headingIndex)
        {
            int count = data.modules[moduleIndex].jPosts[forumIndex][headingIndex].Count();
            List<ForumPostTitle> titles = new List<ForumPostTitle>();
            for (int i = 0; i < count; i++)            
                titles.Add(convert(data.modules[moduleIndex].jPosts[forumIndex][headingIndex][i]));
            return titles;
        }

        private static ForumPostTitle convert(JToken jToken)
        {
            ForumPostTitle newPost = new ForumPostTitle();
            newPost.Heading = jToken["PostTitle"].ToString();
            newPost.Timestamp = jToken["PostDate_js"].ToString();
            newPost.Type = jToken["isSurveyPost"].ToString();
            newPost.Author = jToken["Poster"]["Name"].ToString();
            newPost.Votes = 0;
            newPost.Answers = 0;
            newPost.Number = 0;

            return newPost;
        }

        private static void DFS(List<ForumPost> posts, JToken jPost)
        {
            ForumPost newPost = new ForumPost();
            newPost.Heading = jPost["PostTitle"].ToString();
            newPost.Timestamp = jPost["PostDate_js"].ToString();
            newPost.Type = jPost["isSurveyPost"].ToString();
            newPost.Author = jPost["Poster"]["Name"].ToString();
            
            newPost.PostContent = jPost["PostBody"].ToString();
            newPost.PostContent = Utility.StripTagsCharArray(newPost.PostContent);
            int index_of_prev = newPost.PostContent.IndexOf("-----text of original message-----");
            if( index_of_prev > 0 )
                newPost.PostContent = newPost.PostContent.Substring(0, index_of_prev);

            newPost.Votes = 0;
            newPost.Answers = 0;
            newPost.Number = 0;
            posts.Add(newPost);

            JArray jChildren = jPost["Threads"] as JArray;

            if (jChildren.Count() > 0)
            {
                foreach (var jThread in jChildren)
                    DFS(posts, jThread);
            }
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

        private static Func<JToken, ForumId> MapForumID()
        {
            return json => new ForumId
            {
                ForumID = (string)json["ID"],
                Title = (string)json["Title"]                
            };
        }
    }
}
