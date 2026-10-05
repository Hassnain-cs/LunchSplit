# LunchSplit (Core Engine) - CSCN72010 Assignment 1

Restaurant bill-splitting domain logic built with Test-Driven Development in C# / .NET 8, tested with MSTest.

**Author:** Muhammad Hassnain

## Projects
- `LunchSplit.Core` - domain model and engines: Bill, Attendee, Share, Splitter, Rounder, BillValidator, ReceiptFormatter (plus TipCalculator, TipMode, RoundingMode, ValidationResult)
- `LunchSplit.Tests` - MSTest unit tests (the 27-case matrix plus extra tests for edge cases and coverage)
- `LunchSplit.Cli` - small console demo (the only project that does I/O)

## Requirements
- .NET 8 SDK
- Visual Studio 2026 (or any editor plus the `dotnet` CLI)

## Build
Run from the folder that contains the `.sln` file:
```
dotnet build
```

## Run the demo
```
dotnet run --project LunchSplit.Cli
```

## Run the unit tests
```
dotnet test
```
Expected result: all tests pass except **2 intentional RED/PENDING TDD placeholders**
(`Format_CsvReceiptExporter_MatchesSchema` and `CalculateShares_PaymentRequest_ValidatesStripeMock`),
which target future features F5 (CSV export) and F6 (Stripe gateway).

To run everything except the RED placeholders:
```
dotnet test --filter "TestCategory!=RED"
```

## Design notes
- `Bill` is an immutable record; tip math lives only in `TipCalculator` (no duplication).
- **Penny Reconciliation:** `Rounder` rounds each share, then distributes leftover cents one at a time so the sum of the
  shares equals the grand total exactly (e.g. $10.00 / 3 = $3.34 + $3.33 + $3.33).
- Domain classes never read or write the console or files; I/O lives only in the CLI project.
