using Telegram.Bot.Types.ReplyMarkups;

public static class InlineKeyboards
{
    public static InlineKeyboardMarkup LanguageSelection => new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Русский", "lang_ru"),
            InlineKeyboardButton.WithCallbackData("English", "lang_en"),
            InlineKeyboardButton.WithCallbackData("Deutsch", "lang_de")
        }
    });

    // Russian Main Menu
    public static InlineKeyboardMarkup MainMenuRu => new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Химический калькулятор", "buttonChemistry"),
            InlineKeyboardButton.WithCallbackData("Генетический калькулятор", "buttonGenetic")
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Обо мне", "buttonMeInfo"),
            InlineKeyboardButton.WithCallbackData("Полезные материалы", "buttonMaterials")
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Изменить язык", "lang_selection")
        }
    });

    // English Main Menu
    public static InlineKeyboardMarkup MainMenuEn => new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Chemistry calculator", "buttonChemistry"),
            InlineKeyboardButton.WithCallbackData("Genetic calculator", "buttonGenetic")
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("About Me", "buttonMeInfo"),
            InlineKeyboardButton.WithCallbackData("Useful Materials", "buttonMaterials")
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Change Language", "lang_selection")
        }
    });

    // German Main Menu
    public static InlineKeyboardMarkup MainMenuDe => new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Chemie-Rechner", "buttonChemistry"),
            InlineKeyboardButton.WithCallbackData("Genetik-Rechner", "buttonGenetic")
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Über mich", "buttonMeInfo"),
            InlineKeyboardButton.WithCallbackData("Nützliche Materialien", "buttonMaterials")
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Sprache ändern", "lang_selection")
        }
    });
}