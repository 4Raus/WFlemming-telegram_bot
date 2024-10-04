using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading.Tasks;

public static class Handlers
{
    public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update.Message?.Text != null)
        {
            await HandleMessageAsync(botClient, update.Message);
        }
        else if (update.Type == UpdateType.CallbackQuery)
        {
            await HandleCallbackQueryAsync(botClient, update.CallbackQuery);
        }
    }

    private static async Task HandleMessageAsync(ITelegramBotClient botClient, Message message)
    {
        var chatId = message.Chat.Id;
        switch (message.Text)
        {
            case "/start":
                UserState.SetUserLanguage(chatId, "en");
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: Messages.WelcomeMessageEnglish,
                    replyMarkup: InlineKeyboards.LanguageSelection
                );
                break;

            case "/clear":
                for (int i = 0; i < 50; i++)
                {
                    try
                    {
                        await botClient.DeleteMessageAsync(chatId, message.MessageId - i);
                    }
                    catch (Exception) { }
                }
                break;

            default:
                var language = UserState.GetUserLanguage(chatId);
                var replyMessage = language switch
                {
                    "ru" => Messages.UnknownCommandRu,
                    "de" => Messages.UnknownCommandDe,
                    _ => Messages.UnknownCommandEn
                };

                await botClient.SendTextMessageAsync(chatId, replyMessage);
                break;
        }
    }

    private static async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
        var chatId = callbackQuery.Message.Chat.Id;
        var callbackData = callbackQuery.Data;

        if (callbackQuery.Message != null)
        {
            await botClient.DeleteMessageAsync(chatId, callbackQuery.Message.MessageId);
        }

        InlineKeyboardMarkup keyboard;

        switch (callbackData)
        {
            case "lang_ru":
                UserState.SetUserLanguage(chatId, "ru");
                keyboard = InlineKeyboards.MainMenuRu;
                await botClient.SendTextMessageAsync(chatId, Messages.LanguageSelectedRu, replyMarkup: keyboard);
                break;

            case "lang_en":
                UserState.SetUserLanguage(chatId, "en");
                keyboard = InlineKeyboards.MainMenuEn;
                await botClient.SendTextMessageAsync(chatId, Messages.LanguageSelectedEn, replyMarkup: keyboard);
                break;

            case "lang_de":
                UserState.SetUserLanguage(chatId, "de");
                keyboard = InlineKeyboards.MainMenuDe;
                await botClient.SendTextMessageAsync(chatId, Messages.LanguageSelectedDe, replyMarkup: keyboard);
                break;

            case "lang_selection":
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Please choose your language:",
                    replyMarkup: InlineKeyboards.LanguageSelection
                );
                return;

            default:
                string responseMessage;
                keyboard = GetKeyboardForLanguage(UserState.GetUserLanguage(chatId));
                switch (callbackData)
                {
                    case "buttonChemistry":
                        responseMessage = GetLocalizedMessage("Вы выбрали химический калькулятор.",
                                                              "You have selected the chemistry calculator.",
                                                              "Sie haben den Chemie-Rechner ausgewählt.",
                                                              UserState.GetUserLanguage(chatId));
                        await botClient.SendTextMessageAsync(chatId, responseMessage, replyMarkup: keyboard);
                        break;

                    case "buttonGenetic":
                        responseMessage = GetLocalizedMessage("Вы выбрали генетический калькулятор.",
                                                              "You have selected the genetic calculator.",
                                                              "Sie haben den genetischen Rechner ausgewählt.",
                                                              UserState.GetUserLanguage(chatId));
                        await botClient.SendTextMessageAsync(chatId, responseMessage, replyMarkup: keyboard);
                        break;

                    case "buttonMeInfo":
                        responseMessage = GetLocalizedMessage(Messages.AboutBotRu, Messages.AboutBotEn, Messages.AboutBotDe, UserState.GetUserLanguage(chatId));
                        await botClient.SendTextMessageAsync(chatId, responseMessage, replyMarkup: keyboard);
                        break;

                    case "buttonMaterials":
                        responseMessage = GetLocalizedMessage(Messages.UsefulMaterialsRu, Messages.UsefulMaterialsEn, Messages.UsefulMaterialsDe, UserState.GetUserLanguage(chatId));
                        await botClient.SendTextMessageAsync(chatId, responseMessage, replyMarkup: keyboard);
                        break;

                    default:
                        await botClient.SendTextMessageAsync(chatId, "Unknown option selected.", replyMarkup: keyboard);
                        break;
                }
                break;
        }
    }

    private static string GetLocalizedMessage(string ru, string en, string de, string language)
    {
        return language switch
        {
            "ru" => ru,
            "de" => de,
            _ => en,
        };
    }

    private static InlineKeyboardMarkup GetKeyboardForLanguage(string language)
    {
        return language switch
        {
            "ru" => InlineKeyboards.MainMenuRu,
            "de" => InlineKeyboards.MainMenuDe,
            _ => InlineKeyboards.MainMenuEn,
        };
    }

    public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Error: {exception.Message}");
        return Task.CompletedTask;
    }
}