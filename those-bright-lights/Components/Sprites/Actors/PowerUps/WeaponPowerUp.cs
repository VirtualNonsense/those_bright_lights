using Microsoft.Xna.Framework.Audio;
using SE_Praktikum.Components.Sprites.Actors.Weapons;
using SE_Praktikum.Services;
using System.Collections.Generic;

namespace SE_Praktikum.Components.Sprites.Actors.PowerUps
{
    // Weapon power up
    public class WeaponPowerUp : PowerUp
    {
        public readonly List <Weapon> WeaponList;
        public WeaponPowerUp(AnimationHandler animationHandler, List <Weapon> weaponList, float health = 0.01f, SoundEffect soundEffect = null) : base(animationHandler, health, soundEffect)
        {
            WeaponList = weaponList;
        }
    }
}
