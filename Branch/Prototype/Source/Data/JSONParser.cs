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

using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WP7.Data
{
    public class JSONParser
    {
        public static object Deserialize(Stream streamObject, Type serializedObjectType)
        {
            if (serializedObjectType == null || streamObject == null)
                return null;

            DataContractJsonSerializer ser = new DataContractJsonSerializer(serializedObjectType);
            return ser.ReadObject(streamObject);
        }

        public static List<Module> Parse(String input)
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
                modules.Add(newModule);
            }

            return modules;
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
    }
}
