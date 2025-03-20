using System.Text;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;
using var cts = new CancellationTokenSource();
var bot = new TelegramBotClient("8181148069:AAHGwZXVK1rLdQ45g5D8_KCf2BPqZ1Q_IkE", cancellationToken: cts.Token);
var me = await bot.GetMe();
bot.OnError += OnError;
bot.OnMessage += OnMessage;
bot.OnUpdate += OnUpdate;

Console.WriteLine($"@{me.Username} is running... Press Enter 'stop' to terminate");
while (true)
{
    var input = Console.ReadLine();
    if (input?.ToLower() == "stopbot")
    {
        cts.Cancel();
        break;
    }
    if (input?.ToLower() == "poll")
    {
        var pollData = GetPollMessageDataFromConsole();
        if (!pollData.Item3) continue;
        await PollMessage("565260614", pollData.Item1, pollData.Item2);
    }
    else Console.WriteLine("No such command!");
}

// method to handle errors in polling or in your OnMessage/OnUpdate code
Task OnError(Exception exception, HandleErrorSource source)
{
    Console.WriteLine(exception); // just dump the exception to the console
    return Task.CompletedTask;
}

// method that handle messages received by the bot:
async Task OnMessage(Message msg, UpdateType type)
{
    if (msg.Text == "/start")
    {
        await bot.SendMessage(msg.Chat, "Welcome! Pick one direction",
            replyMarkup: new InlineKeyboardButton[] { "Left", "Middle", "Right" });
    }
}

// method that handle other types of updates received by the bot:
async Task OnUpdate(Update update)
{
    if (update is { CallbackQuery: { } query }) // non-null CallbackQuery
    {
        await bot.AnswerCallbackQuery(query.Id);
        await bot.SendMessage(query.Message!.Chat, $"User {query.From} in chat {query.Message.Chat} clicked on {query.Data}");
    }
}

async Task PollMessage(ChatId chatId, string question, IEnumerable<InputPollOption> answers)
{
    await bot.SendPoll(chatId,
        question,
        answers);
}

(string, IEnumerable<InputPollOption>, bool) GetPollMessageDataFromConsole()
{
    QuestionLabel:
    Console.WriteLine("Enter Question");
    var question = Console.ReadLine();
    if (string.IsNullOrEmpty(question))
        goto QuestionLabel;
    
    Console.WriteLine("Enter Options. If finished paste 'end'");
    var options = new List<InputPollOption>();
    while (true)
    {
        var input = Console.ReadLine();
        if (string.IsNullOrEmpty(input)) continue;
        if (input == "end") break;
        options.Add(input);
    }
    
    Console.WriteLine($"Your poll: \nQ: {question}");
    foreach (var o in options)
    {
        Console.WriteLine($"Op: {o.Text}");
    }
    ConfirmationLabel:
    Console.WriteLine("Post - 'ok'\nCancel - 'x'");
    var confirmation = Console.ReadLine();
    if (!string.IsNullOrEmpty(confirmation))
    {
        switch (confirmation.ToLower())
        {
            case "ok":
                return (question, options, true);
            case "x":
                return (question, options, false);
        }
    }
    Console.WriteLine("Choose one of existing options!");
    goto ConfirmationLabel;
}