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
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Collections;
using Microsoft.Phone.Controls;

using InteractIVLE.Data;
using InteractIVLE.UIPages;

using Amazon;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;


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

            if (jHeadings.Count == 0)
                jHeadings = data.modules[data.curModuleIndex].jPosts["Results"] as JArray; 
            else
                data.modules[data.curModuleIndex].jPosts = json;

            // Multiple headings in current forum
            int i = 0;
            foreach (var jHeading in jHeadings)
            {
                //data.modules[data.curModuleIndex].jPosts[forumIndex].Add(jHeading["Threads"] as JArray);

                int index = 0;
                foreach (var jPost in jHeading["Threads"])
                {
                    ForumPostTitle newPost = new ForumPostTitle();
                    newPost.Heading = jPost["PostTitle"].ToString();
                    newPost.Timestamp = jPost["PostDate_js"].ToString();
                    newPost.Type = jPost["isSurveyPost"].ToString();
                    newPost.Author = jPost["Poster"]["Name"].ToString();
                    newPost.ID = jPost["ID"].ToString();

                    if (jPost["votes"] != null)
                        newPost.Votes = Convert.ToInt32(jPost["votes"].ToString());
                    else
                        newPost.Votes = 0;

                    newPost.threadIndex = index;
                    newPost.Votes = 0;
                    newPost.Answers = 0;
                    newPost.Number = 0;

                    index++;
                    posts.Add(newPost);
                }
                i++;
            }

            return posts;
        }

        public static List<ForumPostTitle> sortTitlesByTime(List<ForumPostTitle> posts)
        {
            var dictionary = new Dictionary<long, ForumPostTitle>();

            foreach (ForumPostTitle fpt in posts)
            {
                long timestamp = strip(fpt.Timestamp);
                dictionary.Add(timestamp, fpt);
            }

            var list = dictionary.Keys.ToList();
            list.Sort();
            list.Reverse(0, list.Count);

            List<ForumPostTitle> sortedPosts = new List<ForumPostTitle>();

            foreach (var key in list)
            {
                sortedPosts.Add(dictionary[key]);
            }

            JToken a = data.modules[data.curModuleIndex].jPosts["Results"][0]["Threads"];
            return sortedPosts;
        }

        private static long strip(String input)
        {
            // 2012-06-25T21:22:33.98
            String result = "";
            int counter = 0;
            String month = "";
            String date = "";
            String year = "";
            String hour = "";
            String minute = "";
            String second = "";

            for (int i = 0; i < input.Count(); i++)
            {
                if ((input[i] >= '0' && input[i] <= '9'))
                {
                    if (counter == 0)
                        month = month + input[i];
                    else if (counter == 1)
                        date = date + input[i];
                    else if (counter == 2)
                        year = year + input[i];
                    else if (counter == 3)
                        hour = hour + input[i];
                    else if (counter == 4)
                        minute = minute + input[i];
                    else if (counter == 5)
                        second = second + input[i];
                }
                else
                    counter++;
            }

            if (month.Count() == 1)
                month = "0" + month;
            if (date.Count() == 1)
                date = "0" + date;
            if (hour.Count() == 1)
                hour = "0" + hour;
            if (minute.Count() == 1)
                minute = "0" + minute;
            if (second.Count() == 1)
                second = "0" + second;

            result = year + month + date + hour + minute + second;
            return Convert.ToInt64(result);
        }

        public static List<ForumPost> ParseForumThreads(int forumIndex, int headingIndex, int threadIndex)
        {
            var jPost = data.modules[data.curModuleIndex].jPosts["Results"][headingIndex]["Threads"][threadIndex];
            List<ForumPost> posts = new List<ForumPost>();

            DFS(posts, jPost);
            //String vote_of_first_post_in_cur_thread = data.modules[data.curModuleIndex].jPosts["Results"][headingIndex]["Threads"][threadIndex]["Votes"].ToString();
            return posts;
        }

        public static List<ForumPostTitle> fetchForumTitles(int moduleIndex, int forumIndex, int headingIndex)
        {
            List<ForumPostTitle> titles = new List<ForumPostTitle>();

            int index = 0;
            foreach (var jPost in data.modules[moduleIndex].jPosts["Results"][headingIndex]["Threads"])
            {
                titles.Add(convert(jPost));
                titles[titles.Count - 1].threadIndex = index;
                index++;
            }

            return titles;
        }

        public static ForumPostTitle convert(JToken jToken)
        {
            ForumPostTitle newPost = new ForumPostTitle();
            newPost.Heading = jToken["PostTitle"].ToString();
            newPost.Timestamp = jToken["PostDate_js"].ToString();
            newPost.Type = jToken["isSurveyPost"].ToString();
            newPost.Author = jToken["Poster"]["Name"].ToString();
            newPost.ID = jToken["ID"].ToString();
            if (jToken["votes"] != null)
                newPost.Votes = Convert.ToInt32(jToken["votes"].ToString());
            else
                newPost.Votes = 0;

            newPost.Answers = 0;
            newPost.Number = 0;

            return newPost;
        }

        public static ForumPost convertP(JToken jToken)
        {
            ForumPost newPost = new ForumPost();
            newPost.Heading = jToken["PostTitle"].ToString();
            newPost.Timestamp = jToken["PostDate_js"].ToString();
            newPost.Type = jToken["isSurveyPost"].ToString();
            newPost.Author = jToken["Poster"]["Name"].ToString();
            newPost.ID = jToken["ID"].ToString();
            if (jToken["votes"] != null)
                newPost.Votes = Convert.ToInt32(jToken["votes"].ToString());
            else
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
            newPost.ID = jPost["ID"].ToString();
            int index_of_prev = newPost.PostContent.IndexOf("-----text of original message-----");
            if( index_of_prev > 0 )
                newPost.PostContent = newPost.PostContent.Substring(0, index_of_prev);

            if (jPost["votes"] != null)
                newPost.Votes = Convert.ToInt32(jPost["votes"].ToString());
            else
                newPost.Votes = 0;
            newPost.Answers = 0;
            newPost.Number = 0;
            posts.Add(newPost);

            //jPost["Votes"] = 10;

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

        // AWS Functions

        #region AWS_SimpleDB

        public static void AmazonBatchPutPostTitle(List<ForumPostTitle> awsPosts)
        {
            SimpleDB.Client.OnSimpleDBResponse += BatchPutAttributeWebResponse;
            BatchPutAttributesRequest request = new BatchPutAttributesRequest() { DomainName = "coreDB" };
            ReplaceableAttribute attributesOne = new ReplaceableAttribute().WithName("votes").WithValue("0").WithReplace(true);
            ReplaceableAttribute attributesTwo = new ReplaceableAttribute().WithName("module").WithValue(data.modules[data.curModuleIndex].CourseCode.ToString());
            ReplaceableAttribute attributesThree = new ReplaceableAttribute().WithName("AWSTimestamp").WithValue(data.modules[data.curModuleIndex].lastUpdated.ToString()).WithReplace(true);
            ReplaceableAttribute attributesFour;
            ReplaceableAttribute attributesFive = new ReplaceableAttribute().WithName("root").WithValue("null");

            int forumIndex = 0;
            int i = 0;

            List<ReplaceableItem> replacableItem = request.Items;
            int t = 0;

            //List<ForumPostTitle> titles = fetchForumTitles(data.curModuleIndex, data.curForumIndex, 0);
            foreach (var post in awsPosts)
            {

                attributesFour = new ReplaceableAttribute().WithName("title").WithValue(post.Heading.ToString());
                //attributesFive = new ReplaceableAttribute().WithName("subscriber").WithValue(data.modules[data.curModuleIndex].lastUpdated.ToString());

                ReplaceableItem repItem = new ReplaceableItem() { ItemName = post.ID.ToString() };
                repItem.Attribute.Add(attributesOne);
                repItem.Attribute.Add(attributesTwo);
                repItem.Attribute.Add(attributesThree);
                repItem.Attribute.Add(attributesFour);
                repItem.Attribute.Add(attributesFive);
                replacableItem.Add(repItem);
                t++;

            }

            SimpleDB.Client.BatchPutAttributes(request);
        }

        public static void AmazonBatchPutPost(List<ForumPost> awsPosts,int threadIndex)
        {
            SimpleDB.Client.OnSimpleDBResponse += BatchPutAttributeWebResponse;
            BatchPutAttributesRequest request = new BatchPutAttributesRequest() { DomainName = "coreDB" };
            ReplaceableAttribute attributesOne = new ReplaceableAttribute().WithName("votes").WithValue("0").WithReplace(true);
            ReplaceableAttribute attributesTwo = new ReplaceableAttribute().WithName("module").WithValue(data.modules[data.curModuleIndex].CourseCode.ToString());
            ReplaceableAttribute attributesThree = new ReplaceableAttribute().WithName("AWSTimestamp").WithValue(data.modules[data.curModuleIndex].lastUpdated.ToString()).WithReplace(true);
            ReplaceableAttribute attributesFour;
            ReplaceableAttribute attributesFive = new ReplaceableAttribute().WithName("root").WithValue(data.modules[data.curModuleIndex].jPosts["Results"][0]["Threads"][threadIndex]["ID"].ToString());

            int forumIndex = 0;
            int i = 0;

            List<ReplaceableItem> replacableItem = request.Items;
            int t = 0;

            //List<ForumPostTitle> titles = fetchForumTitles(data.curModuleIndex, data.curForumIndex, 0);
            foreach (var post in awsPosts)
            {

                attributesFour = new ReplaceableAttribute().WithName("title").WithValue(post.Heading.ToString());
                //attributesFive = new ReplaceableAttribute().WithName("subscriber").WithValue(data.modules[data.curModuleIndex].lastUpdated.ToString());

                ReplaceableItem repItem = new ReplaceableItem() { ItemName = post.ID.ToString() };
                repItem.Attribute.Add(attributesOne);
                repItem.Attribute.Add(attributesTwo);
                repItem.Attribute.Add(attributesThree);
                repItem.Attribute.Add(attributesFour);
                repItem.Attribute.Add(attributesFive);
                replacableItem.Add(repItem);
                t++;

            }

            SimpleDB.Client.BatchPutAttributes(request);
        }

        static void BatchPutAttributeWebResponse(object sender, ResponseEventArgs args)
        {
            ISimpleDBResponse result = args.Response;
            SimpleDB.Client.OnSimpleDBResponse -= BatchPutAttributeWebResponse;

            //this.Dispatcher.BeginInvoke(() =>
            //{
            //    BatchPutAttributesResponse response = result as BatchPutAttributesResponse;

            //    if (null != response)
            //    {
            //        this.BatchPutMessage = "Batch attributes put successfully";
            //    }
            //    else
            //    {
            //        AmazonSimpleDBException exception = result as AmazonSimpleDBException;
            //        if (null != exception)
            //            this.BatchPutMessage = "Error: " + exception.Message;
            //    }
            //});
        }

        static void AmazonGetLatestTimestamp()
        {
            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object senderOriginal, ResponseEventArgs args)
            {
                //Unhook from event.
                SimpleDB.Client.OnSimpleDBResponse -= handler;
                SelectResponse response = args.Response as SelectResponse;

                if (null != response)
                {
                    SelectResult selectResult = response.SelectResult;
                    if (null != selectResult)
                    {
                        foreach (var item in selectResult.Item)
                        {
                            data.modules[data.curModuleIndex].AWSTimestamp = Convert.ToDateTime(item.Attribute[0].Value);
                        }

                    }
                }
            };
            SimpleDB.Client.OnSimpleDBResponse += handler;
            SimpleDB.Client.Select(new SelectRequest { SelectExpression = "SELECT AWSTimestamp FROM coreDB WHERE module = '" + data.modules[data.curModuleIndex].CourseCode.ToString() + "' limit 1", ConsistentRead = true });
        }

        public static void AmazonSyncPostTitleVotes(UIForum uiForum)
        {
            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object senderOriginal, ResponseEventArgs args)
            {
                //Unhook from event.
                SimpleDB.Client.OnSimpleDBResponse -= handler;
                SelectResponse response = args.Response as SelectResponse;
                if (response == null)
                {
                    // 5 times
                    if (data.retries < 5)
                    {
                        AmazonSyncPostTitleVotes(uiForum);
                        data.retries++;
                    }
                }

                if (null != response)
                {                    
                    SelectResult selectResult = response.SelectResult;
                    if (null != selectResult)
                    {

                        //AwsEntry awsEntry(;
                        //data.modules[data.curModuleIndex].awsEntries.Add(

                        data.modules[data.curModuleIndex].awsEntries.Clear();

                        int itemsCount = 0;
                        int attributesCount = 0;
                        foreach (var item in selectResult.Item)
                        {
                            data.modules[data.curModuleIndex].awsEntries.Add(new AwsEntry { ID = item.Name, votes = item.Attribute[4].Value });
                            itemsCount++;
                            attributesCount += item.Attribute.Count;
                        }

                    }

                    //update data.forumPostTitles with votes score, ans bla bla            
                    List<ForumPostTitle> titles = data.forumPostTitles; 
                    //fetchForumTitles(data.curModuleIndex, 0, 0);                    
                    List<ForumPostTitle> awsTitles = new List<ForumPostTitle>();

                    int index = 0;
                    foreach (var title in titles)
                    {
                        bool flag = true;
                        foreach (var entry in data.modules[data.curModuleIndex].awsEntries)
                        {
                            if (title.ID == entry.ID)
                            {
                                flag = false;
                                title.Votes = int.Parse(entry.votes);

                                //title.Votes+=index;

                                // Update JSON DB
                                data.modules[data.curModuleIndex].jPosts["Results"][0]["Threads"][index]["votes"] = title.Votes;
                            }
                        }
                        index++;
                        if (flag)
                        {
                            awsTitles.Add(title);
                        }
                        if (awsTitles.Count >= 25)
                        {
                            AmazonBatchPutPostTitle(awsTitles);
                            awsTitles.Clear();
                        }
                    }

                    if (awsTitles.Count > 0)
                    {
                        AmazonBatchPutPostTitle(awsTitles);
                        awsTitles.Clear();
                    }
                    data.forumPostTitles = titles;
                    uiForum.updateUI();       
                }
            };
            data.retries = 0;
            SimpleDB.Client.OnSimpleDBResponse += handler;
            String query = "SELECT * FROM `coreDB` WHERE module = '" + data.modules[data.curModuleIndex].CourseCode.ToString() + "' and root ='null'";
            SimpleDB.Client.Select(new SelectRequest { SelectExpression = query, ConsistentRead = true });
        }

        public static void AmazonSyncPostVotes(int threadIndex, UIPost uiPost)
        {
            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object senderOriginal, ResponseEventArgs args)
            {
                //Unhook from event.
                SimpleDB.Client.OnSimpleDBResponse -= handler;
                SelectResponse response = args.Response as SelectResponse;

                if (null != response)
                {
                    SelectResult selectResult = response.SelectResult;
                    if (null != selectResult)
                    {

                        //AwsEntry awsEntry(;
                        //data.modules[data.curModuleIndex].awsEntries.Add(

                        data.modules[data.curModuleIndex].awsEntries.Clear();

                        int itemsCount = 0;
                        int attributesCount = 0;
                        foreach (var item in selectResult.Item)
                        {
                            data.modules[data.curModuleIndex].awsEntries.Add(new AwsEntry { ID = item.Name, votes = item.Attribute[4].Value });
                            itemsCount++;
                            attributesCount += item.Attribute.Count;
                        }

                    }

                    //update data.forumPostTitles with votes score, ans bla bla            
                    List<ForumPost> posts = ParseForumThreads(0, 0, threadIndex);
                    List<ForumPost> awsPosts = new List<ForumPost>();

                    int i = 0;
                    foreach (var post in posts)
                    {
                        if (i == 0)
                        {
                            i++;
                            continue;
                        }

                        bool flag = true;
                        foreach (var entry in data.modules[data.curModuleIndex].awsEntries)
                        {
                            if (post.ID == entry.ID)
                            {
                                flag = false;                               
                            }
                        }
                        if (flag)
                        {
                            awsPosts.Add(post);
                        }
                        if (awsPosts.Count >= 25)
                        {
                            AmazonBatchPutPost(awsPosts, threadIndex);
                            awsPosts.Clear();
                        }
                    }
                    
                    if (awsPosts.Count > 0)
                    {
                        AmazonBatchPutPost(awsPosts, threadIndex);
                        awsPosts.Clear();
                    }
                    // ToDO
                    int awsEntryIndex = 0;
                    data.modules[data.curModuleIndex].jPosts["Results"][0]["Threads"][threadIndex] =
                         AWS_to_JSON_DFS(data.modules[data.curModuleIndex].jPosts["Results"][0]["Threads"][threadIndex], data.modules[data.curForumIndex].awsEntries, ref awsEntryIndex, threadIndex);
                    
                    data.forumPosts = ParseForumThreads(0, 0, threadIndex);
                    uiPost.updateUI();

                }
            };
            SimpleDB.Client.OnSimpleDBResponse += handler;
            SimpleDB.Client.Select(new SelectRequest { SelectExpression = "SELECT * FROM coreDB WHERE module = '" + data.modules[data.curModuleIndex].CourseCode.ToString() + "' and root= '" + data.modules[data.curModuleIndex].jPosts["Results"][0]["Threads"][threadIndex]["ID"].ToString()+"'", ConsistentRead = true });
        }

        public static JToken updateDB(JToken jPost, String ID, Action action)
        {
            // update JToken
            if( jPost["ID"].ToString() == ID )
            {
                if (jPost["votes"] == null)
                {
                    if( action == Action.Add )
                        jPost["votes"] = 1;
                    else
                        jPost["votes"] = -1;                    
                }                
                else
                {
                    int value = Convert.ToInt32(jPost["votes"].ToString());
                    if (action == Action.Add)
                        jPost["votes"] = value + 1;
                    else
                        jPost["votes"] = value - 1;
                }

                data.curUpdatedPost = jPost;
                return jPost;
            }
            else
            {
                JArray jChildren = jPost["Threads"] as JArray;

                if (jChildren.Count() > 0)
                {
                    for (int i = 0; i < jChildren.Count; i++)
                    {
                        JToken jReply = jChildren[i];
                        jReply = updateDB(jReply, ID, action);
                        jPost["Threads"][i] = jReply;
                    }
                }

                return jPost;
            }            
        }

        public static JToken AWS_to_JSON_DFS(JToken jPost, List<AwsEntry> awsEntries, ref int awsEntryIndex, int threadIndex)
        {
            // update JToken
            if (awsEntries != null)
            {
                for (int i = 0; i < awsEntries.Count; i++)
                {
                    if (awsEntries[i].ID == jPost["ID"].ToString())                    
                        jPost["votes"] = (awsEntries[i].votes).ToString();                    
                }                
            }

            JArray jChildren = jPost["Threads"] as JArray;
            awsEntryIndex++;

            if (jChildren.Count() > 0)
            {                
                for (int i=0; i < jChildren.Count; i++)
                {
                    JToken jReply = jChildren[i];
                    jReply = AWS_to_JSON_DFS(jReply, awsEntries,ref awsEntryIndex, threadIndex);
                    jPost["Threads"][i] = jReply;
                }
            }

            return jPost;
        }

        public static void AmazonUpdateVotes(string ID, Action action, int votes)
        {
            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;
            handler = delegate(object senderAmazon, ResponseEventArgs args)
            {
                //Unhook from event.
                SimpleDB.Client.OnSimpleDBResponse -= handler;
                PutAttributesResponse response = args.Response as PutAttributesResponse;
                int a = 5;
                int b = a + 1;
            };

            SimpleDB.Client.OnSimpleDBResponse += handler;

            PutAttributesRequest putAttributesRequest = new PutAttributesRequest { DomainName = "coreDB", ItemName = ID };
            List<ReplaceableAttribute> attributesOne = putAttributesRequest.Attribute;

            //Calculate the attributes and their values to put.
            //foreach (var item in GetListAttributeAndValueFromString(this.AttributesAndValuesToPut))
            //    attributesOne.Add(new ReplaceableAttribute().WithName(item.Attribute).WithValue(item.Value))
            
            attributesOne.Add(new ReplaceableAttribute().WithName("votes").WithValue(votes.ToString()).WithReplace(true));
            SimpleDB.Client.PutAttributes(putAttributesRequest);
        }
        
        #endregion

    }
}
