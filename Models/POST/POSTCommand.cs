using System;
using System.Collections.Generic;
using System.Text;

namespace StopHandler.Models.POST
{
    abstract class POSTCommand
    {

        public static IPOSTCommand Parse(string post)
        {
            List<string> identifiers = new List<string>();
            List<string[]> tags = new List<string[]>();

            //STOP
            tags.Add(StopCommand.tags);
            identifiers.Add(StopCommand.identifier);

            //STOP
            tags.Add(StartCommand.tags);
            identifiers.Add(StartCommand.identifier);



            string[] values = { };
            for (int com = 0; com < tags.Count; com++)
            {
                try
                {
                    string id = FindTag(post, tags[com][0]);

                    if (identifiers[com] == id)
                    {
                        values = new string[tags[com].Length];
                        values[0] = id;

                        for (int i = 1; i < tags[com].Length; i++)
                        {
                            values[i] = FindTag(post, tags[com][i]);
                        }
                        break;
                    }
                }
                catch (Exception)
                {
                    break;
                }
            }

            if (values == null || values.Length == 0) return null;
            for (int i = 0; i < values.Length - 1; i++)
            {
                if (values[i] == null || values[i] == "") return null;
            }
            if (values[values.Length - 1] == "") values[values.Length - 1] = "#DBG";

            if (values[0] == StopCommand.identifier) return StopCommand.Instantiate(values); //STOP
            if (values[0] == StartCommand.identifier) return StartCommand.Instantiate(values); //START
            else return null;

        }
        private static string FindTag(string post, string tag)
        {
            int openTagIndex = post.IndexOf(tag + '#');
            int closeTagIndex = post.IndexOf('#' + tag);

            if (openTagIndex == -1 || closeTagIndex == -1) return null;

            int startIndex = openTagIndex + tag.Length + 1;
            return post.Substring(startIndex, closeTagIndex - startIndex);
        }
        public static DateTime UnixTimeToLocalTime(long unix)
        {
            var delta = DateTime.Now.Subtract(DateTime.UtcNow);
            return DateTime.UnixEpoch.AddSeconds(unix).Add(delta);
        }
    }
}
