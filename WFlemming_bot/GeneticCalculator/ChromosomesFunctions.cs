using Telegram.Bot;
using System.Threading.Tasks;

//namespace GeneticCalculator
//{
//    public static class ChromosomesFunctions
//    {
//        public static async void HandleChromosomes(ITelegramBotClient botClient, long chatId)
//        {
//            await botClient.SendTextMessageAsync(chatId, "Chromosome calculations started...");
//            //Plug
//            int numberOfChromosomes = 23;
//            string chromosomeInfo = $"Humans have {numberOfChromosomes} pairs of chromosomes.";

//            await botClient.SendTextMessageAsync(chatId, chromosomeInfo);
//        }

//        public static int CalculateTotalChromosomes(int organism1Chromosomes, int organism2Chromosomes)
//        {
//            return organism1Chromosomes + organism2Chromosomes;
//        }
//    }
//}

namespace GeneticCalculator
{
    public static class ChromosomesFunctions
    {
        public static async void HandleChromosomes(ITelegramBotClient botClient, long chatId, string genotype1, string genotype2, string language)
        {
            string result = CalculateChromosomeGenotype(genotype1, genotype2, language);
            await botClient.SendTextMessageAsync(chatId, result);

            var instruction = GetLocalizedMessage(
                "Выберите раздел генетического калькулятора:",
                "Select a section of the genetic calculator:",
                "Wählen Sie einen Abschnitt des genetischen Rechners:",
                language);
            var keyboard = InlineKeyboards.GeneticCalculatorMenu(language);
            await botClient.SendTextMessageAsync(chatId, instruction, replyMarkup: keyboard);
        }

        private static string CalculateChromosomeGenotype(string genotype1, string genotype2, string language)
        {
            var chromosomeCombinations = new Dictionary<(string, string), (string P1, string P2, string[] gametes)>
            {
                { ("1_type", "3_type"), ("X⁰X⁰", "X⁰Y", new[] { "X⁰", "X⁰", "Y" }) },
                { ("1_type", "4_type"), ("X⁰X⁰", "XᴷY", new[] { "X⁰", "Xᴷ", "Y" }) },
                { ("2_type", "3_type"), ("XᴷX⁰", "X⁰Y", new[] { "Xᴷ", "X⁰", "Y" }) },
                { ("2_type", "4_type"), ("XᴷX⁰", "XᴷY", new[] { "Xᴷ", "X⁰", "Y" }) },
            };

            // Check for the matching combination, or reverse order
            if (chromosomeCombinations.TryGetValue((genotype1, genotype2), out var values) ||
                chromosomeCombinations.TryGetValue((genotype2, genotype1), out values))
            {
                var (P1, P2, gametes) = values;
                var formattedResults = GenerateGameteCombinations(gametes);
                return GenerateFormattedResults(P1, P2, gametes, formattedResults, language);
            }

            return GetLocalizedMessage(
                "Неизвестная комбинация хромосом.",
                "Unknown combination of chromosomes.",
                "Unbekannte Kombination von Chromosomen.",
                language);
        }

        // Method to generate all possible offspring combinations from the gametes
        private static string[] GenerateGameteCombinations(string[] gametes)
        {
            var combinations = new List<string>();

            for (int i = 0; i < gametes.Length; i++)
            {
                for (int j = 0; j < gametes.Length; j++)
                {
                    combinations.Add($"{gametes[i]}{gametes[j]}");
                }
            }

            return combinations.ToArray();
        }

        private static string GenerateFormattedResults(string P1, string P2, string[] gametes, string[] fCombinations, string language)
        {
            var gametesStr = string.Join(", ", gametes);
            var fCombinationsStr = string.Join(", ", fCombinations);

            return language switch
            {
                "ru" => $"Родитель 1 (P1): {P1}\nРодитель 2 (P2): {P2}\nГаметы: {gametesStr}\nКомбинации потомков (F): {fCombinationsStr}",
                "de" => $"Elternteil 1 (P1): {P1}\nElternteil 2 (P2): {P2}\nGameten: {gametesStr}\nKombinationen der Nachkommen (F): {fCombinationsStr}",
                _ => $"Parent 1 (P1): {P1}\nParent 2 (P2): {P2}\nGametes: {gametesStr}\nOffspring Combinations (F): {fCombinationsStr}"
            };
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
    }
}