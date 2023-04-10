namespace StableDiffusionSdk.Infrastructure;

public static class ConsoleProgressBar
{
    public static void ShowProgressBar(int number, int outOf)
    {
        if (outOf < 1 || number < 0 || number > outOf)
        {
            throw new ArgumentOutOfRangeException("Invalid number or outOf values.");
        }

        int barWidth = Console.WindowWidth - 2;
        double progress = (double)number / outOf;
        int progressChars = (int)Math.Round(progress * barWidth);

        Console.CursorLeft = 0;
        Console.Write('[');

        for (int i = 0; i < barWidth; i++)
        {
            if (i < progressChars)
            {
                Console.Write('=');
            }
            else
            {
                Console.Write(' ');
            }
        }

        Console.Write(']');
    }
}