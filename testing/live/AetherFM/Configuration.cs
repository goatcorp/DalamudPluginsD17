using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;

namespace AetherFM;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool IsConfigWindowMovable { get; set; } = true;
    public string RadioUrl { get; set; } = "";
    public float Volume { get; set; } = 50f;
    public string Language { get; set; } = "en";
    public string LastCountry { get; set; } = "";
    public List<string> FavoriteUrls { get; set; } = new List<string>();

    // the below exist just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
