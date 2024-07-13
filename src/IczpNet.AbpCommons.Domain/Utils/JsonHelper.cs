using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;

namespace IczpNet.AbpCommons.Utils;

public static class JsonHelper
{
    public static List<string> ParseToList(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }
        using var document = JsonDocument.Parse(json);

        var builder = ImmutableArray.CreateBuilder<string>(document.RootElement.GetArrayLength());

        foreach (var element in document.RootElement.EnumerateArray())
        {
            var value = element.GetString();
            if (string.IsNullOrEmpty(value))
            {
                continue;
            }

            builder.Add(value);
        }

        return [.. builder];
    }
}
