using Telegram.Bot;
using System.Threading.Tasks;

namespace GeneticCalculator
{
    public static class BloodFunctions
    {
        public static async void HandleBlood(ITelegramBotClient botClient, long chatId, string genotype1, string genotype2, string language)
        {
            string result = CalculateBloodGenotype(genotype1, genotype2, language);
            await botClient.SendTextMessageAsync(chatId, result);

            var instruction = GetLocalizedMessage(
                "Выберите раздел генетического калькулятора:",
                "Select a section of the genetic calculator:",
                "Wählen Sie einen Abschnitt des genetischen Rechners:",
                language);
            var keyboard = InlineKeyboards.GeneticCalculatorMenu(language);
            await botClient.SendTextMessageAsync(chatId, instruction, replyMarkup: keyboard);
        }

        private static string CalculateBloodGenotype(string genotype1, string genotype2, string language)
        {
            var genotypeCombinations = new Dictionary<(string, string), (string, string, string[])>
    {
        { ("first_type", "first_type"), ("j⁰j⁰", "j⁰j⁰", new[] { "j⁰", "j⁰" }) },
        { ("first_type", "second_type_1"), ("j⁰j⁰", "JᴬJᴬ", new[] { "j⁰", "Jᴬ" }) },
        { ("first_type", "second_type_2"), ("j⁰j⁰", "JᴬJ⁰", new[] { "j⁰", "Jᴬ", "j⁰" }) },
        { ("first_type", "third_type_1"), ("j⁰j⁰", "JᴮJᴮ", new[] { "j⁰", "Jᴮ" }) },
        { ("first_type", "third_type_2"), ("j⁰j⁰", "JᴮJ⁰", new[] { "j⁰", "Jᴮ", "j⁰" }) },
        { ("first_type", "fourth_type"), ("j⁰j⁰", "JᴬJᴮ", new[] { "j⁰", "Jᴬ", "Jᴮ" }) },

        { ("second_type_1", "first_type"), ("JᴬJᴬ", "j⁰j⁰", new[] { "Jᴬ", "j⁰" }) },
        { ("second_type_1", "second_type_1"), ("JᴬJᴬ", "JᴬJᴬ", new[] { "Jᴬ", "Jᴬ" }) },
        { ("second_type_1", "second_type_2"), ("JᴬJᴬ", "JᴬJ⁰", new[] { "Jᴬ", "Jᴬ", "j⁰" }) },

        { ("second_type_2", "first_type"), ("JᴬJ⁰", "j⁰j⁰", new[] { "Jᴬ", "j⁰" }) },
        { ("second_type_2", "second_type_1"), ("JᴬJ⁰", "JᴬJᴬ", new[] { "Jᴬ", "Jᴬ", "j⁰" }) },
        { ("second_type_2", "second_type_2"), ("JᴬJ⁰", "JᴬJ⁰", new[] { "Jᴬ", "Jᴬ", "j⁰" }) },

        { ("third_type_1", "third_type_1"), ("JᴮJᴮ", "JᴮJᴮ", new[] { "Jᴮ" }) },
        { ("third_type_1", "fourth_type"), ("JᴮJᴮ", "JᴬJᴮ", new[] { "Jᴮ", "Jᴬ" }) },

        { ("third_type_2", "first_type"), ("JᴮJ⁰", "j⁰j⁰", new[] { "Jᴮ", "j⁰" }) },
        { ("third_type_2", "third_type_1"), ("JᴮJ⁰", "JᴮJᴮ", new[] { "Jᴮ", "j⁰" }) },
        { ("third_type_2", "third_type_2"), ("JᴮJ⁰", "JᴮJ⁰", new[] { "Jᴮ", "j⁰" }) },
        { ("third_type_2", "fourth_type"), ("JᴮJ⁰", "JᴬJᴮ", new[] { "Jᴮ", "Jᴬ", "j⁰" }) },

        { ("fourth_type", "fourth_type"), ("JᴬJᴮ", "JᴬJᴮ", new[] { "Jᴬ", "Jᴮ" }) },
    };

            // Possibility of reverse combinations to avoid duplicating entries
            if (genotypeCombinations.TryGetValue((genotype1, genotype2), out var values) ||
                genotypeCombinations.TryGetValue((genotype2, genotype1), out values))
            {
                var (P1, P2, genotypes) = values;
                var formattedResults = GenerateFormattedResults(genotypes);
                return FormatResult(P1, P2, genotypes, formattedResults, language);
            }

            return GetLocalizedMessage(
                "Неизвестная комбинация генотипов.",
                "Unknown combination of genotypes.",
                "Unbekannte Kombination von Genotypen.",
                language);
        }

