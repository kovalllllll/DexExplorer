using DexExplorer.Models;

namespace DexExplorer.Reporting;

public static class ConsoleReporter
{
    private const int Width = 72;
    private static readonly string Line = new('─', Width);

    public static void PrintPoolHeader(PoolState pool)
    {
        Console.WriteLine();
        Console.WriteLine("╔" + new string('═', Width) + "╗");
        Console.WriteLine($"║{"  Constant-Product DEX Explorer  ",-(Width)}║");
        Console.WriteLine("╚" + new string('═', Width) + "╝");
        Console.WriteLine();
        Console.WriteLine("  Initial Pool State");
        Console.WriteLine($"  {"Reserve A (ETH)",-26} {pool.ReserveA,12:N2}");
        Console.WriteLine($"  {"Reserve B (USDC)",-26} {pool.ReserveB,12:N2}");
        Console.WriteLine($"  {"Fee",-26} {pool.Fee * 100,11:N2}%");
        Console.WriteLine();

        // Both spot prices shown upfront — used as reference values
        // for slippage calculation in each swap direction
        Console.WriteLine($"  {"Spot Price A to B (USDC/ETH)",-26} {pool.SpotPriceAInB,12:N4}");
        Console.WriteLine($"  {"Spot Price B to A (ETH/USDC)",-26} {pool.SpotPriceBInA,12:N6}");
        Console.WriteLine($"  {"Invariant k",-26} {pool.K,12:N0}");
        Console.WriteLine();
    }

    public static void PrintSwapResult(SwapResult result)
    {
        // Both idealPrice and effectivePrice are in "output per input" units —
        // directly comparable to each other, so slippage is simply the % gap between them
        var (tokenIn, tokenOut, priceUnit, idealPrice) = result.Direction == SwapDirection.AtoB
            ? ("ETH", "USDC", "USDC/ETH (output per input)", result.PoolBefore.SpotPriceAInB)
            : ("USDC", "ETH", "ETH/USDC (output per input)", result.PoolBefore.SpotPriceBInA);

        Console.WriteLine(Line);
        Console.WriteLine($"  Scenario : {result.Label}");
        Console.WriteLine(Line);

        Console.WriteLine($"  {"Amount In",-30} {result.AmountIn,12:N4}  {tokenIn}");
        Console.WriteLine($"  {"Amount Out",-30} {result.AmountOut,12:N4}  {tokenOut}");
        Console.WriteLine();

        Console.WriteLine($"  {"New Reserve A (ETH)",-30} {result.PoolAfter.ReserveA,12:N4}");
        Console.WriteLine($"  {"New Reserve B (USDC)",-30} {result.PoolAfter.ReserveB,12:N4}");
        Console.WriteLine($"  {"New k (>= original k)",-30} {result.PoolAfter.K,12:N2}");
        Console.WriteLine();

        // idealPrice   = reserveOut / reserveIn  (spot, zero-impact)
        // effectivePrice = amountOut / amountIn  (actual, includes price impact)
        // Both in the same units → slippage is the % gap between them
        Console.WriteLine($"  {"Initial Price (spot)",-30} {idealPrice,12:N6}  {priceUnit}");
        Console.WriteLine($"  {"Effective Price",-30} {result.EffectivePrice,12:N6}  {priceUnit}");
        Console.WriteLine();

        WriteSlippage(result.SlippagePercent);
        Console.WriteLine();
    }

    public static void PrintSummaryTable(IReadOnlyList<SwapResult> results)
    {
        Console.WriteLine(Line);
        Console.WriteLine("  SUMMARY TABLE");
        Console.WriteLine(Line);

        PrintGroup(
            "A to B  (sell ETH, receive USDC)  — ref: reserveB / reserveA",
            results.Where(r => r.Direction == SwapDirection.AtoB));

        PrintGroup(
            "B to A  (sell USDC, receive ETH)  — ref: reserveA / reserveB",
            results.Where(r => r.Direction == SwapDirection.BtoA));
    }

    private static void PrintGroup(string header, IEnumerable<SwapResult> group)
    {
        Console.WriteLine();
        Console.WriteLine($"  {header}");
        Console.WriteLine($"  {"Scenario",-32} {"AmountIn",10} {"AmountOut",13} {"Slippage",10}");
        Console.WriteLine($"  {new string('-', 68)}");

        foreach (var r in group)
            Console.WriteLine(
                $"  {r.Label,-32} {r.AmountIn,10:N2} {r.AmountOut,13:N4} {r.SlippagePercent,9:N4}%");

        Console.WriteLine();
    }

    private static void WriteSlippage(decimal slippage)
    {
        var colour = slippage switch
        {
            < 0.5m => ConsoleColor.Green,
            < 5.0m => ConsoleColor.Yellow,
            _ => ConsoleColor.Red
        };

        Console.Write($"  {"Slippage",-30} ");
        var prev = Console.ForegroundColor;
        Console.ForegroundColor = colour;
        Console.WriteLine($"{slippage,12:N4}%");
        Console.ForegroundColor = prev;
    }
}