using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;


//NEED FIX (DOMINANT AND REC GEN) & FIND THE WAY OF NORMAL INTERPRETATION OF RESULT
namespace GeneticCalculator
{
    public class GenotypeResult
    {
        public string[] Genotypes { get; }
        public List<string> OffspringCombinations { get; }

        public GenotypeResult(string[] genotypes, List<string> offspringCombinations)
        {
            Genotypes = genotypes;
            OffspringCombinations = offspringCombinations;
        }
    }

    public interface IGeneticCalculator
    {
        Task<GenotypeResult> CalculateAsync(string genotype1, string genotype2);
    }

    public class DigipolnCalculator : IGeneticCalculator
    {
        public async Task<GenotypeResult> CalculateAsync(string genotype1, string genotype2)
        {
            // Determination of alleles
            string A1 = genotype1[0].ToString(); // Dominant allele 1
            string A2 = genotype1[1].ToString(); // Dominant allele 2
            string B1 = genotype1[2].ToString(); // Recessive allele 1
            string B2 = genotype1[3].ToString(); // Recessive allele 2
            string C1 = genotype2[0].ToString(); // Dominant allele 3
            string C2 = genotype2[1].ToString(); // Dominant allele 4
            string D1 = genotype2[2].ToString(); // Recessive allele 3
            string D2 = genotype2[3].ToString(); // Recessive allele 4

            // Genotypes
            string[] G = { $"{A1}{B1}", $"{A2}{B2}", $"{C1}{D1}", $"{C1}{D2}", $"{C2}{D1}", $"{C2}{D2}" };

            // Combinations of descendants
            var FResults = new List<string>
            {
                $"{A1}{C1}{B1}{D1}", // F1
                $"{A1}{C1}{B1}{D2}", // F2
                $"{A1}{C2}{B1}{D1}", // F3
                $"{A1}{C2}{B1}{D2}", // F4
                $"{A2}{C1}{B1}{D1}", // F5
                $"{A2}{C1}{B1}{D2}", // F6
                $"{A2}{C2}{B1}{D1}", // F7
                $"{A2}{C2}{B1}{D2}", // F8
                $"{A1}{C1}{B2}{D1}", // F9
                $"{A1}{C1}{B2}{D2}", // F10
                $"{A1}{C2}{B2}{D1}", // F11
                $"{A1}{C2}{B2}{D2}", // F12
                $"{A2}{C1}{B2}{D1}", // F13
                $"{A2}{C1}{B2}{D2}", // F14
                $"{A2}{C2}{B2}{D1}", // F15
                $"{A2}{C2}{B2}{D2}"  // F16
            };

            return new GenotypeResult(G, FResults);
        }
    }

    public class DiginepolnCalculator : IGeneticCalculator
    {
        public async Task<GenotypeResult> CalculateAsync(string genotype1, string genotype2)
        {
            // Determination of alleles
            string A1 = genotype1[0].ToString(); // Dominant allele 1
            string A2 = genotype1[1].ToString(); // Dominant allele 2
            string B1 = genotype1[2].ToString(); // Recessive allele 1
            string B2 = genotype1[3].ToString(); // Recessive allele 2
            string C1 = genotype2[0].ToString(); // Dominant allele 3
            string C2 = genotype2[1].ToString(); // Dominant allele 4
            string D1 = genotype2[2].ToString(); // Recessive allele 3
            string D2 = genotype2[3].ToString(); // Recessive allele 4

            // Genotypes
            string[] G = { $"{A1}{B1}", $"{A1}{B2}", $"{A2}{B1}", $"{A2}{B2}", $"{C1}{D1}", $"{C1}{D2}", $"{C2}{D1}", $"{C2}{D2}" };

            // Combinations of descendants
            var FResults = new List<string>
            {
                $"{A1}{C1}{B1}{D1}", // F1
                $"{A1}{C1}{B1}{D2}", // F2
                $"{A1}{C2}{B1}{D1}", // F3
                $"{A1}{C2}{B1}{D2}", // F4
                $"{A2}{C1}{B1}{D1}", // F5
                $"{A2}{C1}{B1}{D2}", // F6
                $"{A2}{C2}{B1}{D1}", // F7
                $"{A2}{C2}{B1}{D2}", // F8
                $"{A1}{C1}{B2}{D1}", // F9
                $"{A1}{C1}{B2}{D2}", // F10
                $"{A1}{C2}{B2}{D1}", // F11
                $"{A1}{C2}{B2}{D2}", // F12
                $"{A2}{C1}{B2}{D1}", // F13
                $"{A2}{C1}{B2}{D2}", // F14
                $"{A2}{C2}{B2}{D1}", // F15
                $"{A2}{C2}{B2}{D2}"  // F16
            };

            return new GenotypeResult(G, FResults);
        }
    }