        private static string[] GenerateFormattedResults(string[] genotypes)
        {
            var formattedResults = new List<string>();

            for (int i = 0; i < genotypes.Length; i++)
            {
                for (int j = i; j < genotypes.Length; j++)
                {
                    formattedResults.Add($"{genotypes[i]}{genotypes[j]}");
                }
            }

            return formattedResults.ToArray();
        }


        // Prev. version
        //private static string CalculateBloodGenotype(string genotype1, string genotype2, string language)
        //{
        //    string P1, P2, G1, G2, G3, G4;
        //    string F1, F2, F3, F4;

        //    switch (genotype1)
        //    {
        //        case "first_type":
        //            switch (genotype2)
        //            {
        //                case "first_type":
        //                    P1 = "j⁰j⁰";
        //                    P2 = "j⁰j⁰";
        //                    G1 = "j⁰";
        //                    G2 = "j⁰";
        //                    F1 = $"{G1}{G2}";
        //                    return FormatResult(P1, P2, new[] { G1, G2 }, new[] { F1 }, language);

        //                case "second_type_1":
        //                    P1 = "j⁰j⁰";
        //                    P2 = "JᴬJᴬ";
        //                    G1 = "j⁰";
        //                    G2 = "Jᴬ";
        //                    F1 = $"{G2}{G1}";
        //                    return FormatResult(P1, P2, new[] { G1, G2 }, new[] { F1 }, language);

        //                case "second_type_2":
        //                    P1 = "j⁰j⁰";
        //                    P2 = "JᴬJ⁰";
        //                    G1 = "j⁰";
        //                    G2 = "Jᴬ";
        //                    G3 = "j⁰";
        //                    F1 = $"{G2}{G1}";
        //                    F2 = $"{G1}{G3}";
        //                    return FormatResult(P1, P2, new[] { G1, G2, G3 }, new[] { F1, F2 }, language);

        //                case "third_type_1":
        //                    P1 = "j⁰j⁰";
        //                    P2 = "JᴮJᴮ";
        //                    G1 = "j⁰";
        //                    G2 = "Jᴮ";
        //                    F1 = $"{G2}{G1}";
        //                    return FormatResult(P1, P2, new[] { G1, G2 }, new[] { F1 }, language);

        //                case "third_type_2":
        //                    P1 = "j⁰j⁰";
        //                    P2 = "JᴮJ⁰";
        //                    G1 = "j⁰";
        //                    G2 = "Jᴮ";
        //                    G3 = "j⁰";
        //                    F1 = $"{G2}{G1}";
        //                    F2 = $"{G1}{G3}";
        //                    return FormatResult(P1, P2, new[] { G1, G2, G3 }, new[] { F1, F2 }, language);

        //                case "fourth_type":
        //                    P1 = "j⁰j⁰";
        //                    P2 = "JᴬJᴮ";
        //                    G1 = "j⁰";
        //                    G2 = "Jᴬ";
        //                    G3 = "Jᴮ";
        //                    F1 = $"{G2}{G1}";
        //                    F2 = $"{G3}{G1}";
        //                    return FormatResult(P1, P2, new[] { G1, G2, G3 }, new[] { F1, F2 }, language);

        //        default:
        //            return GetLocalizedMessage(
        //                "Неизвестная комбинация генотипов.",
        //                "Unknown combination of genotypes.",
        //                "Unbekannte Kombination von Genotypen.",
        //                language);
        //            }

