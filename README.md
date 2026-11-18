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

## Assignment 3: Square payment gateway

LunchSplit can now take payments through the Square API (Sandbox only). The core project only knows an interface, `IPaymentProcessor`. All Square-specific code lives in `LunchSplit.Infrastructure/SquarePaymentService.cs`, so the rest of the app never touches the Square SDK.

### Where things are
See `CODEMAP.md` in the repo root for the solution file and the source and test folders.

### Setting up credentials
Real keys are never committed. `SquareSettings.Load()` looks in this order and uses the first value it finds:
1. Environment variables `SQUARE_SANDBOX_ACCESS_TOKEN` and `SQUARE_SANDBOX_LOCATION_ID`
2. `appsettings.local.json` in the test project (gitignored, holds the real keys)
3. `appsettings.json` in the test project (committed, blank placeholders)

To get the values: create a free account at developer.squareup.com, create an application, switch to Sandbox, and copy the Sandbox access token and the location id of the Default Test Account. The currency is `CAD` by default and can be changed in the json file if the sandbox account uses another one.

### Running the tests
From the repo root:

```
dotnet test
```

There are 10 Assignment 3 tests, in two tiers:
- **Tier 1 (live Square sandbox, 5 tests):** M3-01, M3-02, M3-08, M3-09, M3-10 in `Integration/SquareSandboxTests.cs`. These need internet and credentials. Every call has a 5 second timeout, so a dropped connection can't hang the run.
- **Tier 2 (Moq, 5 tests):** M3-03 to M3-07 in `Unit/PaymentProcessorMoqTests.cs`. These run fully in memory.

Expected result: 40 passed, 2 skipped. The two skipped tests are the Assignment 1 placeholders that were deliberately red (`[Ignore]`), so the command line run finishes clean.

### Notes
- **Package:** the assignment says `dotnet add package Square`. The current `Square` package is a rewritten SDK without `PaymentsApi.CreatePaymentAsync`, so this project uses `Square.Legacy` 40.1.0, which still has it.
- **Declined card:** for the sandbox declined nonce, Square actually answers with the code `GENERIC_DECLINE`. `SquarePaymentService` translates that into our own domain code `CARD_DECLINED`, which is what the spec and the M3-02 test expect.

