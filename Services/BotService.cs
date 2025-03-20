using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace moldovabolgaria_bot.Services;

public class BotService(TelegramBotClient bot)
{
    public Task OnError(Exception exception, HandleErrorSource errorSource)
    {
        Console.WriteLine(exception);
        return Task.CompletedTask;
    }

    public async Task OnMessage(Message msg, UpdateType updateType)
    {
        if (msg.Text == "/start")
        {
            await bot.SendMessage(msg.Chat, "Welcome! Pick one direction",
                replyMarkup: new InlineKeyboardButton[] { "Left", "Middle", "Right" });
        }
    }

    public async Task OnUpdate(Update update)
    {
        if (update is { CallbackQuery: { } query })
        {
            await bot.AnswerCallbackQuery(query.Id);
            await bot.SendMessage(query.Message!.Chat, $"User {query.From} in chat {query.Message.Chat} clicked on {query.Data}");
        }
    }
}