namespace TestProject1.Helpers;

public static class ListHelper
{
    public static T Second<T>(this List<T> data) => data[1];
}