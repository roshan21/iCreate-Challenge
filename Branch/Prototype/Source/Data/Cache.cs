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
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InteractIVLE.Data
{    
    public sealed class GlobalCache
    {
        public List<Module> modules;
        public List<ForumPostTitle> forumPostTitles;
        public List<ForumPost> forumPosts;
        public ForumPosts obsForumPosts;
        public ForumPostTitles obsForumPostTitles;
        public String APIKey, AuthToken;
        public int curModuleIndex;

        public bool moduleCacheLoaded;        
        public List<bool> forumPostCacheLoaded;

        private static readonly GlobalCache instance = new GlobalCache();

        private GlobalCache() 
        {
            moduleCacheLoaded = false;
            forumPostCacheLoaded = new List<bool>(0);

            modules = new List<Module>();
            forumPostTitles = new List<ForumPostTitle>();
            forumPosts = new List<ForumPost>();
            obsForumPosts = new ForumPosts();
            obsForumPostTitles = new ForumPostTitles();
        }

        public static GlobalCache Instance
        {
            get
            {
                return instance;
            }
        }        
    }
}
