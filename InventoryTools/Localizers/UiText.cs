using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using Dalamud.Plugin.Services;

namespace InventoryTools.Localizers;

public sealed class UiText(IClientState clientState)
{
    private static readonly Lazy<Dictionary<string, string>> Chinese = new(() =>
    {
        using var resource = typeof(UiText).Assembly.GetManifestResourceStream("InventoryTools.Localizers.zh-CN.json")!;
        return JsonSerializer.Deserialize<Dictionary<string, string>>(resource)!;
    });

    private readonly bool _isChinese = clientState.ClientLanguage.ToString() == "ChineseSimplified";

    public string Get(string english)
    {
        return _isChinese ? Chinese.Value.GetValueOrDefault(english, english) : english;
    }

    public string Format(string english, params object?[] arguments)
    {
        return string.Format(CultureInfo.CurrentCulture, Get(english), arguments);
    }

    public string Label(string english)
    {
        var suffix = english.IndexOf("##", StringComparison.Ordinal);
        var caption = suffix < 0 ? english : english[..suffix];
        var translated = Get(caption);
        if (translated == caption)
            return english;

        return suffix >= 0 && english.AsSpan(suffix).StartsWith("###")
            ? translated + english[suffix..]
            : translated + "###" + english;
    }
}
