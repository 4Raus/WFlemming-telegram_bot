using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

public static class BotInit
{
    public static ITelegramBotClient InitializeBot()
    {
        return new TelegramBotClient("7735038211:AAHvNz4HERya9d0u4SiwdmSehDTx2s-yBXQ");
    }

    public static ReceiverOptions ReceiverOptions => new()
    {
        AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery },
        ThrowPendingUpdates = true,
    };
}