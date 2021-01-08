﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using NLog;
using SE_Praktikum.Components.Sprites;
using SE_Praktikum.Components.Sprites.Bullets;
using SE_Praktikum.Models;
using SE_Praktikum.Services;
using SE_Praktikum.Services.Factories;

namespace SE_Praktikum.Components.Sprites.Weapons
{
    public class SingleShotWeapon : ClipWeapon
    {
        private readonly Func<Bullet> _constructPreconfiguredBullet;
        
        /// <summary>
        /// A precise gun that fires one shot at a time.
        /// </summary>
        /// <param name="Parent">the owner of the gun. it will be assigned to each bullet as well</param>
        /// <param name="shotSoundEffect"></param>
        /// <param name="clipEmptySound"></param>
        /// <param name="weaponEmptySound"></param>
        /// <param name="reloadSound"></param>
        /// <param name="nameTag">Guns can have names i guess. Don't use this for weapon differentiation</param>
        /// <param name="clipSize">amount of bullets in one magazine</param>
        /// <param name="clips">amount of full magazine</param>
        /// <param name="constructPreconfiguredBullet">method that will create a bullet</param>
        /// <param name="shotCoolDown">in milliseconds</param>
        /// <param name="reloadTime">in milliseconds</param>
        public SingleShotWeapon(Actor Parent,
                                SoundEffect shotSoundEffect, 
                                SoundEffect clipEmptySound,
                                SoundEffect weaponEmptySound,
                                SoundEffect reloadSound,
                                string nameTag,
                                int clipSize,
                                int clips,
                                Func<Bullet> constructPreconfiguredBullet,
                                int shotCoolDown = 10,
                                int reloadTime = 1000)
            : base(Parent,
                shotSoundEffect,
                clipEmptySound,
                weaponEmptySound,
                reloadSound,
                nameTag,
                clipSize,
                clips,
                shotCoolDown,
                reloadTime)
        {
            _constructPreconfiguredBullet = constructPreconfiguredBullet;
        }

        protected override Bullet GetBullet()
        {
            var b = _constructPreconfiguredBullet();
            b.Parent = Parent;
            b.Position = Parent.Position;
            b.Rotation = Parent.Rotation;
            b.Velocity = Parent.Velocity;
            b.Layer = Parent.Layer;
            return b;
        }
    }
}