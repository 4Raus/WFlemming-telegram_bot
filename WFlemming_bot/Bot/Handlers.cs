using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading.Tasks;
using GeneticCalculator;

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
        var userName = message.From?.Username ?? $"{message.From?.FirstName} {message.From?.LastName}";
        var userId = message.From?.Id;

        //Logs messages
        Console.WriteLine($"User: {userName} (ID: {userId}) sent message: \"{message.Text}\"");

        switch (message.Text)
        {
            case "/start":
                UserState.SetUserLanguage(chatId, "en");
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: Messages.WelcomeMessageEnglish,
                    replyMarkup: InlineKeyboards.LanguageSelection,
                    parseMode: ParseMode.Html
                );
                break;

            case "/clear":
                //Logs messages
                Console.WriteLine($"User: {userName} (ID: {userId}) requested to clear chat.");
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

                //Logs messages
                Console.WriteLine($"User: {userName} (ID: {userId}) sent an unrecognized command.");
                await botClient.SendTextMessageAsync(chatId, replyMessage);
                break;
        }
    }

    private static async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
        var chatId = callbackQuery.Message.Chat.Id;
        var callbackData = callbackQuery.Data;
        var userName = callbackQuery.From?.Username ?? $"{callbackQuery.From?.FirstName} {callbackQuery.From?.LastName}";
        var userId = callbackQuery.From?.Id;

        //Logs messages
        Console.WriteLine($"User: {userName} (ID: {userId}) selected option: \"{callbackData}\"");

        if (callbackQuery.Message != null)
        {
            await botClient.DeleteMessageAsync(chatId, callbackQuery.Message.MessageId);
        }

        var language = UserState.GetUserLanguage(chatId);
        InlineKeyboardMarkup keyboard;

        switch (callbackData)
        {
            // LANGUAGE
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

            // GENERAL MENU
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
            // GEN -> BLOOD
            case "gen_blood":
                // Instraction (how to use)
                var instruction = GetLocalizedMessage(
                    "Выберите генотип крови для расчета. Вы можете выбрать первую, вторую, третью или четвертую группу крови.",
                    "Select the blood genotype for calculation. You can choose the first, second, third, or fourth blood type.",
                    "Wählen Sie den Blutgenotyp zur Berechnung aus. Sie können den ersten, zweiten, dritten oder vierten Bluttyp wählen.",
                    language);

                keyboard = InlineKeyboards.BloodCalculatorMenu(language);
                await botClient.SendTextMessageAsync(chatId, instruction, replyMarkup: keyboard);
                break;

            case "blood_first_type":
                BloodFunctions.HandleBlood(botClient, chatId, "first_type", "first_type", language);
                break;

            case "blood_second_type_1":
                BloodFunctions.HandleBlood(botClient, chatId, "second_type_1", "second_type_1", language);
                break;

            case "blood_second_type_2":
                BloodFunctions.HandleBlood(botClient, chatId, "second_type_2", "second_type_2", language);
                break;

            case "blood_third_type_1":
                BloodFunctions.HandleBlood(botClient, chatId, "third_type_1", "third_type_1", language);
                break;

            case "blood_third_type_2":
                BloodFunctions.HandleBlood(botClient, chatId, "third_type_2", "third_type_2", language);
                break;

            case "blood_fourth_type":
                BloodFunctions.HandleBlood(botClient, chatId, "fourth_type", "fourth_type", language);
                break;

            case "blood_back":
                keyboard = InlineKeyboards.GeneticCalculatorMenu(language);
                var backMessage = GetLocalizedMessage(
                    "Выберите раздел генетического калькулятора:",
                    "Select a section of the genetic calculator:",
                    "Wählen Sie einen Abschnitt des genetischen Rechners:",
                    language);
                await botClient.SendTextMessageAsync(chatId, backMessage, replyMarkup: keyboard);
                break;
            
            //GEN -> POLYM
            case "gen_polymorphism":
                await botClient.SendTextMessageAsync(chatId, GetLocalizedMessage(
                    "Вы выбрали Полиморфизм. Начинаем расчет...",
                    "You selected Polymorphism. Starting calculation...",
                    "Sie haben Polymorphismus gewählt. Berechnung beginnt...",
                    language));
                GeneticCalculator.PolymorphismFunctions.HandlePolymorphism(botClient, chatId);
                await SendGeneticCalculatorMenu(botClient, chatId, language);
                break;

            //GEN -> COMPL
            case "gen_complementarity":
                await botClient.SendTextMessageAsync(chatId, GetLocalizedMessage(
                    "Вы выбрали Комплиментарность. Начинаем расчет...",
                    "You selected Complementarity. Starting calculation...",
                    "Sie haben Komplementarität gewählt. Berechnung beginnt...",
                    language));
                GeneticCalculator.ComplementarityFunctions.HandleComplementarity(botClient, chatId);
                await SendGeneticCalculatorMenu(botClient, chatId, language);
                break;

            //GEN -> CHROMO
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