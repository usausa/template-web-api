namespace Template.ApiServer.Host.Infrastructure.Http;

using Microsoft.Net.Http.Headers;

// バージョン番号を強い ETag として扱う
public static class EntityTag
{
    public static string From(int version) => $"\"{version}\"";

    // If-Match なしと * は無条件(version = null)。数値でない ETag と弱い ETag はどの版にも一致しない
    public static bool TryParseIfMatch(string? value, out int? version)
    {
        version = null;
        if (String.IsNullOrEmpty(value) || value == "*")
        {
            return true;
        }

        if (EntityTagHeaderValue.TryParse(value, out var tag) && !tag.IsWeak && Int32.TryParse(tag.Tag.AsSpan(1, tag.Tag.Length - 2), out var parsed))
        {
            version = parsed;
            return true;
        }

        return false;
    }
}
