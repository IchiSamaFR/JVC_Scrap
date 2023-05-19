

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
                await Task.Delay(1);
            }
            Console.WriteLine();


            Console.WriteLine(topicVisitor.GetTopWordFilter());
            
            //topicVisitor.ToJson("file.json");
        }
        private static void OnProgress(float value)
        {
            int progressSize = 20;
            int progress = (int)MathF.Round(value * progressSize);
            Console.SetCursorPosition(0, 0);
            Console.Write("[" + new string('#', progress)  + new string('.', progressSize - progress) + "]");
            Console.Write((value * 100).ToString("0") + "%");
        }
    }
}