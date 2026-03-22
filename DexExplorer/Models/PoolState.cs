namespace DexExplorer.Models;

/// <summary>
/// Represents a snapshot of the liquidity pool's reserves at a given moment
/// Both reserves must always satisfy the invariant: ReserveA * ReserveB = k
/// </summary>
public sealed record PoolState(
    decimal ReserveA,
    decimal ReserveB,
    decimal Fee)
{
    /// <summary>
    /// The invariant k = x * y. It should remain constant after every swap
    /// (the fee slightly increases it over time, which benefits liquidity providers)
    /// </summary>
    public decimal K => ReserveA * ReserveB;

    /// <summary>
    /// Mid-price how many units of B you get per 1 unit of A at zero trade size
    /// This is the "ideal" price — slippage is measured against this
    /// </summary>
    public decimal SpotPriceAInB => ReserveB / ReserveA;

    /// <summary>
    /// Mid-price in the opposite direction units of A per 1 unit of B
    /// </summary>
    public decimal SpotPriceBInA => ReserveA / ReserveB;
}