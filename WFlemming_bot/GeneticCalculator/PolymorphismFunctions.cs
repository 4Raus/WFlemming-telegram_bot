using Telegram.Bot;
using System.Threading.Tasks;

namespace GeneticCalculator
{
    public static class PolymorphismFunctions
    {
        public static async void HandlePolymorphism(ITelegramBotClient botClient, long chatId)
        {
            await botClient.SendTextMessageAsync(chatId, "Polymorphism calculation started...");
            //Plug
            string[] geneVariants = { "Variant A", "Variant B", "Variant C" };
            string message = $"Available gene variants: {string.Join(", ", geneVariants)}";

            await botClient.SendTextMessageAsync(chatId, message);
        }

        public static string GetGeneVariant(string genotype)
        {
            return genotype.Contains("A") ? "Variant A" : "Variant B";
        }
    }
}