        //        case "second_type_1":
        //            switch (genotype2)
        //            {
        //                case "first_type":
        //                    P1 = "JᴬJᴬ";
        //                    P2 = "j⁰j⁰";
        //                    G1 = "Jᴬ";
        //                    G2 = "j⁰";
        //                    F1 = $"{G1}{G2}";
        //                    return FormatResult(P1, P2, new[] { G1, G2 }, new[] { F1 }, language);

        //                case "second_type_1":
        //                    P1 = "JᴬJᴬ";
        //                    P2 = "JᴬJᴬ";
        //                    G1 = "Jᴬ";
        //                    G2 = "Jᴬ";
        //                    F1 = $"{G1}{G1}";
        //                    return FormatResult(P1, P2, new[] { G1, G2 }, new[] { F1 }, language);

        //                case "second_type_2":
        //                    P1 = "JᴬJᴬ";
        //                    P2 = "JᴬJ⁰";
        //                    G1 = "Jᴬ";
        //                    G2 = "Jᴬ";
        //                    G3 = "j⁰";
        //                    F1 = $"{G1}{G2}";
        //                    F2 = $"{G1}{G3}";
        //                    return FormatResult(P1, P2, new[] { G1, G2, G3 }, new[] { F1, F2 }, language);

        //                default:
        //                    return GetLocalizedMessage(
        //                        "Неизвестная комбинация генотипов.",
        //                        "Unknown combination of genotypes.",
        //                        "Unbekannte Kombination von Genotypen.",
        //                        language);
        //            }

        //        case "second_type_2":
        //            switch (genotype2)
        //            {
        //                case "first_type":
        //                    P1 = "JᴬJ⁰";
        //                    P2 = "j⁰j⁰";
        //                    G1 = "Jᴬ";
        //                    G2 = "j⁰";
        //                    F1 = $"{G1}{G2}";
        //                    return FormatResult(P1, P2, new[] { G1, G2 }, new[] { F1 }, language);

        //                case "second_type_1":
        //                    P1 = "JᴬJ⁰";
        //                    P2 = "JᴬJᴬ";
        //                    G1 = "Jᴬ";
        //                    G2 = "j⁰";
        //                    F1 = $"{G1}{G1}";
        //                    F2 = $"{G1}{G2}";
        //                    return FormatResult(P1, P2, new[] { G1, G2 }, new[] { F1, F2 }, language);

        //                case "second_type_2":
        //                    P1 = "JᴬJ⁰";
        //                    P2 = "JᴬJ⁰";
        //                    G1 = "Jᴬ";
        //                    G2 = "j⁰";
        //                    F1 = $"{G1}{G1}";
        //                    F2 = $"{G1}{G2}";
        //                    F3 = $"{G2}{G2}";
        //                    return FormatResult(P1, P2, new[] { G1, G2 }, new[] { F1, F2, F3 }, language);

        //                default:
        //                    return GetLocalizedMessage(
        //                        "Неизвестная комбинация генотипов.",
        //                        "Unknown combination of genotypes.",
        //                        "Unbekannte Kombination von Genotypen.",
        //                        language);
        //            }
        //            break;

        //        case "third_type_1":
        //            switch (genotype2)
        //            {
        //                case "third_type_1":
        //                    P1 = "JᴮJᴮ";
        //                    P2 = "JᴮJᴮ";
        //                    G1 = "Jᴮ";
        //                    F1 = $"{G1}{G1}";
        //                    return FormatResult(P1, P2, new[] { G1 }, new[] { F1 }, language);

        //                case "fourth_type":
        //                    P1 = "JᴮJᴮ";
        //                    P2 = "JᴬJᴮ";
        //                    G1 = "Jᴮ";
        //                    G2 = "Jᴬ";
        //                    F1 = $"{G1}{G2}";
        //                    F2 = $"{G1}{G1}";
        //                    return FormatResult(P1, P2, new[] { G1, G2 }, new[] { F1, F2 }, language);

        //                default:
        //                    return GetLocalizedMessage(
        //                        "Неизвестная комбинация генотипов.",
        //                        "Unknown combination of genotypes.",
        //                        "Unbekannte Kombination von Genotypen.",
        //                        language);
        //            }
        //            break;

