using System;
using System.Collections.Generic;

namespace Mercado.Shared.Models;

public static class AppInfo {
    public static readonly string ID = "uy.cyt.mercado";
    public static readonly string EnglishShortName = "Market";
    public static readonly string Version = "2024.1.0";
    public static readonly string ShortName = "Mercado";
    public static readonly string Description = "Una simple aplicación para administrar un mercado";
    public static readonly Uri SourceRepo = new("https://github.com/Nahuelche13/Mercado");
    public static readonly Uri IssueTracker = new("https://github.com/Nahuelche13/Mercado/issues/new");
    public static readonly Uri SupportUrl = new("https://github.com/Nahuelche13/Mercado/discussions");
    public static readonly Dictionary<string, Uri> Developers = new(){
        {"Nahuelche", new Uri("https://github.com/Nahuelche13")},
    };
    public static readonly Dictionary<string, Uri> Designers = new(){
        {"Nahuelche", new Uri("https://github.com/Nahuelche13")},
    };
    public static readonly Dictionary<string, Uri> Artists = new(){
        {"Nahuelche", new Uri("https://github.com/Nahuelche13")},
    };
    public static readonly string TranslatorCredits = "translator-credits";

    public static readonly Dictionary<string, Uri> ExtraLinks = [];

    public static bool IsDevVersion => Version.Contains('-');
    public static List<string> TranslatorNames
    {
        get
        {
            var result = new List<string>();
            foreach (var line in TranslatorCredits.Split("\n"))
            {
                if (line.IndexOf("<") > -1)
                {
                    result.Add(line.Substring(0, line.IndexOf("<")).Trim());
                }
                else if (line.IndexOf("http") > -1)
                {
                    result.Add(line.Substring(0, line.IndexOf("http")).Trim());
                }
                else
                {
                    result.Add(line);
                }
            }
            return result;
        }
    }

    public static string[] ConvertURLDictToArray(Dictionary<string, Uri> dict)
    {
        string[] arr = new string[dict.Count];
        int i = 0;
        foreach (KeyValuePair<string, Uri> pair in dict)
        {
            arr[i] = $"{pair.Key} {pair.Value}";
            i++;
        }
        return arr;
    }
}