    public static class GeneticCalculatorFunctions
    {
        public static async Task HandleChromosomeCalculation(ITelegramBotClient botClient, long chatId, string genotype1, string genotype2, string operation, string language)
        {
            IGeneticCalculator calculator = operation switch
            {
                "digipoln" => new DigipolnCalculator(),
                "diginepoln" => new DiginepolnCalculator(),
                _ => throw new ArgumentException("Unknown operation")
            };

            try
            {
                var result = await calculator.CalculateAsync(genotype1, genotype2);
                await SendResults(botClient, chatId, result, language);
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(chatId, "An error occurred: " + ex.Message);
            }
        }

        private static async Task SendResults(ITelegramBotClient botClient, long chatId, GenotypeResult result, string language)
        {
            // Markers for dominant and recessive alleles
            string dominantMarker = "●"; // Symbol for dominant (black circle)
            string recessiveMarker = "○"; // Symbol for recessive (white circle)

            // Identify dominant alleles based on specific criteria
            var dominantAlleles = new HashSet<string> { "fr", "is", "sc", "so", "ec", "eo" }; // Add all dominant alleles here

            var formattedGenotypes = result.Genotypes.Select(g =>
                dominantAlleles.Contains(g) ? $"{dominantMarker}{g}" : $"{recessiveMarker}{g}"
            );

            var formattedOffspring = result.OffspringCombinations.Select(f =>
                dominantAlleles.Contains(f) ? $"{dominantMarker}{f}" : $"{recessiveMarker}{f}"
            );

            var genotypesStr = string.Join(", ", formattedGenotypes);
            var offspringStr = string.Join(", ", formattedOffspring);

            var message = $"{GetLocalizedMessage("Calculation Results:", "Calculation Results:", "Berechnungsergebnisse:", language)}\n\n" +
                          $"{GetLocalizedMessage("Genotypes (G):", "Genotypes (G):", "Genotypen (G):", language)}\n" +
                          $"➜ {genotypesStr}\n\n" +
                          $"{GetLocalizedMessage("Offspring Combinations (F):", "Offspring Combinations (F):", "Nachkommenkombinationen (F):", language)}\n" +
                          $"➜ {offspringStr}\n\n" +
                          $"{GetLocalizedMessage("Please choose the next step:", "Please choose the next step:", "Bitte wählen Sie den nächsten Schritt:", language)}";

            await botClient.SendTextMessageAsync(chatId, message);
            await SendMenu(botClient, chatId, language);
        }

        private static async Task SendMenu(ITelegramBotClient botClient, long chatId, string language)
        {
            var instruction = GetLocalizedMessage(
                "Select a section of the genetic calculator:",
                "Select a section of the genetic calculator:",
                "Wählen Sie einen Abschnitt des genetischen Rechners:",
                language);
            var keyboard = InlineKeyboards.GeneticCalculatorMenu(language);
            await botClient.SendTextMessageAsync(chatId, instruction, replyMarkup: keyboard);
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


//namespace GeneticCalculator
//{
//    public static class PolymorphismFunctions
//    {
//        public static async void HandlePolymorphism(ITelegramBotClient botClient, long chatId)
//        {
//            await botClient.SendTextMessageAsync(chatId, "Polymorphism calculation started...");
//            //Plug
//            string[] geneVariants = { "Variant A", "Variant B", "Variant C" };
//            string message = $"Available gene variants: {string.Join(", ", geneVariants)}";

//            await botClient.SendTextMessageAsync(chatId, message);
//        }

//        public static string GetGeneVariant(string genotype)
//        {
//            return genotype.Contains("A") ? "Variant A" : "Variant B";
//        }
//    }
//}