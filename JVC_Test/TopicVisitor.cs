using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace JVC_Test
{
    internal class TopicVisitor
    {
        private const string TOPIC_URL = @"https://www.jeuxvideo.com/forums/0-51-0-1-0-{1}-0-blabla-18-25-ans.htm";
        private const string FILTER_PATH = "Resources/filter.json";

        private List<TopicStats> topicStats = new List<TopicStats>();

        public async Task ParseTopics(int pages, Action<float> onProgress = null)
        {
            topicStats.Clear();
            for (int i = 0; i < pages; i++)
            {
                int count = (1 + i * 25);
                topicStats.AddRange(HTMLVisitor.GetTopics(HTMLVisitor.GetHtml(TOPIC_URL.Replace("{1}", count.ToString()))));
                onProgress?.Invoke((float)(i + 1) / pages);
            }
        }
        public string GetTopicsInfos()
        {
            return string.Join("\n\n", topicStats.Select(topic => $"{topic.TopicAuthor} : {topic.TopicDate}\n{topic.TopicName}"));
        }
        public string GetTopWord()
        {
            var lst = String.Join(' ', topicStats.Select(t => t.TopicName)).Split(' ').Where(s => !string.IsNullOrEmpty(s));
            var order = lst.GroupBy(s => s).OrderByDescending(g => g.Count()).ToList();


            return string.Join("\n", order.Select(g => g.Key + " - " + g.Count()));
        }
        public string GetTopWordFilter()
        {
            var json = File.ReadAllText(FILTER_PATH);
            List<string> filters = JsonConvert.DeserializeObject<List<string>>(json);
            var lst = String.Join(' ', topicStats.Select(t => t.TopicName.ToLower())).Split(' ').Where(s => !string.IsNullOrEmpty(s));
            var order = lst.GroupBy(s => s).OrderByDescending(g => g.Count()).Where(g => !filters.Contains(g.Key)).ToList();

            return string.Join("\n", order.Select(g => g.Key + " - " + g.Count()));
        }

        public void ToJson(string path = "")
        {
            string json = JsonConvert.SerializeObject(topicStats, Formatting.Indented);
            if (!string.IsNullOrEmpty(path))
            {
                File.WriteAllText(path, json);
            }
        }
    }
}
