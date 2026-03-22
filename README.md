# Constant-Product DEX Explorer

A console simulator for swaps on a Uniswap v2-style AMM.
Demonstrates how slippage grows non-linearly with trade size.

## How to run

```bash
dotnet run --project src/DexExplorer
```

Use the menu to either run predefined scenarios or enter custom parameters.

---

## Formulas

### Invariant
```
x * y = k
```
The product of the two reserves must remain constant after every swap.
In practice, `k` grows slightly with each trade because the fee stays in the pool.

### Swap output
```
amountInWithFee = amountIn * (1 - fee)
amountOut       = (reserveOut * amountInWithFee) / (reserveIn + amountInWithFee)
```

### Effective price (output tokens per input token)
```
effectivePrice = amountOut / amountIn
```

### Slippage vs initial price
```
initialPrice = reserveOut / reserveIn   ← reserveB/reserveA for AtoB, reserveA/reserveB for BtoA
slippage %   = (initialPrice - effectivePrice) / initialPrice * 100
```
A positive result means you received fewer output tokens per input token
than the zero-impact spot rate would give.

---

## Pool used in scenarios

| Parameter      | Value               |
|----------------|---------------------|
| Reserve A      | 1 000 ETH           |
| Reserve B      | 1 500 000 USDC      |
| Fee            | 0.3%                |
| Spot Price A→B | 1 500.0000 USDC/ETH |
| Spot Price B→A | 0.000667 ETH/USDC   |
| Invariant k    | 1 500 000 000       |

---

## Scenario results

### A to B (sell ETH, receive USDC) — reference price: reserveB / reserveA = 1 500.000000 USDC/ETH

| Scenario      | AmountIn (ETH) | AmountOut (USDC) | New Reserve A | New Reserve B   | Eff. Price (USDC/ETH) | Slippage |
|---------------|---------------:|-----------------:|--------------:|----------------:|----------------------:|---------:|
| Small  (~1%)  |          10.00 |    14 807.3705   |     1 010.00  | 1 485 192.6295  |          1 480.737052 |  1.2842% |
| Medium (~10%) |         100.00 |   135 991.6341   |     1 100.00  | 1 364 008.3659  |          1 359.916341 |  9.3389% |
| Large  (~35%) |         350.00 |   388 024.0187   |     1 350.00  | 1 111 975.9813  |          1 108.640053 | 26.0907% |

### B to A (sell USDC, receive ETH) — reference price: reserveA / reserveB = 0.000667 ETH/USDC

| Scenario      | AmountIn (USDC) | AmountOut (ETH) | New Reserve A | New Reserve B   | Eff. Price (ETH/USDC) | Slippage |
|---------------|----------------:|----------------:|--------------:|----------------:|----------------------:|---------:|
| Small  (~1%)  |      15 000.00  |          9.8716 |      990.1284 | 1 515 000.0000  |              0.000658 |  1.2842% |
| Medium (~10%) |     150 000.00  |         90.6611 |      909.3389 | 1 650 000.0000  |              0.000604 |  9.3389% |
| Large  (~35%) |     525 000.00  |        258.6827 |      741.3173 | 2 025 000.0000  |              0.000493 | 26.0907% |

---

## Conclusions

- **Slippage is non-linear.** Going from a 1% trade to a 35% trade (35× larger)
  increases slippage from ~1.3% to ~26% — roughly a 20× increase for a 35× increase
  in trade size. Doubling the trade size always more than doubles the slippage.

- **Both swap directions are symmetric.** A 1% trade of reserveA in the A→B direction
  produces exactly the same slippage as a 1% trade of reserveB in the B→A direction.
  This follows directly from the symmetric shape of the x * y = k curve.

- **The fee is negligible for large trades.** For a 1% swap, the 0.3% fee accounts
  for roughly a quarter of total slippage. For a 35% swap, price impact alone
  drives nearly all of the ~26% slippage — the fee becomes insignificant.