using Telegram.Bot;
using Telegram.Bot.Types;

namespace moldovabolgaria_bot.Services;

public static class PollService
{
    public static async Task PollMessage(TelegramBotClient bot)
    {
        var inputData = GetPollMessageDataFromConsole();
        if (!inputData.Item3) return;
        await bot.SendPoll("565260614",
            inputData.Item1,
            inputData.Item2);
    }
    
    private static (string, IEnumerable<InputPollOption>, bool) GetPollMessageDataFromConsole()
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
}