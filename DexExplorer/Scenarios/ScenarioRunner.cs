using DexExplorer.Models;
using DexExplorer.Services;

namespace DexExplorer.Scenarios;

/// <summary>
/// Defines and runs swap scenarios — three in each direction
/// so both SpotPriceAInB (A to B) and SpotPriceBInA (B to A) are demonstrated
/// </summary>
public sealed class ScenarioRunner(IAmmCalculator calculator)
{
    private const decimal InitialReserveA = 1_000m; // ETH
    private const decimal InitialReserveB = 1_500_000m; // USDC
    private const decimal PoolFee = 0.003m; // 0.3%

    public IReadOnlyList<SwapResult> RunAll()
    {
        var pool = new PoolState(InitialReserveA, InitialReserveB, PoolFee);

        return
        [
            //  Sell ETH → receive USDC (A to B) 
            // Reference price: SpotPriceAInB = reserveB / reserveA = 1500 USDC/ETH

            Run(pool, SwapDirection.AtoB, InitialReserveA * 0.01m,
                label: "A to B  Small  (~1%  of reserveA)"),

            Run(pool, SwapDirection.AtoB, InitialReserveA * 0.10m,
                label: "A to B  Medium (~10% of reserveA)"),

            Run(pool, SwapDirection.AtoB, InitialReserveA * 0.35m,
                label: "A to B  Large  (~35% of reserveA)"),

            //  Sell USDC → receive ETH (B to A) 
            // Reference price: SpotPriceBInA = reserveA / reserveB = 0.000667 ETH/USDC
            // We size these trades as % of reserveB for a fair comparison

            Run(pool, SwapDirection.BtoA, InitialReserveB * 0.01m,
                label: "B to A  Small  (~1%  of reserveB)"),

            Run(pool, SwapDirection.BtoA, InitialReserveB * 0.10m,
                label: "B to A  Medium (~10% of reserveB)"),

            Run(pool, SwapDirection.BtoA, InitialReserveB * 0.35m,
                label: "B to A  Large  (~35% of reserveB)"),
        ];
    }

    private SwapResult Run(PoolState pool, SwapDirection direction,
        decimal amountIn, string label)
        => calculator.Calculate(new SwapParameters(pool, direction, amountIn), label);
}