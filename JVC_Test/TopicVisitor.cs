using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVC_Test
{
    internal class TopicVisitor
    {
        private const string TOPIC_URL = @"https://www.jeuxvideo.com/forums/0-51-0-1-0-{1}-0-blabla-18-25-ans.htm";

        private List<TopicStats> topicStats = new List<TopicStats>();

        public async Task ParseTopics(int pages, Action<float> onProgress = null)
        {
            topicStats.Clear();
            for (int i = 0; i < pages; i++)
            {
                int count = (1 + i * 25);
                topicStats.AddRange(HTMLVisitor.GetTopics(HTMLVisitor.GetHtml(TOPIC_URL.Replace("{1}", count.ToString()))));
                onProgress?.Invoke((float)i / pages);
            }
        }
        public string GetTopicsInfos()
        {
            return string.Join("\n\n", topicStats.Select(topic => $"{topic.TopicAuthor} : {topic.TopicDate}\n{topic.TopicName}"));
        }
    }
}
