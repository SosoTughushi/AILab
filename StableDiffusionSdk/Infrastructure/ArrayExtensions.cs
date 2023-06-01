namespace StableDiffusionSdk.Infrastructure;

public static class ArrayExtensions
{
    public static T PickRandom<T>(this T[] array)
    {
        var random = new Random();
        return array[random.Next(array.Length)];
    }
    public static T PickRandom<T>(this List<T> array)
    {
        var random = new Random();
        return array[random.Next(array.Count)];
    }
}