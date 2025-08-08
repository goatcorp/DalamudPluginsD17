using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace AetherFM;

public static class RadiosureParser
{
    public static List<RadiosureStation> Parse(string filePath)
    {
        var stations = new List<RadiosureStation>();
        var lines = File.ReadAllLines(filePath);
        for (int i = 1; i < lines.Length; i++) // skip first line (date)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;
            var parts = Regex.Split(line, @"\s{2,}|\t");
            if (parts.Length < 6) continue;
            stations.Add(new RadiosureStation
            {
                Name = parts[0].Trim(),
                Genre = parts[2].Trim(),
                Country = parts[3].Trim(),
                Language = parts[4].Trim(),
                Url = parts[5].Trim()
            });
        }
        return stations;
    }
} 