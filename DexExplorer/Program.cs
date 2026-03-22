using DexExplorer.Input;
using DexExplorer.Models;
using DexExplorer.Reporting;
using DexExplorer.Scenarios;
using DexExplorer.Services;

namespace DexExplorer;

public static class Program
{
    private static int Main()
    {
        var calculator = new AmmCalculator();
        var runner = new ScenarioRunner(calculator);

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════════════╗");
            Console.WriteLine("  ║     Constant-Product DEX Explorer        ║");
            Console.WriteLine("  ╠══════════════════════════════════════════╣");
            Console.WriteLine("  ║  [1]  Run predefined scenarios           ║");
            Console.WriteLine("  ║  [2]  Enter a custom swap                ║");
            Console.WriteLine("  ║  [Q]  Quit                               ║");
            Console.WriteLine("  ╚══════════════════════════════════════════╝");

            switch (Console.ReadLine()?.Trim().ToUpperInvariant())
            {
                case "1":
                    RunPredefined(runner);
                    break;
                case "2":
                    RunCustom(calculator);
                    break;
                case "Q":
                    Console.WriteLine("\n  Goodbye!");
                    return 0;
                default:
                    Console.WriteLine("   Unknown option.");
                    break;
            }
        }
    }

    private static void RunPredefined(ScenarioRunner runner)
    {
        try
        {
            var results = runner.RunAll();
            ConsoleReporter.PrintPoolHeader(results[0].PoolBefore);
            foreach (var r in results)
                ConsoleReporter.PrintSwapResult(r);
            ConsoleReporter.PrintSummaryTable(results);
        }
        catch (ArgumentException ex)
        {
            WriteError(ex.Message);
        }
    }

    private static void RunCustom(AmmCalculator calculator)
    {
        try
        {
            var (pool, direction, amountIn) = ConsoleInputHandler.Read();
            var result = calculator.Calculate(
                new SwapParameters(pool, direction, amountIn), "Custom swap");
            ConsoleReporter.PrintPoolHeader(pool);
            ConsoleReporter.PrintSwapResult(result);
        }
        catch (ArgumentException ex)
        {
            WriteError(ex.Message);
        }
    }

    private static void WriteError(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine($"\n  [Error] {msg}");
        Console.ResetColor();
    }
}