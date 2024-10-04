using Telegram.Bot;
using System.Threading.Tasks;

namespace GeneticCalculator
{
    public static class ChromosomesFunctions
    {
        public static async void HandleChromosomes(ITelegramBotClient botClient, long chatId)
        {
            await botClient.SendTextMessageAsync(chatId, "Chromosome calculations started...");
            //Plug
            int numberOfChromosomes = 23;
            string chromosomeInfo = $"Humans have {numberOfChromosomes} pairs of chromosomes.";

            await botClient.SendTextMessageAsync(chatId, chromosomeInfo);
        }

        public static int CalculateTotalChromosomes(int organism1Chromosomes, int organism2Chromosomes)
        {
            return organism1Chromosomes + organism2Chromosomes;
        }
    }
}