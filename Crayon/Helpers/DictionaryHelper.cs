namespace Crayon.Helpers;

public static class DictionaryHelper
{
    public static string ToGroupName(this Dictionary<string, string> data)
    {
        return string.Join("_", data.OrderBy(x => x.Key).Select(x => $"{x.Key.ToLower()}_{x.Value.ToLower()}"));
    }
}