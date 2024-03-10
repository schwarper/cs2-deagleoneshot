using CounterStrikeSharp.API.Core;
using static CounterStrikeSharp.API.Core.Listeners;

namespace DeagleOneShot;

public class DeagleOneShot : BasePlugin
{
    public override string ModuleName => "Deagle One Shot";
    public override string ModuleVersion => "0.0.1";
    public override string ModuleAuthor => "schwarper";
    public override void Load(bool hotReload)
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
                weaponData.PrimaryReserveAmmoMax = 1;
            }

            weapon.Clip1 = 1;
            weapon.ReserveAmmo[0] = 1;
        });

        RegisterEventHandler<EventWeaponFire>((@event, info) =>
        {
            if (@event.Weapon != "weapon_deagle" || @event.Userid.PlayerPawn.Value?.WeaponServices?.ActiveWeapon.Value is not CBasePlayerWeapon activeweapon)
            {
                return HookResult.Continue;
            }

            activeweapon.ReserveAmmo[0] = 2;

            return HookResult.Continue;
        });
    }
}