using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AntiBaldaGame.Models;

public class OfflineDictionary
{
    private static readonly Lazy<string[]> words = new(() =>
    {
        List<string> list = [];
        var location = Assembly.GetExecutingAssembly().Location;
        var path = $"{Path.GetDirectoryName(location)![..^16]}/Assets/words.txt";
        using var fs = new StreamReader(path);
        string? s;
        while ((s = fs.ReadLine()) != null)
            list.Add(s);
        return [.. list];
    });

    public static string GetRandom => words.Value[Random.Shared.Next() % words.Value.Length];
}
