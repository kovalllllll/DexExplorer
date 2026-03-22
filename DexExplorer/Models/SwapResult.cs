namespace DexExplorer.Models;

/// <summary>
/// All outputs produced by one swap calculation
/// </summary>
public sealed record SwapResult(
    decimal AmountIn,
    decimal AmountOut,
    PoolState PoolBefore,
    PoolState PoolAfter,
    decimal EffectivePrice,
    decimal SlippagePercent,
    SwapDirection Direction,
    string Label);