using HtmlAgilityPack;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace moldovabolgaria_bot.Services
{
    public class NewsService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly Dictionary<int, string> _sources;
        private readonly Dictionary<int, string> _lastNews;
        private readonly System.Timers.Timer _timer;
        private ChatId _chatId;
        private static readonly TimeSpan StartTime = new (8, 0, 0);
        private static readonly TimeSpan EndTime = new (22, 0, 0);

        public NewsService(ITelegramBotClient botClient)
        {
            _botClient = botClient;
            _chatId = new ChatId(0);
            _sources = new Dictionary<int, string>
            {
                { 1, "https://nokta.md" },
                { 2, "https://newsmaker.md/ru/category/news" }
            };
            _lastNews = new Dictionary<int, string>();
            _timer = new System.Timers.Timer(3600000);
            _timer.Elapsed += async (sender, e) => await PostNewsAsync();
        }

        public async Task StartNewsPublishingAsync(ChatId chatId)
        {
            _chatId = chatId;
            await PostNewsAsync();
            _timer.Start();
        }

        public void SetTimer(double value)
        {
            _timer.Interval = value * 60000f;
        }

        public void StopNewsPublishing()
        {
            _timer.Stop();
        }

        private async Task PostNewsAsync()
        {
            var now = DateTime.Now.TimeOfDay;
            if (now < StartTime || now > EndTime)
            {
                Console.WriteLine("Время отдохнуть от публикации новостей! Спокойной ночи!");
                return;
            }

            foreach (var source in _sources)
            {
                var latestNews = await ParseNewsAsync(source.Value);

                if (string.IsNullOrEmpty(latestNews) || latestNews == "Нет новостей" || latestNews == "Ошибка загрузки новостей")
                {
                    Console.WriteLine($"Нет новых новостей из источника {source.Key}.");
                    continue;
                }

                if (_lastNews.ContainsKey(source.Key) && _lastNews[source.Key] == latestNews)
                    continue;

                _lastNews[source.Key] = latestNews;
                await _botClient.SendMessage(_chatId, latestNews);
                Console.WriteLine($"News posted from source {source.Key}: {latestNews}");
                return;
            }
        }


        private async Task<string> ParseNewsAsync(string url)
        {
            try
            {
                var client = new HttpClient();
                var html = await client.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                
                var node = doc.DocumentNode.SelectSingleNode("//a[contains(@class, 'list-item__link-inner')]");
                return node?.GetAttributeValue("href", "Нет новостей") ?? "Нет новостей";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка парсинга {url}: {ex.Message}");
                return "Ошибка загрузки новостей";
            }
        }
    }
}
