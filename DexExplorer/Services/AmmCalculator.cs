using DexExplorer.Models;

namespace DexExplorer.Services;

/// <summary>
/// Implements the Uniswap v2 constant-product AMM formula
///
/// Core invariant:  x * y = k
/// Swap formula:    amountOut = (reserveOut * amountInWithFee)
///                              / (reserveIn + amountInWithFee)
/// where:           amountInWithFee = amountIn * (1 - fee)
/// </summary>
public sealed class AmmCalculator : IAmmCalculator
{
    /// <inheritdoc/>
    public SwapResult Calculate(SwapParameters parameters, string label = "")
    {
        //Validate inputs first — fail fast with a clear message
        parameters.Validate();

        var pool = parameters.Pool;
        var amountIn = parameters.AmountIn;

        //Determine which reserve is "in" and which is "out"
        //based on the swap direction.
        var (reserveIn, reserveOut) = parameters.Direction == SwapDirection.AtoB
            ? (pool.ReserveA, pool.ReserveB)
            : (pool.ReserveB, pool.ReserveA);

        // Initial spot price — output tokens per input token
        // A to B: reserveB / reserveA
        // B to A: reserveA / reserveB
        var initialPrice = reserveOut / reserveIn;

        //  Apply the fee to amountIn
        //  The fee is deducted BEFORE the swap calculation
        //  E.g. fee=0.003 means 0.3% of amountIn stays in the pool as LP reward
        var amountInWithFee = amountIn * (1m - pool.Fee);

        // Constant-product formula
        // Derived from: (reserveIn + amountInWithFee) * (reserveOut - amountOut) = k
        // Solving for amountOut gives
        var amountOut = (reserveOut * amountInWithFee)
                        / (reserveIn + amountInWithFee);

        // Full amountIn enters the pool (fee portion stays as extra reserve)
        //  amountOut leaves the pool
        var (newReserveA, newReserveB) = parameters.Direction == SwapDirection.AtoB
            ? (pool.ReserveA + amountIn, pool.ReserveB - amountOut)
            : (pool.ReserveA - amountOut, pool.ReserveB + amountIn);

        var poolAfter = new PoolState(newReserveA, newReserveB, pool.Fee);

        // Effective price — output tokens received per input token spent.
        // Same units as initialPrice, so the two are directly comparable.
        var effectivePrice = amountOut / amountIn;

        // Slippage — how much less output you received vs the zero-impact spot rate.
        // slippage% = (initialPrice - effectivePrice) / initialPrice * 100
        // Always >= 0: a larger trade always moves the price against the buyer.
        var slippagePercent = (initialPrice - effectivePrice) / initialPrice * 100m;

        return new SwapResult(
            AmountIn: amountIn,
            AmountOut: amountOut,
            PoolBefore: pool,
            PoolAfter: poolAfter,
            EffectivePrice: effectivePrice,
            SlippagePercent: slippagePercent,
            Direction: parameters.Direction,
            Label: label);
    }
}