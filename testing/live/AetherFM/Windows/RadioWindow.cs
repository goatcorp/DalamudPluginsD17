using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;

using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Net;
using Newtonsoft.Json;
using System.Linq;

namespace AetherFM.Windows;

public class RadioBrowserStation
{
    public string name { get; set; }
    public string url_resolved { get; set; }
    public string country { get; set; }
    public string tags { get; set; }
}

public class RadioWindow : Window, IDisposable
{
    private readonly Plugin plugin;
    private string radioUrl;
    private string status = "Stopped";
    private bool isPlaying = false;
    private int selectedStation = -1;
    private string stationSearch = string.Empty;
    private List<string> availableCountries = new List<string> { "Select a country...", "Italy", "United States", "United Kingdom", "France", "Germany", "Spain", "Japan", "Brazil", "Canada", "Australia" };
    private int selectedCountryIndex = 0;
    private float volume = 50f; // Default volume (50%)
    private int selectedSection = 0;

    public RadioWindow(Plugin plugin) : base("AetherFM - Radio Player")
    {
        this.plugin = plugin;
        this.radioUrl = plugin.Configuration.RadioUrl ?? string.Empty;
        // Imposta il paese selezionato dall'ultima sessione
        if (!string.IsNullOrEmpty(plugin.Configuration.LastCountry))
        {
            var idx = availableCountries.IndexOf(plugin.Configuration.LastCountry);
            if (idx >= 0) selectedCountryIndex = idx;
        }
        DownloadStationsFromRadioBrowser(availableCountries[selectedCountryIndex]);
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(600, 400),
            MaximumSize = new Vector2(2000, 1200)
        };
    }

    private void DownloadStationsFromRadioBrowser(string country = null)
    {
        try
        {
            string url;
            if (string.IsNullOrEmpty(country) || country == "All")
                url = "https://de1.api.radio-browser.info/json/stations?limit=500";
            else
                url = $"https://de1.api.radio-browser.info/json/stations/bycountry/{country}?limit=10000";
            using var client = new WebClient();
            var json = client.DownloadString(url);
            var rbStations = JsonConvert.DeserializeObject<List<RadioBrowserStation>>(json);
            // Rimuovi duplicati per URL (case-insensitive)
            var uniqueStations = rbStations
                .Where(s => s.url_resolved != null && s.url_resolved.EndsWith(".mp3") && s.url_resolved.StartsWith("http://"))
                .GroupBy(s => s.url_resolved.Trim().ToLowerInvariant())
                .Select(g => g.First())
                .ToList();
            plugin.Stations = uniqueStations
                .Select(s => new RadiosureStation
                {
                    Name = s.name,
                    Url = s.url_resolved,
                    Country = s.country,
                    Genre = s.tags,
                    Language = ""
                })
                .ToList();
            status = $"Loaded {plugin.Stations.Count} stations from Radio Browser{(string.IsNullOrEmpty(country) || country == "All" ? "" : " for " + country)}";
        }
        catch (Exception ex)
        {
            status = $"Error loading from Radio Browser: {ex.Message}";
        }
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(16, 16));
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(10, 8));

        // Sidebar in alto (verticale, non più colonna separata)
        ImGui.BeginChild("Sidebar", new Vector2(200, 0), true);
        ImGui.Dummy(new Vector2(0, 12));
        string[] sections = { "Stations", "Favorites", "Settings", "About" };
        for (int i = 0; i < sections.Length; i++)
        {
            bool selected = (selectedSection == i);
            if (selected)
            {
                ImGui.PushStyleColor(ImGuiCol.Header, new Vector4(0.26f, 0.84f, 0.46f, 0.25f));
                ImGui.PushStyleColor(ImGuiCol.HeaderHovered, new Vector4(0.36f, 0.94f, 0.56f, 0.35f));
            }
            if (ImGui.Selectable(sections[i], selected, ImGuiSelectableFlags.None, new Vector2(180, 0)))
                selectedSection = i;
            if (selected)
            {
                ImGui.PopStyleColor(2);
            }
            ImGui.Dummy(new Vector2(0, 4));
        }
        ImGui.EndChild();

        ImGui.SameLine();

        // Pannello principale a destra della sidebar
        ImGui.BeginChild("MainPanel", new Vector2(0, 0), false);
        ImGui.TextUnformatted($"AetherFM - {sections[selectedSection]}");
        ImGui.Separator();
        ImGui.Spacing();

        if (selectedSection == 0) // Stations
        {
            // Mostra sempre il selettore paese
            ImGui.TextUnformatted("Country:");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(160f);
            if (ImGui.BeginCombo("##countryCombo", availableCountries[selectedCountryIndex]))
            {
                for (int i = 0; i < availableCountries.Count; i++)
                {
                    bool isSelected = (i == selectedCountryIndex);
                    if (ImGui.Selectable(availableCountries[i], isSelected))
                    {
                        selectedCountryIndex = i;
                        if (i > 0) // Solo se non è il placeholder
                        {
                            plugin.Configuration.LastCountry = availableCountries[i];
                            plugin.Configuration.Save();
                            DownloadStationsFromRadioBrowser(availableCountries[i]);
                        }
                        else
                        {
                            plugin.Configuration.LastCountry = "";
                            plugin.Configuration.Save();
                        }
                    }
                    if (isSelected)
                        ImGui.SetItemDefaultFocus();
                }
                ImGui.EndCombo();
            }
            ImGui.SameLine();
            if (ImGui.Button("Refresh"))
            {
                if (selectedCountryIndex > 0)
                {
                    plugin.Configuration.LastCountry = availableCountries[selectedCountryIndex];
                    plugin.Configuration.Save();
                    DownloadStationsFromRadioBrowser(availableCountries[selectedCountryIndex]);
                }
            }
            if (selectedCountryIndex == 0 || string.IsNullOrEmpty(plugin.Configuration.LastCountry))
            {
                ImGui.Spacing();
                ImGui.TextColored(new Vector4(1f, 0.8f, 0.2f, 1f), "Please select a country from the dropdown to view available radio stations.");
                ImGui.EndChild(); // Chiudo il MainPanel
                ImGui.PopStyleVar(2);
                return;
            }
            // Filtro: barra di ricerca
            ImGui.TextUnformatted("Search:");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(200f);
            ImGui.InputText("##stationSearch", ref stationSearch, 128);
            ImGui.SameLine();
            if (ImGui.Button("Miniplayer"))
            {
                plugin.ToggleMiniPlayer();
            }
            ImGui.Spacing();
            ImGui.Separator();
            // Applica filtri
            var filtered = plugin.Stations
                .Where(s => (string.IsNullOrEmpty(stationSearch) || s.Name.Contains(stationSearch, StringComparison.OrdinalIgnoreCase) || s.Genre.Contains(stationSearch, StringComparison.OrdinalIgnoreCase) || s.Country.Contains(stationSearch, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            Vector2 tableSize = new Vector2(ImGui.GetContentRegionAvail().X, 350);
            if (ImGui.BeginTable("Stations", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY, tableSize))
            {
                ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch, 0.40f);
                ImGui.TableSetupColumn("Country", ImGuiTableColumnFlags.WidthFixed, 90f);
                ImGui.TableSetupColumn("Genre", ImGuiTableColumnFlags.WidthStretch, 0.45f);
                ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.WidthFixed, 90f);
                ImGui.TableHeadersRow();
                for (int i = 0; i < filtered.Count; i++)
                {
                    var station = filtered[i];
                    ImGui.TableNextRow();
                    bool isCurrent = plugin.Configuration.RadioUrl == station.Url && plugin.RadioManager.GetStatus() == "Playing";
                    if (isCurrent)
                        ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0, ImGui.ColorConvertFloat4ToU32(new Vector4(0.26f, 0.84f, 0.46f, 0.25f)));
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted(station.Name);
                    if (ImGui.IsItemHovered())
                        ImGui.SetTooltip($"URL: {station.Url}");
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted(station.Country);
                    ImGui.TableSetColumnIndex(2);
                    ImGui.TextUnformatted(station.Genre);
                    ImGui.TableSetColumnIndex(3);
                    if (isCurrent)
                    {
                        ImGui.PushItemWidth(60f);
                        if (ImGui.Button($"Stop##stop{i}", new Vector2(60, 0)))
                        {
                            plugin.RadioManager.Stop();
                        }
                        ImGui.PopItemWidth();
                    }
                    else
                    {
                        ImGui.PushItemWidth(60f);
                        if (ImGui.Button($"Play##play{i}", new Vector2(60, 0)))
                        {
                            plugin.Configuration.RadioUrl = station.Url;
                            plugin.Configuration.Save();
                            plugin.RadioManager.Start(station.Url, null, plugin.Configuration.Volume / 100f);
                        }
                        ImGui.PopItemWidth();
                    }
                    ImGui.SameLine();
                    // Preferiti: stella piena/vuota
                    bool isFav = plugin.Configuration.FavoriteUrls.Contains(station.Url);
                    string star = isFav ? "★" : "☆";
                    if (ImGui.Button($"{star}##fav{i}"))
                    {
                        if (isFav)
                            plugin.Configuration.FavoriteUrls.Remove(station.Url);
                        else
                            plugin.Configuration.FavoriteUrls.Add(station.Url);
                        plugin.Configuration.Save();
                    }
                    ImGui.SameLine();
                    if (ImGui.Button($"i##info{i}"))
                    {
                        // TODO: mostra popup info
                    }
                }
                ImGui.EndTable();
            }
        }
        else if (selectedSection == 1) // Favorites
        {
            var favs = plugin.Stations.Where(s => plugin.Configuration.FavoriteUrls.Contains(s.Url)).ToList();
            if (favs.Count == 0)
            {
                ImGui.TextUnformatted("No favorite stations yet.");
            }
            else
            {
                Vector2 tableSize = new Vector2(ImGui.GetContentRegionAvail().X, 350);
                if (ImGui.BeginTable("FavStations", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY, tableSize))
                {
                    ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch, 0.40f);
                    ImGui.TableSetupColumn("Country", ImGuiTableColumnFlags.WidthFixed, 90f);
                    ImGui.TableSetupColumn("Genre", ImGuiTableColumnFlags.WidthStretch, 0.45f);
                    ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.WidthFixed, 90f);
                    ImGui.TableHeadersRow();
                    for (int i = 0; i < favs.Count; i++)
                    {
                        var station = favs[i];
                        ImGui.TableNextRow();
                        bool isCurrent = plugin.Configuration.RadioUrl == station.Url && plugin.RadioManager.GetStatus() == "Playing";
                        if (isCurrent)
                            ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0, ImGui.ColorConvertFloat4ToU32(new Vector4(0.26f, 0.84f, 0.46f, 0.25f)));
                        ImGui.TableSetColumnIndex(0);
                        ImGui.TextUnformatted(station.Name);
                        if (ImGui.IsItemHovered())
                            ImGui.SetTooltip($"URL: {station.Url}");
                        ImGui.TableSetColumnIndex(1);
                        ImGui.TextUnformatted(station.Country);
                        ImGui.TableSetColumnIndex(2);
                        ImGui.TextUnformatted(station.Genre);
                        ImGui.TableSetColumnIndex(3);
                        if (isCurrent)
                        {
                            ImGui.PushItemWidth(60f);
                            if (ImGui.Button($"Stop##stop_fav{i}", new Vector2(60, 0)))
                            {
                                plugin.RadioManager.Stop();
                            }
                            ImGui.PopItemWidth();
                        }
                        else
                        {
                            ImGui.PushItemWidth(60f);
                            if (ImGui.Button($"Play##play_fav{i}", new Vector2(60, 0)))
                            {
                                plugin.Configuration.RadioUrl = station.Url;
                                plugin.Configuration.Save();
                                plugin.RadioManager.Start(station.Url, null, plugin.Configuration.Volume / 100f);
                            }
                            ImGui.PopItemWidth();
                        }
                        ImGui.SameLine();
                        // Preferiti: stella piena/vuota
                        bool isFav = plugin.Configuration.FavoriteUrls.Contains(station.Url);
                        string star = isFav ? "★" : "☆";
                        if (ImGui.Button($"{star}##fav_fav{i}"))
                        {
                            if (isFav)
                                plugin.Configuration.FavoriteUrls.Remove(station.Url);
                            else
                                plugin.Configuration.FavoriteUrls.Add(station.Url);
                            plugin.Configuration.Save();
                        }
                        ImGui.SameLine();
                        if (ImGui.Button($"i##info_fav{i}"))
                        {
                            // TODO: mostra popup info
                        }
                    }
                    ImGui.EndTable();
                }
            }
        }
        else if (selectedSection == 2) // Settings
        {
            if (ImGui.CollapsingHeader("General Settings"))
            {
                ImGui.TextUnformatted("(You can add general settings here)");
            }
            if (ImGui.CollapsingHeader("UI Settings"))
            {
                ImGui.TextUnformatted("(You can add UI settings here)");
            }
        }
        else if (selectedSection == 3) // About
        {
            ImGui.TextUnformatted("AetherFM Plugin\nAuthor: SalvatoreDevelopment\nVersion: 1.0.0");
        }
        ImGui.EndChild();

        ImGui.PopStyleVar(2);
    }

    private void OnStatusChanged(string newStatus)
    {
        status = newStatus;
        isPlaying = newStatus == "Playing";
    }
} 