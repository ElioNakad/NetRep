namespace MyAzureDemo.Utilities;

public static class DotEnvLoader
{
    public static void Load()
    {
        var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");

        if (!File.Exists(envPath))
        {
            return;
        }

        foreach (var rawLine in File.ReadLines(envPath))
        {
            var line = rawLine.Trim();

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
            {
                continue;
            }

            var separatorIndex = line.IndexOf('=');

            if (separatorIndex <= 0)
            {
                continue;
            }

            var key = line[..separatorIndex].Trim();
            var value = line[(separatorIndex + 1)..].Trim().Trim('"', '\'');

            if (string.IsNullOrWhiteSpace(key) || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
            {
                continue;
            }

            Environment.SetEnvironmentVariable(key, value);
        }
    }
}
