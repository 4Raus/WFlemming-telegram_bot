using Telegram.Bot;
using System.Threading;

class Program
{
    private static ITelegramBotClient _botClient;

    static async Task Main()
    {
        // Initialize the bot
        _botClient = BotInit.InitializeBot();
        var cts = new CancellationTokenSource();

        // Start receiving updates
        _botClient.StartReceiving(Handlers.HandleUpdateAsync, Handlers.HandleErrorAsync, BotInit.ReceiverOptions, cts.Token);

        var me = await _botClient.GetMeAsync();
        Console.WriteLine($"{me.FirstName} launched!");

        // Keep the bot running
        await Task.Delay(-1);
    }
}






// First try
//using System.Text.Json.Serialization;
//using Telegram.Bot;
//using Telegram.Bot.Exceptions;
//using Telegram.Bot.Polling;
//using Telegram.Bot.Types;
//using Telegram.Bot.Types.Enums;
//using Telegram.Bot.Types.ReplyMarkups;
//using System.IO;
//using System.Threading;

//class Program
//{
//    private static ITelegramBotClient _botClient;
//    private static ReceiverOptions _receiverOptions;

//    static async Task Main()
//    {
//        _botClient = new TelegramBotClient("7735038211:AAHvNz4HERya9d0u4SiwdmSehDTx2s-yBXQ");
//        _receiverOptions = new ReceiverOptions
//        {
//            AllowedUpdates = new[]
//            {
//                UpdateType.Message, //just messages from keyboard for formuls (chemistry)
//                UpdateType.CallbackQuery //Inline buttons
//            },
//            ThrowPendingUpdates = true,
//        };
//        using var cts = new CancellationTokenSource();
//        _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token);

//        var me = await _botClient.GetMeAsync();

//        Console.WriteLine($"{me.FirstName} launched!");

//        await Task.Delay(-1);
//    }

//    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
//    {
//        try
//        {
//            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message && update.Message.Text != null)
//            {
//                var chatId = update.Message.Chat.Id;
//                var messageText = update.Message.Text;

//                if (messageText == "/start" || messageText == "Start")
//                {
//                    var startKeyboard = new ReplyKeyboardMarkup(new[]
//                    {
//                        new KeyboardButton[] {"Start"}
//                    })
//                    {
//                        ResizeKeyboard = true,
//                        OneTimeKeyboard = true,
//                    };

//                    await botClient.SendTextMessageAsync(
//                        chatId: chatId,
//                        text: "Hallöchen, mein Freund! My name is Walther Flemming and yes, I am a bot that will help you with both chemistry and genetics. So that we can start working with your tasks, please press the “start” button.");
//                }
//            }

//            switch (update.Type)
//            {
//                case UpdateType.Message:
//                {
//                    var message = update.Message;
//                    var user = message.From;

//                    Console.WriteLine($"{user.FirstName} ({user.Id}) wrote messaage: {message.Text}");

//                    var chat = message.Chat;

//                    switch (message.Type)
//                    {
//                        case MessageType.Text:
//                        {
//                            if (message.Text == "/start")
//                            {
//                                await botClient.SendTextMessageAsync(
//                                    chat.Id,
//                                    "Choose the type of keyboard to communicate with me.\n" +
//                                    "/inline  -> it means that the buttons will be located above the input line.\n");
//                                    //"/reply  -> it means that the buttons will be located below the input line. \n"
//                                return;
//                            }
//                            if (message.Text == "/inline")
//                            {
//                                var inlineKeyboard = new InlineKeyboardMarkup(
//                                    new List<InlineKeyboardButton[]>()
//                                    {
//                                        new InlineKeyboardButton[]
//                                        {
//                                            InlineKeyboardButton.WithCallbackData("Chemistry calculator", "buttonChemistry"),
//                                            InlineKeyboardButton.WithCallbackData("Genetic calculator", "buttonGenetic"),
//                                        },
//                                        new InlineKeyboardButton[]
//                                        {
//                                            InlineKeyboardButton.WithCallbackData("About Me", "buttonMeInfo"),
//                                            InlineKeyboardButton.WithCallbackData("Some good materials for you", "buttonMaterials"),
//                                        },
//                                    });
//                                await botClient.SendTextMessageAsync(
//                                    chat.Id,
//                                    "That's inline K.",
//                                    replyMarkup: inlineKeyboard);
//                                return;
//                            }
//                            //if (message.Text == "/reply")
//                            //{
//                            //    var replyKeyboard = new ReplyKeyboardMarkup(
//                            //        new List<KeyboardButton[]>()
//                            //        {
//                            //            new KeyboardButton[]
//                            //            {
//                            //                new KeyboardButton(""),
//                            //            }
//                            //        })
//                            //    {
//                            //        ResizeKeyboard = true
//                            //    };
//                            //    await botClient.SendTextMessageAsync(
//                            //        chat.Id,
//                            //        "That's reply K.",
//                            //        replyMarkup: replyKeyboard);
//                            //    return;
//                            //}
//                            return;
//                        }
//                        default:
//                        {
//                            await botClient.SendTextMessageAsync(
//                                chat.Id,
//                                "Use only text!");
//                            return;
//                        }
//                    }
//                    return;
//                }
//            }
//        }
//        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
//    }

//    private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
//    {
//        var ErrorMessage = error switch
//        {
//            ApiRequestException apiRequestException
//                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
//            _ => error.ToString()
//        };
//        Console.WriteLine(ErrorMessage);
//        return Task.CompletedTask;
//    }
//}

////switch (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message && update.Message.Text != null)
////{
////    case UpdateType.Message:
////        {
////            var chatId = update.Message.Chat.Id;
////            var messageText = update.Message.Text;

////            if (messageText == "/start" || messageText == "Start")
////            {
////                var startKeyboard = new ReplyKeyboardMarkup(new[]
////                {
////                            new KeyboardButton[] {"Start"}
////                        })
////                {
////                    ResizeKeyboard = true,
////                    OneTimeKeyboard = true,
////                };

////                await botClient.SendTextMessageAsync(
////                    chatId: chatId,
////                    text: "Hallöchen, mein Freund! My name is Walther Flemming and yes, I am a bot that will help you with both chemistry and genetics. So that we can start working with your tasks, please press the “start” button.",
////                    replyMarkup: startKeyboard,
////                    cancellationToken: cancellationToken);
////            }
////            else if (messageText == "Start")
////            {
////                var replyKeyboardMarkup = new ReplyKeyboardMarkup(
////                    new List<KeyboardButton[]>()
////                    {
////                                new KeyboardButton[]
////                                {
////                                    new KeyboardButton("Chemistry calculator"),
////                                    new KeyboardButton("Genetic calculator"),
////                                },
////                                new KeyboardButton[]
////                                {
////                                    new KeyboardButton("About Me"),
////                                    new KeyboardButton("Some good materials for u")
////                                },
////                    })
////                {
////                    ResizeKeyboard = true,
////                };

////                await botClient.SendTextMessageAsync(
////                    chatId: chatId,
////                    text:);
////            }
////        }

////}