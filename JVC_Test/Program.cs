

namespace JVC_Test
{
    public partial class Program
    {
        public static void Main()
        {
            Start();
        }
        public static async void Start()
        {
            var topicVisitor = new TopicVisitor();
            var task = topicVisitor.ParseTopics(100, OnProgress);
            while (!task.IsCompleted)
            {
                await Task.Delay(10);
            }
            Console.Clear();
            Console.WriteLine(topicVisitor.GetTopicsInfos());
            Console.ReadLine();
        }
        private static void OnProgress(float value)
        {
            Console.Clear();
            Console.Write((value * 100).ToString("0") + "%");
        }
    }
}