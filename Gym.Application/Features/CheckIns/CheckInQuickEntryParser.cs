namespace Gym.Application.CheckIns;

/// <summary>
/// Recognizes explicit front-desk commands without guessing member identities.
/// The vocabulary is intentionally small and independent of tenant data.
/// </summary>
public static class CheckInQuickEntryParser
{
    private static readonly HashSet<string> Commands = new(StringComparer.OrdinalIgnoreCase)
    {
        "checkin", "check", "in", "member"
    };

    public static string ExtractMemberLookup(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var trimmed = input.Trim();
        // Only interpret a command prefix if an explicit identifier follows it.
        // Raw member codes and phone numbers must continue to work unchanged.
        if (trimmed.Length > 128)
            return trimmed;

        var compact = string.Concat(trimmed.Where(c => !char.IsWhiteSpace(c)));
        if (!TryParseCommand(compact, out var identifier))
            return trimmed;

        return identifier;
    }

    private static bool TryParseCommand(string compact, out string identifier)
    {
        identifier = string.Empty;
        var memo = new HashSet<int>();
        return ParseAt(0);

        bool ParseAt(int position)
        {
            if (memo.Contains(position))
                return false;

            foreach (var command in Commands.OrderByDescending(x => x.Length))
            {
                if (!compact.AsSpan(position).StartsWith(command, StringComparison.OrdinalIgnoreCase))
                    continue;

                int next = position + command.Length;
                if (next == compact.Length)
                    continue;

                // A prefix is meaningful only for an identifier shaped like a member
                // code or a phone number. Unknown text is never silently rewritten.
                var suffix = compact[next..];
                if (LooksLikeIdentifier(suffix))
                {
                    identifier = suffix;
                    return true;
                }

                if (ParseAt(next))
                    return memo[position] = true;
            }

            memo.Add(position);
            return false;
        }
    }

    private static bool LooksLikeIdentifier(string value)
    {
        if (value.Length is < 4 or > 64)
            return false;

        if (value.StartsWith("MEM-", StringComparison.OrdinalIgnoreCase))
            return value.Length > 4 && value[4..].All(char.IsAsciiLetterOrDigit);

        if (value.StartsWith("MEM", StringComparison.OrdinalIgnoreCase))
            return value.Length > 3 && value[3..].All(char.IsAsciiDigit);

        return value.All(char.IsAsciiDigit);
    }
}
