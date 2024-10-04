using Telegram.Bot;
using System.Threading.Tasks;

namespace GeneticCalculator
{
    public static class ComplementarityFunctions
    {
        public static async void HandleComplementarity(ITelegramBotClient botClient, long chatId)
        {
            await botClient.SendTextMessageAsync(chatId, "Complementarity calculations started...");
            //Plug
            string[] bases = { "A-T", "C-G" };
            string message = $"Base pairs: {string.Join(", ", bases)}";

            await botClient.SendTextMessageAsync(chatId, message);
        }

        public static char FindComplementaryBase(char baseChar)
        {
            return baseChar switch
            {
                'A' => 'T',
                'T' => 'A',
                'C' => 'G',
                'G' => 'C',
                _ => 'N' // IDK base
            };
        }
    }
}