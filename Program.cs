using System.Text;
using moldovabolgaria_bot.Services;
using Telegram.Bot;
using static System.Double;

namespace moldovabolgaria_bot;

internal static class Program
{
    private static TelegramBotClient? _botClient;
    private static CancellationTokenSource? _cts;
    private static BotService? _botService;
    private static NewsService? _newsService;

    static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        try
        {
            // Initialization
            _botClient = new TelegramBotClient("8181148069:AAHGwZXVK1rLdQ45g5D8_KCf2BPqZ1Q_IkE");
            _cts = new CancellationTokenSource();
            _botService = new BotService(_botClient);
            _newsService = new NewsService(_botClient);

            // Start bot
            var me = await _botClient.GetMe();
            Console.WriteLine($"@{me.Username} is running... Press 'stopbot' to terminate.");

            // Subscriptions
            _botClient.OnError += _botService.OnError;
            _botClient.OnMessage += _botService.OnMessage;
            _botClient.OnUpdate += _botService.OnUpdate;

            // Handle console commands
            await HandleConsoleCommandsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        finally
        {
            await _cts?.CancelAsync()!;
        }
    }

    private static async Task HandleConsoleCommandsAsync()
    {
        while (true)
        {
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input)) continue;

            switch (input.ToLower())
            {
                case "stopbot":
                    await _cts?.CancelAsync()!;
                    Console.WriteLine("Bot stopped.");
                    return;

                case "poll":
                    await PollService.PollMessage(_botClient!, "565260614");
                    break;
                
                case "startnews":
                    await _newsService!.StartNewsPublishingAsync("565260614");
                    break;
                case "stopnews":
                    _newsService!.StopNewsPublishing();
                    break;
                case "setnewstimer":
                    Console.WriteLine("Enter value");
                    var value = Console.ReadLine();
                    if (!string.IsNullOrEmpty(value))
                    {
                        TryParse(value, out var timerValue);
                        _newsService?.SetTimer(timerValue);
                    }
                    break;

                default:
                    Console.WriteLine("No such command!");
                    break;
            }
        }
    }
}