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