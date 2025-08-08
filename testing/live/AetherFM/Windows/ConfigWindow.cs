using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;


namespace AetherFM.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;

    // We give this window a constant ID using ###
    // This allows for labels being dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public ConfigWindow(Plugin plugin) : base("AetherFM Configuration###AetherFMConfig")
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(400, 200);
        SizeCondition = ImGuiCond.Always;

        Configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void PreDraw()
    {
        // Flags must be added or removed before Draw() is being called, or they won't apply
        if (Configuration.IsConfigWindowMovable)
        {
            Flags &= ~ImGuiWindowFlags.NoMove;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoMove;
        }
    }

    public override void Draw()
    {
        ImGui.TextUnformatted("AetherFM Configuration");
        ImGui.Separator();
        
        ImGui.Spacing();
        
        // Default radio URL setting
        var radioUrl = Configuration.RadioUrl ?? "";
        if (ImGui.InputText("Default Radio URL", ref radioUrl, 500))
        {
            Configuration.RadioUrl = radioUrl;
            Configuration.Save();
        }
        
        ImGui.Spacing();
        
        // Window behavior settings
        var movable = Configuration.IsConfigWindowMovable;
        if (ImGui.Checkbox("Movable Configuration Window", ref movable))
        {
            Configuration.IsConfigWindowMovable = movable;
            Configuration.Save();
        }
        
        ImGui.Spacing();
        
        ImGui.TextUnformatted("Settings will be saved automatically.");
    }
}
