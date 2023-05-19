using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace JVC_Test
{
    internal class HTMLVisitor
    {
        private const string FILTER_PATH = "Resources/http_proxies.txt";

        private static WebProxy GetProxy()
        {
            var lst = File.ReadAllLines(FILTER_PATH).ToList();

            var ip = lst[new Random().Next(0, lst.Count)];
            WebProxy proxy = new WebProxy();
            proxy.Address = new Uri("http://" + ip);

            return proxy;
        }

        public static string GetHtml(string url)
        {
            var httpClientHandler = new HttpClientHandler
            {
                Proxy = GetProxy(),
            };

            var client = new HttpClient();
            var content = client.GetStringAsync(url).Result;
            return content;
        }
        public static List<TopicStats> GetTopics(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var lst = new List<TopicStats>();
            var topicList = FindFirstClass(doc.DocumentNode, "topic-list");

            foreach (HtmlNode? node in topicList?.ChildNodes)
            {
                if (int.TryParse(node.Attributes["data-id"]?.Value, out int t))
                {
                    TopicStats topic = new TopicStats();
                    var topicTitle = FindFirstClass(FindFirstClass(node, "topic-subject"), "topic-title");
                    topic.TopicName = Clean(topicTitle?.Attributes["title"]?.Value);
                    topic.TopicAuthor = Clean(FindFirstClass(node, "topic-author")?.InnerHtml);
                    string topicDate = Clean(FindFirstClass(FindFirstClass(node, "topic-date"), "lien-jv")?.InnerHtml);
                    topic.TopicDate = DateTime.Parse(topicDate).ToString("dd/MM/yyyy HH:mm:ss");
                    topic.TopicUrl = Clean(topicTitle?.Attributes["href"].Value);

                    lst.Add(topic);
                }
            }
            return lst;
        }
        public static string Clean(string value)
        {
            if (value == null)
            {
                return value;
            }
            return HttpUtility.HtmlDecode(value.Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim());
        }

        private static List<HtmlNode> FindAttributes(HtmlNode node, string attribute, string value)
        {
            var lst = new List<HtmlNode>();
            if (node == null)
            {
                return lst;
            }

            if (node.Attributes[attribute]?.Value.Contains(value) == true)
            {
                lst.Add(node);
            }

            foreach (HtmlNode child in node.ChildNodes)
            {
                HtmlNode result = FindFirstAttribute(child, attribute, value);
                if (result != null)
                {
                    lst.Add(result);
                }
            }

            return lst;
        }

        private static HtmlNode FindFirstClass(HtmlNode node, string nodeClass)
        {
            return FindFirstAttribute(node, "class", nodeClass);
        }
        private static HtmlNode FindFirstId(HtmlNode node, string nodeClass)
        {
            return FindFirstAttribute(node, "id", nodeClass);
        }
        private static HtmlNode FindFirstAttribute(HtmlNode node, string attribute, string value = "")
        {
            if (node == null)
            {
                return null;
            }

            if (node.Attributes[attribute]?.Value.Contains(value) == true)
            {
                return node;
            }

            foreach (HtmlNode child in node.ChildNodes)
            {
                HtmlNode result = FindFirstAttribute(child, attribute, value);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