        //        case "third_type_2":
        //            switch (genotype2)
        //            {
        //                case "first_type":
        //                    P1 = "JᴮJ⁰";
        //                    P2 = "j⁰j⁰";
        //                    G1 = "Jᴮ";
        //                    G2 = "j⁰";
        //                    F1 = $"{G1}{G2}";
        //                    return FormatResult(P1, P2, new[] { G1, G2 }, new[] { F1 }, language);

        //                case "third_type_1":
        //                    P1 = "JᴮJ⁰";
        //                    P2 = "JᴮJᴮ";
        //                    G1 = "Jᴮ";
        //                    G2 = "j⁰";
        //                    F1 = $"{G1}{G1}";
        //                    F2 = $"{G1}{G2}";
        //                    return FormatResult(P1, P2, new[] { G1, G2 }, new[] { F1, F2 }, language);

        //                case "third_type_2":
        //                    P1 = "JᴮJ⁰";
        //                    P2 = "JᴮJ⁰";
        //                    G1 = "Jᴮ";
        //                    G2 = "j⁰";
        //                    F1 = $"{G1}{G1}";
        //                    F2 = $"{G1}{G2}";
        //                    F3 = $"{G2}{G2}";
        //                    return FormatResult(P1, P2, new[] { G1, G2 }, new[] { F1, F2, F3 }, language);

        //                case "fourth_type":
        //                    P1 = "JᴮJ⁰";
        //                    P2 = "JᴬJᴮ";
        //                    G1 = "Jᴮ";
        //                    G2 = "Jᴬ";
        //                    G3 = "j⁰";
        //                    F1 = $"{G1}{G2}";
        //                    F2 = $"{G2}{G3}";
        //                    F3 = $"{G1}{G3}";
        //                    return FormatResult(P1, P2, new[] { G1, G2, G3 }, new[] { F1, F2, F3 }, language);

        //                default:
        //                    return GetLocalizedMessage(
        //                        "Неизвестная комбинация генотипов.",
        //                        "Unknown combination of genotypes.",
        //                        "Unbekannte Kombination von Genotypen.",
        //                        language);
        //            }
        //            break;

        //        case "fourth_type":
        //            switch (genotype2)
        //            {
        //                case "fourth_type":
        //                    P1 = "JᴬJᴮ";
        //                    P2 = "JᴬJᴮ";
        //                    G1 = "Jᴬ";
        //                    G2 = "Jᴮ";
        //                    F1 = $"{G1}{G1}";
        //                    F2 = $"{G1}{G2}";
        //                    F3 = $"{G2}{G1}";
        //                    F4 = $"{G2}{G2}";
        //                    return FormatResult(P1, P2, new[] { G1, G2 }, new[] { F1, F2, F3, F4 }, language);

        //                default:
        //                    return GetLocalizedMessage(
        //                        "Неизвестная комбинация генотипов.",
        //                        "Unknown combination of genotypes.",
        //                        "Unbekannte Kombination von Genotypen.",
        //                        language);
        //            }
        //            break;

        //        default:
        //            return "Unknown genotype.";
        //    }
        //    return GetLocalizedMessage(
        //        "Неизвестная комбинация генотипов.",
        //        "Unknown combination of genotypes.",
        //        "Unbekannte Kombination von Genotypen.",
        //        language);
        //}

        private static string FormatResult(string P1, string P2, string[] gametes, string[] fCombinations, string language)
        {
            var gametesStr = string.Join(", ", gametes);
            var fCombinationsStr = string.Join(", ", fCombinations);

            return language switch
            {
                "ru" => $"P1: {P1}, P2: {P2}\nГаметы: {gametesStr}\nКомбинации F: {fCombinationsStr}",
                "de" => $"P1: {P1}, P2: {P2}\nGameten: {gametesStr}\nF Kombinationen: {fCombinationsStr}",
                _ => $"P1: {P1}, P2: {P2}\nGametes: {gametesStr}\nF Combinations: {fCombinationsStr}"
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