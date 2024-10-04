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

        Console.WriteLine($"Received callback data: {callbackData}");

        if (callbackQuery.Message != null)
        {
            await botClient.DeleteMessageAsync(chatId, callbackQuery.Message.MessageId);
        }

        var language = UserState.GetUserLanguage(chatId);
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

            case "buttonChemistry":
                var responseMessageChemistry = GetLocalizedMessage("Вы выбрали химический калькулятор.",
                                                                  "You have selected the chemistry calculator.",
                                                                  "Sie haben den Chemie-Rechner ausgewählt.",
                                                                  language);
                keyboard = GetKeyboardForLanguage(language);
                await botClient.SendTextMessageAsync(chatId, responseMessageChemistry, replyMarkup: keyboard);
                break;

            case "buttonGenetic":
                keyboard = InlineKeyboards.GeneticCalculatorMenu(language);
                var responseMessageGenetic = GetLocalizedMessage(
                    "Выберите раздел генетического калькулятора:",
                    "Select a section of the genetic calculator:",
                    "Wählen Sie einen Abschnitt des genetischen Rechners:",
                    language);
                await botClient.SendTextMessageAsync(chatId, responseMessageGenetic, replyMarkup: keyboard);
                break;

            case "buttonMeInfo":
                var aboutMessage = GetLocalizedMessage(Messages.AboutBotRu, Messages.AboutBotEn, Messages.AboutBotDe, language);
                keyboard = GetKeyboardForLanguage(language);
                await botClient.SendTextMessageAsync(chatId, aboutMessage, replyMarkup: keyboard);
                break;

            case "buttonMaterials":
                var materialsMessage = GetLocalizedMessage(Messages.UsefulMaterialsRu, Messages.UsefulMaterialsEn, Messages.UsefulMaterialsDe, language);
                keyboard = GetKeyboardForLanguage(language);
                await botClient.SendTextMessageAsync(chatId, materialsMessage, replyMarkup: keyboard);
                break;

            // GEN
            case "gen_blood":
                await botClient.SendTextMessageAsync(chatId, GetLocalizedMessage(
                    "Вы выбрали Кровь. Начинаем расчет...",
                    "You selected Blood. Starting calculation...",
                    "Sie haben Blut gewählt. Berechnung beginnt...",
                    language));
                GeneticCalculator.BloodFunctions.HandleBlood(botClient, chatId);
                await SendGeneticCalculatorMenu(botClient, chatId, language);
                break;

            case "gen_polymorphism":
                await botClient.SendTextMessageAsync(chatId, GetLocalizedMessage(
                    "Вы выбрали Полиморфизм. Начинаем расчет...",
                    "You selected Polymorphism. Starting calculation...",
                    "Sie haben Polymorphismus gewählt. Berechnung beginnt...",
                    language));
                GeneticCalculator.PolymorphismFunctions.HandlePolymorphism(botClient, chatId);
                await SendGeneticCalculatorMenu(botClient, chatId, language);
                break;

            case "gen_complementarity":
                await botClient.SendTextMessageAsync(chatId, GetLocalizedMessage(
                    "Вы выбрали Комплиментарность. Начинаем расчет...",
                    "You selected Complementarity. Starting calculation...",
                    "Sie haben Komplementarität gewählt. Berechnung beginnt...",
                    language));
                GeneticCalculator.ComplementarityFunctions.HandleComplementarity(botClient, chatId);
                await SendGeneticCalculatorMenu(botClient, chatId, language);
                break;

            case "gen_chromosomes":
                await botClient.SendTextMessageAsync(chatId, GetLocalizedMessage(
                    "Вы выбрали Хромосомы. Начинаем расчет...",
                    "You selected Chromosomes. Starting calculation...",
                    "Sie haben Chromosomen gewählt. Berechnung beginnt...",
                    language));
                GeneticCalculator.ChromosomesFunctions.HandleChromosomes(botClient, chatId);
                await SendGeneticCalculatorMenu(botClient, chatId, language);
                break;

            case "gen_back":
                keyboard = GetKeyboardForLanguage(language);
                await botClient.SendTextMessageAsync(chatId, GetLocalizedMessage(
                    "Возврат в главное меню.",
                    "Returning to the main menu.",
                    "Zurück zum Hauptmenü.",
                    language), replyMarkup: keyboard);
                break;

            default:
                keyboard = GetKeyboardForLanguage(language);
                await botClient.SendTextMessageAsync(chatId, "Unknown option selected.", replyMarkup: keyboard);
                break;
        }
    }

    private static async Task SendGeneticCalculatorMenu(ITelegramBotClient botClient, long chatId, string language)
    {
        var geneticCalculatorMessage = GetLocalizedMessage(
            "Выберите раздел генетического калькулятора:",
            "Select a section of the genetic calculator:",
            "Wählen Sie einen Abschnitt des genetischen Rechners:",
            language);
        var keyboard = InlineKeyboards.GeneticCalculatorMenu(language);
        await botClient.SendTextMessageAsync(chatId, geneticCalculatorMessage, replyMarkup: keyboard);
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