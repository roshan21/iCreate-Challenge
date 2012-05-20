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
using Microsoft.Phone.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace InteractIVLE
{
    public class Utility
    {
        public static String StripTagsCharArray(String source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new String(array, 0, arrayIndex);
        }

        //private void sendEmail(String body)
        //{            
        //    int chunk = 0, size = 65536, bodySize = body.Count();

        //    if (size < bodySize)
        //        size = bodySize;

        //    while (chunk < body.Count())
        //    {
        //        EmailComposeTask emailcomposer = new EmailComposeTask();
        //        emailcomposer.To = "misidd91@gmail.com";
        //        emailcomposer.Subject = "WP7 Debug data";
        //        emailcomposer.Body = body.Substring(chunk, 48800);
        //        emailcomposer.Show();

        //        if (size < 65536)
        //            break;

        //        chunk += 65536;
        //        if (chunk + size > bodySize)
        //            size = bodySize - chunk;
        //    }
        //}
    }
}
