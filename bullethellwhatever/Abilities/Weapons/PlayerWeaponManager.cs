using bullethellwhatever.BaseClasses;
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
        public static Dictionary<Weapon, Keys> Keybinds;
        public static int WeaponSwitchCooldown;
        public static int WeaponSwitchCooldownTimer = 20;
        public static void Initialise(Player owner)
        {
            WeaponSwitchCooldown = 0;
            Keybinds = new Dictionary<Weapon, Keys>();

            AvailableWeapons = [new HomingWeapon(owner), new MachineWeapon(owner)];
            Keys[] keybinds = [Keys.D1, Keys.D2];

            MapKeybinds(AvailableWeapons, keybinds);

            SwitchWeapon(1);
        }

        public static void MapKeybinds(Weapon[] weapons, Keys[] keybinds)
        {
            Keybinds.Clear();

            for (int i = 0; i < weapons.Length; i++)
            {
                Keybinds.Add(weapons[i], keybinds[i]);
            }
        }
        public static void SwitchWeapon(int index)
        {
            ActiveWeapon = AvailableWeapons[index];
            ActiveWeapon.Initialise();
        }
        public static void Update()
        {
            if (WeaponSwitchCooldownTimer > 0)
            {
                WeaponSwitchCooldownTimer--;
            }

            if (WeaponSwitchCooldownTimer == 0)
            {
                for (int i = 0; i < AvailableWeapons.Length; i++)
                {
                    if (IsKeyPressed(Keybinds[AvailableWeapons[i]]))
                    {
                        SwitchWeapon(i);
                    }
                }
            }

            ActiveWeapon.BaseUpdate();
            ActiveWeapon.AI();
        }
    }
}
