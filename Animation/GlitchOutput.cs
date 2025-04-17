namespace NeonShell.Animation;

public static class GlitchOutput
{
    public static async Task GlitchedPrint(string message, TimeSpan delayPerChar)
    {
        var rand = new Random();
        var glitchChars = new[] { '#', '@', '%', '&', '*', '=', '~', '-', '/', '\\', '$', '+', '?' };

        char[] current = new char[message.Length];
        
        for (int i = 0; i < message.Length; i++)
        {
            current[i] = message[i] == ' ' ? ' ' : glitchChars[rand.Next(glitchChars.Length)];
        }
        
        for (int fixedIndex = 0; fixedIndex < message.Length; fixedIndex++)
        {
            for (int round = 0; round < 4; round++)
            {
                for (int i = fixedIndex; i < message.Length; i++)
                {
                    if (message[i] != ' ')
                        current[i] = glitchChars[rand.Next(glitchChars.Length)];
                }

                Console.Write("\r");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(new string(current));
                Console.ResetColor();

                await Task.Delay(delayPerChar / 2);
            }
            
            current[fixedIndex] = message[fixedIndex];

            Console.Write("\r");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(new string(current));
            Console.ResetColor();

            await Task.Delay(delayPerChar);
        }

        Console.WriteLine();
    }
}