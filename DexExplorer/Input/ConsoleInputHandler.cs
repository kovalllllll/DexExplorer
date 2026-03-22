using DexExplorer.Models;

namespace DexExplorer.Input;

public static class ConsoleInputHandler
{
    public static (PoolState Pool, SwapDirection Direction, decimal AmountIn) Read()
    {
        Console.WriteLine("\n  ── Enter swap parameters ───────────────────────");

        var reserveA = ReadPositiveDecimal("  Reserve A (e.g. 1000)");
        var reserveB = ReadPositiveDecimal("  Reserve B (e.g. 1500000)");
        var fee = ReadDecimalInRange("  Fee (e.g. 0.003)", 0m, 1m);
        var direction = ReadDirection();
        var amountIn = ReadPositiveDecimal("  Amount In");

        return (new PoolState(reserveA, reserveB, fee), direction, amountIn);
    }

    private static decimal ReadPositiveDecimal(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            var raw = Console.ReadLine()?.Replace(',', '.');
            if (decimal.TryParse(raw,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var value) && value > 0)
                return value;

            Console.WriteLine("   Please enter a positive number.");
        }
    }

    private static decimal ReadDecimalInRange(string prompt, decimal min, decimal max)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            var raw = Console.ReadLine()?.Replace(',', '.');
            if (decimal.TryParse(raw,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var value) && value >= min && value < max)
                return value;

            Console.WriteLine($"   Value must be in [{min}, {max}).");
        }
    }

    private static SwapDirection ReadDirection()
    {
        while (true)
        {
            Console.Write("  Direction — A for (A to B) or B for (B to A): ");
            var key = Console.ReadLine()?.Trim().ToUpperInvariant();
            if (key == "A") return SwapDirection.AtoB;
            if (key == "B") return SwapDirection.BtoA;
            Console.WriteLine("   Please type A or B.");
        }
    }
}