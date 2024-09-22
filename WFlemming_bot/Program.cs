using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

//TODO:
//1. Start like a button.
//2. No keyboard.
//3. Logic

class Program
{
    private static ITelegramBotClient _botClient;
    private static ReceiverOptions _receiverOptions;

    static async Task Main()
    {
        _botClient = new TelegramBotClient("7735038211:AAHvNz4HERya9d0u4SiwdmSehDTx2s-yBXQ");
        _receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[]
            {
                UpdateType.Message, //just messages from keyboard
                UpdateType.CallbackQuery //Inline buttons
            },
            ThrowPendingUpdates = true,
        };
        using var cts = new CancellationTokenSource();
        _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token);

        var me = await _botClient.GetMeAsync();

        Console.WriteLine($"{me.FirstName} launched!");

        await Task.Delay(-1);
    }

    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                {
                    var message = update.Message;
                    var user = message.From;

                    Console.WriteLine($"{user.FirstName} ({user.Id}) wrote messaage: {message.Text}");

                    var chat = message.Chat;

                    switch (message.Type)
                    {
                        case MessageType.Text:
                        {
                            if (message.Text == "/start")
                            {
                                await botClient.SendTextMessageAsync(
                                    chat.Id,
                                    "Choose the type of keyboard to communicate with me.\n" +
                                    "/inline  -> it means that the buttons will be located above the input line.\n");
                                    //"/reply  -> it means that the buttons will be located below the input line. \n"
                                return;
                            }
                            if (message.Text == "/inline")
                            {
                                var inlineKeyboard = new InlineKeyboardMarkup(
                                    new List<InlineKeyboardButton[]>()
                                    {
                                        new InlineKeyboardButton[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("Chemistry calculator", "buttonChemistry"),
                                            InlineKeyboardButton.WithCallbackData("Genetic calculator", "buttonGenetic"),
                                        },
                                        new InlineKeyboardButton[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("About Me", "buttonMeInfo"),
                                            InlineKeyboardButton.WithCallbackData("Some good materials for you", "buttonMaterials"),
                                        },
                                    });
                                await botClient.SendTextMessageAsync(
                                    chat.Id,
                                    "That's inline K.",
                                    replyMarkup: inlineKeyboard);
                                return;
                            }
                            //if (message.Text == "/reply")
                            //{
                            //    var replyKeyboard = new ReplyKeyboardMarkup(
                            //        new List<KeyboardButton[]>()
                            //        {
                            //            new KeyboardButton[]
                            //            {
                            //                new KeyboardButton(""),
                            //            }
                            //        })
                            //    {
                            //        ResizeKeyboard = true
                            //    };
                            //    await botClient.SendTextMessageAsync(
                            //        chat.Id,
                            //        "That's reply K.",
                            //        replyMarkup: replyKeyboard);
                            //    return;
                            //}
                            return;
                        }
                        default:
                        {
                            await botClient.SendTextMessageAsync(
                                chat.Id,
                                "Use only text!");
                            return;
                        }
                    }
                    return;
                }
            }
        }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
    }

    private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        var ErrorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };
        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}