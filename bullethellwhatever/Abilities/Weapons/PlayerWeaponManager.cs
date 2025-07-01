using bullethellwhatever.BaseClasses;
using bullethellwhatever.DrawCode.UI;
using bullethellwhatever.DrawCode.UI.Player;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Abilities.Weapons
{
    public static class PlayerWeaponManager
    {
        public static Weapon[] AvailableWeapons;
        public static Weapon ActiveWeapon;
        public static int LastWeaponIndex;
        public static int ActiveWeaponIndex;
        public static Dictionary<Weapon, Keys> Keybinds;
        public static int WeaponSwitchCooldown = 10;
        public static int WeaponSwitchCooldownTimer = 0;
        public static void Initialise(Player owner)
        {
            int initialWeaponIndex = 0;

            WeaponSwitchCooldown = 10;
            WeaponSwitchCooldownTimer = 0;

            LastWeaponIndex = initialWeaponIndex;
            Keybinds = new Dictionary<Weapon, Keys>();

            AvailableWeapons = [new SwordWeapon(owner, "HomingWeaponIcon"), new LaserWeapon(owner, "LaserWeaponIcon"), new SharpShooter(owner, "MachineWeaponIcon")];
            Keys[] keybinds = [Keys.D1, Keys.D2, Keys.D3];

            MapKeybinds(AvailableWeapons, keybinds);

            SwitchWeapon(initialWeaponIndex, true);
        }

        public static void MapKeybinds(Weapon[] weapons, Keys[] keybinds)
        {
            Keybinds.Clear();

            for (int i = 0; i < weapons.Length; i++)
            {
                Keybinds.Add(weapons[i], keybinds[i]);
            }
        }
        public static void SwitchWeapon(int index, bool forceSwitch = false)
        {
            // this code needs to be run at the start when the weapon is set to be initially zero. the override bool can be used here.

            if (index != ActiveWeaponIndex || forceSwitch) // dont bother switching to the same weapon
            {
                LastWeaponIndex = ActiveWeaponIndex;
                ActiveWeapon = AvailableWeapons[index];
                ActiveWeaponIndex = index;
                ActiveWeapon.Initialise();
                WeaponSwitchCooldownTimer = WeaponSwitchCooldown;
                UIManager.PlayerHUD.BeginRotation();
            }
        }
        public static void Update()
        {
            if (WeaponSwitchCooldownTimer > 0)
            {
                WeaponSwitchCooldownTimer--;
            }

            if (WeaponSwitchCooldownTimer == 0 && ActiveWeapon.CanSwitchWeapon())
            {
                for (int i = 0; i < AvailableWeapons.Length; i++)
                {
                    if (KeyPressed(Keybinds[AvailableWeapons[i]]))
                    {
                        SwitchWeapon(i);
                        break; // to prevent multiple weapons being held at once and casuing issues
                    }
                }
            }

            ActiveWeapon.BaseUpdate();
            ActiveWeapon.AI();
        }
    }
}
