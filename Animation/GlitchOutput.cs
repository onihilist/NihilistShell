
namespace NeonShell.Animation;

/// <summary>
/// The <c>GlitchOutput</c> class provides a method to simulate a "glitch" text animation effect
/// by displaying characters that gradually resolve into the intended message.
/// </summary>
public static class GlitchOutput
{
    /// <summary>
    /// Asynchronously prints a message to the console using a glitch animation effect.
    /// Each character is temporarily replaced with random symbols before settling on its final form.
    /// </summary>
    /// <param name="message">The final message to display.</param>
    /// <param name="delayPerChar">
    /// The delay between each character reveal, influencing the total animation time.
    /// A shorter delay results in faster glitch transitions.
    /// </param>
    /// <returns>A <see cref="Task"/> representing the asynchronous animation operation.</returns>
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
