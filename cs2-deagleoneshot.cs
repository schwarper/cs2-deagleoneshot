using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;
using static CounterStrikeSharp.API.Core.Listeners;

namespace DeagleOneShot;

public class DeagleOneShotConfig : BasePluginConfig
{
    [JsonPropertyName("deagle_reload")] public bool Reload { get; set; } = true;
    [JsonPropertyName("deagle_infinite_clip")] public bool InfiniteClip { get; set; } = true;
}

public class DeagleOneShot : BasePlugin, IPluginConfig<DeagleOneShotConfig>
{
    public override string ModuleName => "Deagle One Shot";
    public override string ModuleVersion => "0.0.2";
    public override string ModuleAuthor => "schwarper";

    public DeagleOneShotConfig Config { get; set; } = new();

    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventWeaponFire>((@event, info) =>
        {
            if (@event.Weapon != "weapon_deagle")
            {
                return HookResult.Continue;
            }

            CCSPlayerController player = @event.Userid;

            if (player == null || !player.IsValid)
            {
                return HookResult.Continue;
            }

            if (!Config.Reload)
            {
                player.ExecuteClientCommand("slot3");

                AddTimer(0.1f, () =>
                {
                    player.ExecuteClientCommand("slot2");
                });
            }

            if (Config.InfiniteClip)
            {
                if (player.PlayerPawn.Value?.WeaponServices?.ActiveWeapon.Value is not CBasePlayerWeapon activeweapon)
                {
                    return HookResult.Continue;
                }

                activeweapon.ReserveAmmo[0] += 1;
            }

            return HookResult.Continue;
        });
    }

    public void OnConfigParsed(DeagleOneShotConfig config)
    {
        if (config.Reload)
        {
            RegisterListener<OnEntitySpawned>((entity) =>
            {
                if (entity.DesignerName != "weapon_deagle")
                {
                    return;
                }

                CBasePlayerWeapon weapon = entity.As<CBasePlayerWeapon>();

                if (weapon == null)
                {
                    return;
                }

                CCSWeaponBaseVData? weaponData = weapon.As<CCSWeaponBase>().VData;

                if (weaponData != null)
                {
                    weaponData.MaxClip1 = 1;
                    weaponData.DefaultClip1 = 1;
                }

                weapon.Clip1 = 1;
            });
        }

        Config = config;
    }
}