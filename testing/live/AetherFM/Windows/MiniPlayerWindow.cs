using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;

using System.Numerics;
using System.Linq;

namespace AetherFM.Windows;

public class MiniPlayerWindow : Window
{
    private readonly Plugin plugin;
    private bool isPlaying => plugin.RadioManager.GetStatus() == "Playing";
    private float volume = 50f;
    private string stationName
    {
        get
        {
            var url = plugin.Configuration.RadioUrl;
            if (string.IsNullOrEmpty(url)) return "No station";
            string urlNorm(string u) => u?.Trim().ToLowerInvariant().TrimEnd('/').Split('?')[0] ?? "";
            var urlBase = urlNorm(url);
            // Cerca il nome nella lista stazioni della finestra principale
            if (plugin is { })
            {
                var radioWindowField = plugin.GetType().GetField("RadioWindow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (radioWindowField != null)
                {
                    var radioWindow = radioWindowField.GetValue(plugin) as AetherFM.Windows.RadioWindow;
                    if (radioWindow != null)
                    {
                        var stationsField = radioWindow.GetType().GetField("stations", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (stationsField != null)
                        {
                            var stations = stationsField.GetValue(radioWindow) as System.Collections.Generic.List<RadiosureStation>;
                            if (stations != null)
                            {
                                var found = stations.Find(s => urlNorm(s.Url) == urlBase);
                                if (found != null && !string.IsNullOrEmpty(found.Name))
                                    return found.Name;
                            }
                        }
                    }
                }
            }
            // Se non trovato, mostra solo l'URL troncato
            return url.Length > 32 ? url.Substring(0, 29) + "..." : url;
        }
    }

    public MiniPlayerWindow(Plugin plugin) : base("AetherFM Miniplayer", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoScrollbar)
    {
        this.plugin = plugin;
        this.IsOpen = false;
        this.RespectCloseHotkey = false;
        this.volume = plugin.Configuration.Volume;
    }

    public override void Draw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(16, 12));
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(12, 8));
        ImGui.BeginGroup();

        var favs = plugin.Configuration.FavoriteUrls
            .Select(url => plugin.Stations.FirstOrDefault(s => s.Url == url))
            .Where(s => s != null)
            .ToList();
        int currentFavIndex = -1;
        if (favs.Count > 0 && !string.IsNullOrEmpty(plugin.Configuration.RadioUrl))
        {
            currentFavIndex = favs.FindIndex(s => s.Url == plugin.Configuration.RadioUrl);
        }

        // Navigazione preferiti
        if (favs.Count > 0)
        {
            if (ImGui.Button("<<") && favs.Count > 1)
            {
                int idx = currentFavIndex > 0 ? currentFavIndex - 1 : favs.Count - 1;
                var next = favs[idx];
                plugin.Configuration.RadioUrl = next.Url;
                plugin.Configuration.Save();
                plugin.RadioManager.Start(next.Url, null, volume / 100f);
            }
            ImGui.SameLine();
        }

        // Nome stazione (centrato, tooltip su hover)
        var debug = GetStationDebug();
        string displayName = debug.name ?? (debug.url.Length > 32 ? debug.url.Substring(0, 29) + "..." : debug.url);
        ImGui.TextColored(new Vector4(0.85f, 0.95f, 0.95f, 1f), displayName);
        if (ImGui.IsItemHovered())
            ImGui.SetTooltip(debug.url ?? "");
        ImGui.SameLine();

        if (favs.Count > 0)
        {
            if (ImGui.Button(">>") && favs.Count > 1)
            {
                int idx = currentFavIndex >= 0 && currentFavIndex < favs.Count - 1 ? currentFavIndex + 1 : 0;
                var next = favs[idx];
                plugin.Configuration.RadioUrl = next.Url;
                plugin.Configuration.Save();
                plugin.RadioManager.Start(next.Url, null, volume / 100f);
            }
            ImGui.SameLine();
        }

        // Menu a cascata con i preferiti
        if (favs.Count > 0)
        {
            ImGui.SameLine();
            ImGui.TextUnformatted("Favorites:");
            ImGui.SameLine();
            int selectedFav = currentFavIndex >= 0 ? currentFavIndex : 0;
            if (ImGui.BeginCombo("##favCombo", favs.Count > 0 ? favs[selectedFav].Name : "-"))
            {
                for (int i = 0; i < favs.Count; i++)
                {
                    bool isSelected = (i == selectedFav);
                    if (ImGui.Selectable(favs[i].Name, isSelected))
                    {
                        plugin.Configuration.RadioUrl = favs[i].Url;
                        plugin.Configuration.Save();
                        plugin.RadioManager.Start(favs[i].Url, null, volume / 100f);
                    }
                    if (isSelected)
                        ImGui.SetItemDefaultFocus();
                }
                ImGui.EndCombo();
            }
        }

        // Play/Stop
        if (isPlaying)
        {
            if (ImGui.Button("■"))
            {
                plugin.RadioManager.Stop();
            }
        }
        else
        {
            if (ImGui.Button(">"))
            {
                var url = plugin.Configuration.RadioUrl;
                if (!string.IsNullOrEmpty(url))
                    plugin.RadioManager.Start(url, null, volume / 100f);
            }
        }
        ImGui.SameLine();

        // Volume slider (solo icona tooltip)
        ImGui.SetNextItemWidth(90f);
        float prevVolume = volume;
        if (ImGui.SliderFloat("##miniVolume", ref volume, 0f, 100f, "%.0f%%", ImGuiSliderFlags.AlwaysClamp))
        {
            if (isPlaying)
                plugin.RadioManager.SetVolume(volume / 100f);
            plugin.Configuration.Volume = volume;
            plugin.Configuration.Save();
        }
        if (ImGui.IsItemHovered())
            ImGui.SetTooltip("Volume");
        ImGui.SameLine();

        // Chiudi miniplayer
        if (ImGui.Button("X"))
        {
            this.IsOpen = false;
        }

        ImGui.EndGroup();
        ImGui.PopStyleVar(2);
    }

    private (string name, int stationCount, string url) GetStationDebug()
    {
        var url = plugin.Configuration.RadioUrl;
        if (string.IsNullOrEmpty(url)) return ("No station", 0, "");
        string urlNorm(string u) => u?.Trim().ToLowerInvariant().TrimEnd('/').Split('?')[0] ?? "";
        var urlBase = urlNorm(url);
        int count = plugin.Stations?.Count ?? 0;
        string foundName = null;
        if (plugin.Stations != null && plugin.Stations.Count > 0)
        {
            var found = plugin.Stations.Find(s => urlNorm(s.Url) == urlBase);
            if (found != null && !string.IsNullOrEmpty(found.Name))
                foundName = found.Name;
        }
        return (foundName, count, url);
    }
} 