using System.Collections.Concurrent;

public static class UserState
{
    private static ConcurrentDictionary<long, string> userLanguages = new ConcurrentDictionary<long, string>();

    public static void SetUserLanguage(long userId, string languageCode)
    {
        userLanguages[userId] = languageCode;
    }

    public static string GetUserLanguage(long userId)
    {
        return userLanguages.TryGetValue(userId, out var language) ? language : "en"; // Default to English
    }
}