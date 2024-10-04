using Telegram.Bot;
using System.Threading.Tasks;

namespace GeneticCalculator
{
    public static class BloodFunctions
    {
        public static async void HandleBlood(ITelegramBotClient botClient, long chatId)
        {
            // ADD main logic
            await botClient.SendTextMessageAsync(chatId, "Blood calculation logic started...");
            //Plug
            string[] bloodTypes = { "A", "B", "AB", "O" };
            string bloodTypeMessage = $"Available blood types: {string.Join(", ", bloodTypes)}";

            await botClient.SendTextMessageAsync(chatId, bloodTypeMessage);
        }

        public static int CalculateBloodCompatibility(string bloodType1, string bloodType2)
        {
            if (bloodType1 == bloodType2)
            {
                return 100; // Full compatibility
            }
            else
            {
                return 50; // partial compatibility
            }
        }
    }
}