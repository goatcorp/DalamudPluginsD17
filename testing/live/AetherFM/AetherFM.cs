using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using AetherFM.Windows;
using System.Collections.Generic;
using Dalamud.Bindings.ImGui;


namespace AetherFM;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;

    private const string CommandName = "/aetherfm";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("AetherFM");
    private ConfigWindow ConfigWindow { get; init; }
    public RadioManager RadioManager { get; private set; }
    private RadioWindow RadioWindow { get; init; }
    private MiniPlayerWindow MiniPlayerWindow { get; init; }

    public List<RadiosureStation> Stations { get; set; } = new();

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        ConfigWindow = new ConfigWindow(this);
        RadioManager = new RadioManager();
        RadioWindow = new RadioWindow(this);
        MiniPlayerWindow = new MiniPlayerWindow(this);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(RadioWindow);
        WindowSystem.AddWindow(MiniPlayerWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "AetherFM: listen to radio in FFXIV!"
        });

        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;
        PluginInterface.UiBuilder.OpenMainUi += ToggleRadioUI;

        Log.Information($"===AetherFM started!===");
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();
        ConfigWindow.Dispose();
        RadioManager.Dispose();
        CommandManager.RemoveHandler(CommandName);
        MiniPlayerWindow.IsOpen = false;
    }

    private void OnCommand(string command, string args)
    {
        if (!string.IsNullOrWhiteSpace(args) && args.Trim().ToLowerInvariant() == "miniplayer")
        {
            ToggleMiniPlayer();
            return;
        }
        ToggleRadioUI();
    }

    private void DrawUI()
    {
        WindowSystem.Draw();
        // Se tutte le finestre sono chiuse, ferma la radio
        if (!RadioWindow.IsOpen && !MiniPlayerWindow.IsOpen)
        {
            if (RadioManager.GetStatus() == "Playing")
                RadioManager.Stop();
        }
    }

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleRadioUI() => RadioWindow.Toggle();
    public void ToggleMiniPlayer() => MiniPlayerWindow.Toggle();
} 