/*
 * FILE          : SquareSettings.cs
 * PROJECT       : CSCN72010 - Assignment 3
 * PROGRAMMER    : Muhammad Hassnain
 * FIRST VERSION : 2026-11-17
 */

using System.Text.Json;

namespace LunchSplit.Infrastructure;

/*
 *  NAME    : SquareSettings
 *  PURPOSE : Holds the Square sandbox token, location id, currency and
 *            timeout. Secrets are NEVER written in C# code. Load() looks in
 *            three places, first match wins:
 *              1. environment variable SQUARE_SANDBOX_ACCESS_TOKEN (and
 *                 SQUARE_SANDBOX_LOCATION_ID)
 *              2. appsettings.local.json  (real keys, gitignored)
 *              3. appsettings.json        (blank placeholders, committed)
 */
public record SquareSettings(
    string AccessToken,
    string LocationId,
    string Currency,
    int TimeoutSeconds)
{
    private const string TOKEN_VARIABLE = "SQUARE_SANDBOX_ACCESS_TOKEN";
    private const string LOCATION_VARIABLE = "SQUARE_SANDBOX_LOCATION_ID";
    private const string LOCAL_FILE = "appsettings.local.json";
    private const string COMMITTED_FILE = "appsettings.json";
    private const string SECTION_NAME = "Square";
    private const string DEFAULT_CURRENCY = "CAD";
    private const int DEFAULT_TIMEOUT_SECONDS = 5;

    //
    // FUNCTION    : Load
    // DESCRIPTION :
    //   Reads the settings from environment variables or the json files.
    //   Throws if the token or location id is still missing, WITHOUT
    //   printing any secret value.
    // PARAMETERS  : none
    // RETURNS     : SquareSettings : fully filled-in settings
    //
    public static SquareSettings Load()
    {
        string accessToken = FirstNonEmpty(
            Environment.GetEnvironmentVariable(TOKEN_VARIABLE),
            ReadJsonValue(LOCAL_FILE, "AccessToken"),
            ReadJsonValue(COMMITTED_FILE, "AccessToken"));
        string locationId = FirstNonEmpty(
            Environment.GetEnvironmentVariable(LOCATION_VARIABLE),
            ReadJsonValue(LOCAL_FILE, "LocationId"),
            ReadJsonValue(COMMITTED_FILE, "LocationId"));
        string currency = FirstNonEmpty(
            ReadJsonValue(LOCAL_FILE, "Currency"),
            ReadJsonValue(COMMITTED_FILE, "Currency"),
            DEFAULT_CURRENCY);
        string timeoutText = FirstNonEmpty(
            ReadJsonValue(LOCAL_FILE, "TimeoutSeconds"),
            ReadJsonValue(COMMITTED_FILE, "TimeoutSeconds"));

        if (accessToken.Length == 0 || locationId.Length == 0)
        {
            throw new InvalidOperationException(
                "Square credentials are missing. Set " + TOKEN_VARIABLE + " and "
                + LOCATION_VARIABLE + ", or fill in appsettings.local.json.");
        }

        // Fall back to the default if the timeout is blank or not a number.
        int timeoutSeconds = DEFAULT_TIMEOUT_SECONDS;
        if (int.TryParse(timeoutText, out int parsedSeconds))
        {
            timeoutSeconds = parsedSeconds;
        }

        SquareSettings settings = new SquareSettings(
            accessToken, locationId, currency, timeoutSeconds);

        return settings;
    }

    //
    // FUNCTION    : FirstNonEmpty
    // DESCRIPTION : Picks the first value that is not blank, or "" if all are.
    // PARAMETERS  : string?[] candidates : values in order of priority
    // RETURNS     : string : the winning value
    //
    private static string FirstNonEmpty(params string?[] candidates)
    {
        string winner = candidates.FirstOrDefault(
            candidate => !string.IsNullOrWhiteSpace(candidate)) ?? string.Empty;

        return winner;
    }

    //
    // FUNCTION    : ReadJsonValue
    // DESCRIPTION :
    //   Reads one value from the "Square" section of a json file that sits
    //   next to the running test DLL. A missing file or key gives "".
    // PARAMETERS  :
    //   string fileName     : json file to open
    //   string propertyName : key inside the Square section
    // RETURNS     : string : the value as text, or "" if not found
    //
    private static string ReadJsonValue(string fileName, string propertyName)
    {
        string value = string.Empty;
        string path = Path.Combine(AppContext.BaseDirectory, fileName);

        if (File.Exists(path))
        {
            using JsonDocument document = JsonDocument.Parse(File.ReadAllText(path));

            if (document.RootElement.TryGetProperty(SECTION_NAME, out JsonElement section)
                && section.TryGetProperty(propertyName, out JsonElement element))
            {
                value = element.ToString();
            }
        }

        return value;
    }
}