using DexExplorer.Models;

namespace DexExplorer.Services;

/// <summary>
/// Defines the contract for an AMM swap calculator
/// Extracting an interface makes the logic easy to mock in unit tests
/// and easy to swap out (e.g., constant-sum AMM vs. constant-product AMM)
/// </summary>
public interface IAmmCalculator
{
    /// <summary>
    /// Calculates the result of a single swap given the pool state and trade parameters
    /// Does NOT mutate any state — always returns a new <see cref="SwapResult"/>.
    /// </summary>
    SwapResult Calculate(SwapParameters parameters, string label = "");
}