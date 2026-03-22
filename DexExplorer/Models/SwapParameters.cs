namespace DexExplorer.Models;

/// <summary>
/// All inputs required to calculate a single swap
/// </summary>
public sealed record SwapParameters(
    PoolState Pool,
    SwapDirection Direction,
    decimal AmountIn)
{
    /// <summary>
    /// Validates that inputs are logically consistent
    /// Called by the calculator before any math runs
    /// </summary>
    public void Validate()
    {
        if (Pool.ReserveA <= 0) throw new ArgumentException("ReserveA must be positive.", nameof(Pool));
        if (Pool.ReserveB <= 0) throw new ArgumentException("ReserveB must be positive.", nameof(Pool));
        if (Pool.Fee is < 0 or >= 1) throw new ArgumentException("Fee must be in [0, 1).", nameof(Pool));
        if (AmountIn <= 0) throw new ArgumentException("AmountIn must be positive.", nameof(AmountIn));
    }
}